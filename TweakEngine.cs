using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace WindowsTweaks
{
    /// <summary>
    /// Движок для применения системных твиков Windows
    /// Добавлена поддержка отмены твиков + 13 новых твиков
    /// </summary>
    public class TweakEngine
    {
        private readonly HashSet<string> enabledTweaks;
        private readonly Dictionary<string, TweakAction> tweakActions;
        private readonly List<string> appliedTweaks;
        private readonly List<string> failedTweaks;
        private readonly string statePath;

        public TweakEngine()
        {
            enabledTweaks = new HashSet<string>();
            appliedTweaks = new List<string>();
            failedTweaks = new List<string>();
            tweakActions = InitializeTweakActions();

            statePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "WindowsTweaks",
                "applied_tweaks.txt"
            );

            LoadAppliedTweaksState();

            // Синхронизируем enabledTweaks с уже применёнными твиками.
            // Без этого при старте enabledTweaks пуст, и кнопка "Отменить"
            // считает ВСЕ применённые твики "отключёнными" — и предлагает их отменить.
            foreach (var tweak in appliedTweaks)
                enabledTweaks.Add(tweak);
        }

        public void EnableTweak(string tweakKey)
        {
            enabledTweaks.Add(tweakKey);
        }

        public void DisableTweak(string tweakKey)
        {
            enabledTweaks.Remove(tweakKey);
        }

        public bool IsTweakApplied(string tweakKey)
        {
            return appliedTweaks.Contains(tweakKey);
        }

        public bool IsTweakEnabled(string tweakKey)
        {
            return enabledTweaks.Contains(tweakKey);
        }

        public List<string> GetAppliedTweaks()
        {
            return new List<string>(appliedTweaks);
        }

        /// <summary>
        /// Возвращает список твиков, которые включены (галочка стоит), но ещё не применены к системе.
        /// Используется кнопкой "Применить".
        /// </summary>
        public List<string> GetEnabledButNotAppliedTweaks()
        {
            return enabledTweaks
                .Where(t => !appliedTweaks.Contains(t))
                .ToList();
        }

        /// <summary>
        /// Применяет конкретный список твиков (по кнопке "Применить").
        /// </summary>
        public async Task ApplySelectedTweaksAsync(List<string> tweakKeys)
        {
            failedTweaks.Clear();

            await Task.Run(() =>
            {
                foreach (var tweakKey in tweakKeys)
                {
                    if (tweakActions.ContainsKey(tweakKey))
                    {
                        try
                        {
                            tweakActions[tweakKey].Apply();
                            if (!appliedTweaks.Contains(tweakKey))
                                appliedTweaks.Add(tweakKey);
                            Debug.WriteLine($"✓ Применён твик: {tweakKey}");
                        }
                        catch (UnauthorizedAccessException)
                        {
                            failedTweaks.Add($"{tweakKey} (требуются права администратора)");
                        }
                        catch (Exception ex)
                        {
                            failedTweaks.Add($"{tweakKey} ({ex.Message})");
                        }
                    }
                }
            });

            SaveAppliedTweaksState();
            LogResults();
        }

        public async Task ApplyAllTweaksAsync()
        {
            failedTweaks.Clear();

            await Task.Run(() =>
            {
                foreach (var tweakKey in enabledTweaks)
                {
                    if (tweakActions.ContainsKey(tweakKey))
                    {
                        try
                        {
                            tweakActions[tweakKey].Apply();
                            if (!appliedTweaks.Contains(tweakKey))
                                appliedTweaks.Add(tweakKey);
                            Debug.WriteLine($"✓ Успешно применен твик: {tweakKey}");
                        }
                        catch (UnauthorizedAccessException)
                        {
                            failedTweaks.Add($"{tweakKey} (требуются права администратора)");
                        }
                        catch (Exception ex)
                        {
                            failedTweaks.Add($"{tweakKey} ({ex.Message})");
                        }
                    }
                }
            });

            SaveAppliedTweaksState();
            LogResults();
        }

        public async Task ApplySelectedTweakAsync(string tweakKey)
        {
            await Task.Run(() =>
            {
                if (tweakActions.ContainsKey(tweakKey))
                {
                    try
                    {
                        tweakActions[tweakKey].Apply();
                        if (!appliedTweaks.Contains(tweakKey))
                            appliedTweaks.Add(tweakKey);
                        SaveAppliedTweaksState();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"✗ Ошибка применения твика {tweakKey}: {ex.Message}");
                        throw;
                    }
                }
            });
        }

        public async Task RevertAllTweaksAsync()
        {
            failedTweaks.Clear();

            await Task.Run(() =>
            {
                foreach (var tweakKey in appliedTweaks.ToList())
                {
                    if (tweakActions.ContainsKey(tweakKey))
                    {
                        try
                        {
                            tweakActions[tweakKey].Revert();
                            Debug.WriteLine($"✓ Успешно отменен твик: {tweakKey}");
                        }
                        catch (UnauthorizedAccessException)
                        {
                            failedTweaks.Add($"{tweakKey} (требуются права администратора)");
                        }
                        catch (Exception ex)
                        {
                            failedTweaks.Add($"{tweakKey} ({ex.Message})");
                        }
                    }
                }

                appliedTweaks.Clear();
                enabledTweaks.Clear();
            });

            SaveAppliedTweaksState();
            LogResults();
        }

        public async Task RevertSelectedTweaksAsync(IEnumerable<string> tweaksToRevert)
        {
            failedTweaks.Clear();

            await Task.Run(() =>
            {
                foreach (var tweakKey in tweaksToRevert)
                {
                    if (tweakActions.ContainsKey(tweakKey))
                    {
                        try
                        {
                            tweakActions[tweakKey].Revert();
                            appliedTweaks.Remove(tweakKey);
                            Debug.WriteLine($"✓ Успешно отменен твик: {tweakKey}");
                        }
                        catch (Exception ex)
                        {
                            failedTweaks.Add($"{tweakKey} ({ex.Message})");
                        }
                    }
                }
            });

            SaveAppliedTweaksState();
            LogResults();
        }

        public async Task RevertSelectedTweakAsync(string tweakKey)
        {
            await Task.Run(() =>
            {
                if (tweakActions.ContainsKey(tweakKey))
                {
                    try
                    {
                        tweakActions[tweakKey].Revert();
                        appliedTweaks.Remove(tweakKey);
                        SaveAppliedTweaksState();
                        Debug.WriteLine($"✓ Успешно отменен твик: {tweakKey}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"✗ Ошибка отмены твика {tweakKey}: {ex.Message}");
                        throw;
                    }
                }
            });
        }

        private void LoadAppliedTweaksState()
        {
            try
            {
                if (File.Exists(statePath))
                {
                    var lines = File.ReadAllLines(statePath);
                    appliedTweaks.Clear();
                    appliedTweaks.AddRange(lines.Where(l => !string.IsNullOrWhiteSpace(l)));
                }
            }
            catch { }
        }

        private void SaveAppliedTweaksState()
        {
            try
            {
                string directory = Path.GetDirectoryName(statePath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                File.WriteAllLines(statePath, appliedTweaks);
            }
            catch { }
        }

        public string GetApplyResults()
        {
            var result = new System.Text.StringBuilder();
            result.AppendLine("═══════════════════════════════════════════");
            result.AppendLine("РЕЗУЛЬТАТЫ ПРИМЕНЕНИЯ ТВИКОВ");
            result.AppendLine("═══════════════════════════════════════════");
            result.AppendLine();

            if (appliedTweaks.Count > 0)
            {
                result.AppendLine($"✅ Успешно применено: {appliedTweaks.Count}");
                foreach (var tweak in appliedTweaks)
                {
                    if (tweakActions.ContainsKey(tweak))
                        result.AppendLine($"   • {tweakActions[tweak].Description}");
                }
                result.AppendLine();
            }

            if (failedTweaks.Count > 0)
            {
                result.AppendLine($"❌ Ошибки: {failedTweaks.Count}");
                foreach (var tweak in failedTweaks)
                    result.AppendLine($"   • {tweak}");
                result.AppendLine();
            }

            result.AppendLine("═══════════════════════════════════════════");
            return result.ToString();
        }

        private void LogResults()
        {
            try
            {
                string logPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "WindowsTweaks", "Logs");
                Directory.CreateDirectory(logPath);
                string logFile = Path.Combine(logPath, $"tweaks_{DateTime.Now:yyyyMMdd_HHmmss}.log");
                File.WriteAllText(logFile, GetApplyResults());
            }
            catch { }
        }

        public void CreateRestorePoint(string description)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"Checkpoint-Computer -Description '{description}' -RestorePointType 'MODIFY_SETTINGS'\"",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                var process = Process.Start(psi);
                process?.WaitForExit(60000);

                if (process?.ExitCode != 0)
                    throw new Exception("Не удалось создать точку восстановления");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка создания точки восстановления: {ex.Message}");
            }
        }

        private Dictionary<string, TweakAction> InitializeTweakActions()
        {
            return new Dictionary<string, TweakAction>
            {
                // ═══════════════════════════════════════════════
                // ПРОИЗВОДИТЕЛЬНОСТЬ
                // ═══════════════════════════════════════════════

                ["DisableVisualEffects"] = new TweakAction(
                    "Отключение визуальных эффектов Windows",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting", 2, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting", 0, RegistryValueKind.DWord)
                ),

                ["DisableSearchIndexing"] = new TweakAction(
                    "Отключение службы индексирования поиска",
                    () => {
                        ExecuteCommand("sc stop WSearch");
                        ExecuteCommand("sc config WSearch start=disabled");
                    },
                    () => {
                        ExecuteCommand("sc config WSearch start=auto");
                        ExecuteCommand("sc start WSearch");
                    }
                ),

                ["DisableSuperfetch"] = new TweakAction(
                    "Отключение службы SuperFetch/SysMain",
                    () => {
                        ExecuteCommand("sc stop SysMain");
                        ExecuteCommand("sc config SysMain start=disabled");
                    },
                    () => {
                        ExecuteCommand("sc config SysMain start=auto");
                        ExecuteCommand("sc start SysMain");
                    }
                ),

                ["OptimizePageFile"] = new TweakAction(
                    "Оптимизация размера файла подкачки",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "PagingFiles", new string[] { "C:\\pagefile.sys 2048 4096" }, RegistryValueKind.MultiString),
                    () => DeleteRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "PagingFiles")
                ),

                ["DisableHibernation"] = new TweakAction(
                    "Отключение режима гибернации (удаление hiberfil.sys)",
                    () => ExecuteCommand("powercfg -h off"),
                    () => ExecuteCommand("powercfg -h on")
                ),

                ["DisableScheduledDefrag"] = new TweakAction(
                    "Отключение дефрагментации по расписанию",
                    () => ExecuteCommand("schtasks /Change /TN \"\\Microsoft\\Windows\\Defrag\\ScheduledDefrag\" /DISABLE"),
                    () => ExecuteCommand("schtasks /Change /TN \"\\Microsoft\\Windows\\Defrag\\ScheduledDefrag\" /ENABLE")
                ),

                ["IncreaseDNSCache"] = new TweakAction(
                    "Увеличение кэша DNS",
                    () => {
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters", "CacheHashTableBucketSize", 1, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters", "CacheHashTableSize", 384, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters", "MaxCacheEntryTtlLimit", 86400, RegistryValueKind.DWord);
                    },
                    () => {
                        DeleteRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters", "CacheHashTableBucketSize");
                        DeleteRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters", "CacheHashTableSize");
                        DeleteRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters", "MaxCacheEntryTtlLimit");
                    }
                ),

                ["DisableDefender"] = new TweakAction(
                    "Отключение Windows Defender",
                    () => {
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", 1, RegistryValueKind.DWord);
                        ExecuteCommand("sc stop WinDefend");
                        ExecuteCommand("sc config WinDefend start=disabled");
                    },
                    () => {
                        DeleteRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware");
                        ExecuteCommand("sc config WinDefend start=auto");
                        ExecuteCommand("sc start WinDefend");
                    }
                ),

                ["DisableStartupDelay"] = new TweakAction(
                    "Отключить задержку запуска программ при старте",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Serialize", "StartupDelayInMSec", 0, RegistryValueKind.DWord),
                    () => DeleteRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Serialize", "StartupDelayInMSec")
                ),

                ["DisableBackgroundApps"] = new TweakAction(
                    "Отключить фоновые приложения",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", "GlobalUserDisabled", 1, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", "GlobalUserDisabled", 0, RegistryValueKind.DWord)
                ),

                ["DisableGameBar"] = new TweakAction(
                    "Отключить Xbox Game Bar",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\GameDVR", "AppCaptureEnabled", 0, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\GameDVR", "AppCaptureEnabled", 1, RegistryValueKind.DWord)
                ),

                ["DisableTransparency"] = new TweakAction(
                    "Отключить прозрачность интерфейса",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "EnableTransparency", 0, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "EnableTransparency", 1, RegistryValueKind.DWord)
                ),

                // НОВЫЕ ТВИКИ ПРОИЗВОДИТЕЛЬНОСТИ

                ["DelayedServicesStart"] = new TweakAction(
                    "Отложенный запуск служб для ускорения старта",
                    () => {
                        // Переводим ряд автоматических служб в режим отложенного запуска
                        ExecuteCommand("sc config DiagTrack start=delayed-auto");
                        ExecuteCommand("sc config wuauserv start=delayed-auto");
                        ExecuteCommand("sc config WSearch start=delayed-auto");
                        ExecuteCommand("sc config BITS start=delayed-auto");
                    },
                    () => {
                        ExecuteCommand("sc config DiagTrack start=auto");
                        ExecuteCommand("sc config wuauserv start=auto");
                        ExecuteCommand("sc config WSearch start=auto");
                        ExecuteCommand("sc config BITS start=auto");
                    }
                ),

                ["DisableReservedStorage"] = new TweakAction(
                    "Отключение зарезервированного хранилища Windows",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\ReserveManager", "ShippedWithReserves", 0, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\ReserveManager", "ShippedWithReserves", 1, RegistryValueKind.DWord)
                ),

                ["EnableUltimatePowerPlan"] = new TweakAction(
                    "Включение скрытой схемы питания (максимальная производительность)",
                    () => ExecuteCommand("powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61"),
                    () => { /* Схема остаётся, пользователь может переключиться вручную */ }
                ),

                ["DisableScreenOff"] = new TweakAction(
                    "Отключение автоотключения экрана (питание от сети)",
                    () => ExecuteCommand("powercfg -change -monitor-timeout-ac 0"),
                    () => ExecuteCommand("powercfg -change -monitor-timeout-ac 10")
                ),

                ["DisableDiskSleep"] = new TweakAction(
                    "Отключение автоотключения дисков",
                    () => ExecuteCommand("powercfg -change -disk-timeout-ac 0"),
                    () => ExecuteCommand("powercfg -change -disk-timeout-ac 20")
                ),

                ["IncreaseThumbnailCache"] = new TweakAction(
                    "Увеличение кэша превью изображений",
                    () => {
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", "ThumbCacheSizeMB", 1024, RegistryValueKind.DWord);
                    },
                    () => {
                        DeleteRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", "ThumbCacheSizeMB");
                    }
                ),

                ["MoveTempFolder"] = new TweakAction(
                    "Перенос папки Temp в корень диска C:\\Temp",
                    () => {
                        // Создаём папку и меняем переменные окружения
                        ExecuteCommand("if not exist C:\\Temp mkdir C:\\Temp");
                        ExecuteCommand("setx TEMP C:\\Temp /M");
                        ExecuteCommand("setx TMP C:\\Temp /M");
                    },
                    () => {
                        string defaultTemp = "%SystemRoot%\\Temp";
                        ExecuteCommand($"setx TEMP {defaultTemp} /M");
                        ExecuteCommand($"setx TMP {defaultTemp} /M");
                    }
                ),

                // ═══════════════════════════════════════════════
                // КОНФИДЕНЦИАЛЬНОСТЬ
                // ═══════════════════════════════════════════════

                ["DisableTelemetry"] = new TweakAction(
                    "Отключение телеметрии Windows",
                    () => {
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
                        ExecuteCommand("sc stop DiagTrack");
                        ExecuteCommand("sc config DiagTrack start=disabled");
                    },
                    () => {
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 3, RegistryValueKind.DWord);
                        ExecuteCommand("sc config DiagTrack start=auto");
                        ExecuteCommand("sc start DiagTrack");
                    }
                ),

                ["DisableStartMenuAds"] = new TweakAction(
                    "Отключение рекламы в меню Пуск",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SystemPaneSuggestionsEnabled", 0, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SystemPaneSuggestionsEnabled", 1, RegistryValueKind.DWord)
                ),

                ["DisableCortana"] = new TweakAction(
                    "Отключение Cortana",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 0, RegistryValueKind.DWord),
                    () => DeleteRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana")
                ),

                ["DisableLocationTracking"] = new TweakAction(
                    "Отключение отслеживания местоположения",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\location", "Value", "Deny", RegistryValueKind.String),
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\location", "Value", "Allow", RegistryValueKind.String)
                ),

                ["DisableWindowsTips"] = new TweakAction(
                    "Отключение советов Windows",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SubscribedContent-338389Enabled", 0, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SubscribedContent-338389Enabled", 1, RegistryValueKind.DWord)
                ),

                ["DisableAdvertisingID"] = new TweakAction(
                    "Отключение рекламного ID",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled", 0, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled", 1, RegistryValueKind.DWord)
                ),

                ["BlockDiagnosticData"] = new TweakAction(
                    "Блокировка сбора диагностических данных",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry", 3, RegistryValueKind.DWord)
                ),

                ["DisableCloudSync"] = new TweakAction(
                    "Отключение облачной синхронизации",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\SettingSync", "SyncPolicy", 5, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\SettingSync", "SyncPolicy", 1, RegistryValueKind.DWord)
                ),

                ["DisableActivityHistory"] = new TweakAction(
                    "Отключить историю действий",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\System", "PublishUserActivities", 0, RegistryValueKind.DWord),
                    () => DeleteRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\System", "PublishUserActivities")
                ),

                ["DisableWebSearch"] = new TweakAction(
                    "Отключить веб-поиск в меню Пуск",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\Explorer", "DisableSearchBoxSuggestions", 1, RegistryValueKind.DWord),
                    () => DeleteRegistryValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\Explorer", "DisableSearchBoxSuggestions")
                ),

                ["DisableAppSuggestions"] = new TweakAction(
                    "Отключить предложения приложений",
                    () => {
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SilentInstalledAppsEnabled", 0, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "PreInstalledAppsEnabled", 0, RegistryValueKind.DWord);
                    },
                    () => {
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SilentInstalledAppsEnabled", 1, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "PreInstalledAppsEnabled", 1, RegistryValueKind.DWord);
                    }
                ),

                // НОВЫЕ ТВИКИ КОНФИДЕНЦИАЛЬНОСТИ

                ["DisableGameModeNotifications"] = new TweakAction(
                    "Отключение уведомлений игрового режима",
                    () => {
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\GameBar", "ShowStartupPanel", 0, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\GameBar", "GamePanelStartupTipIndex", 3, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\GameBar", "AllowAutoGameMode", 0, RegistryValueKind.DWord);
                    },
                    () => {
                        DeleteRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\GameBar", "ShowStartupPanel");
                        DeleteRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\GameBar", "GamePanelStartupTipIndex");
                        DeleteRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\GameBar", "AllowAutoGameMode");
                    }
                ),

                ["MinimizeSystemReports"] = new TweakAction(
                    "Минимизация системных отчётов (снижение нагрузки на диск)",
                    () => {
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Windows Error Reporting", "Disabled", 1, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Windows Error Reporting", "DontSendAdditionalData", 1, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Windows Error Reporting\Consent", "DefaultConsent", 0, RegistryValueKind.DWord);
                    },
                    () => {
                        DeleteRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Windows Error Reporting", "Disabled");
                        DeleteRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Windows Error Reporting", "DontSendAdditionalData");
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Windows Error Reporting\Consent", "DefaultConsent", 1, RegistryValueKind.DWord);
                    }
                ),

                ["DisableAudioDucking"] = new TweakAction(
                    "Отключение автоприглушения звука при использовании микрофона",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Multimedia\Audio", "UserDuckingPreference", 3, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Multimedia\Audio", "UserDuckingPreference", 0, RegistryValueKind.DWord)
                ),

                // ═══════════════════════════════════════════════
                // СЕТЬ
                // ═══════════════════════════════════════════════

                ["DisableIPv6"] = new TweakAction(
                    "Отключение IPv6",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters", "DisabledComponents", 0xFF, RegistryValueKind.DWord),
                    () => DeleteRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters", "DisabledComponents")
                ),

                ["OptimizeTCPIP"] = new TweakAction(
                    "Оптимизация TCP/IP",
                    () => {
                        ExecuteCommand("netsh int tcp set global autotuninglevel=normal");
                        ExecuteCommand("netsh int tcp set global chimney=enabled");
                        ExecuteCommand("netsh int tcp set global dca=enabled");
                        ExecuteCommand("netsh int tcp set global netdma=enabled");
                    },
                    () => {
                        ExecuteCommand("netsh int tcp set global autotuninglevel=normal");
                        ExecuteCommand("netsh int tcp set global chimney=disabled");
                        ExecuteCommand("netsh int tcp set global dca=disabled");
                        ExecuteCommand("netsh int tcp set global netdma=disabled");
                    }
                ),

                ["FlushDNSCache"] = new TweakAction(
                    "Очистка кэша DNS",
                    () => ExecuteCommand("ipconfig /flushdns"),
                    () => { }
                ),

                ["ResetNetworkAdapters"] = new TweakAction(
                    "Сброс сетевых адаптеров",
                    () => {
                        ExecuteCommand("netsh winsock reset");
                        ExecuteCommand("netsh int ip reset");
                    },
                    () => { }
                ),

                ["DisableMeteredConnection"] = new TweakAction(
                    "Отключение лимитированного подключения",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\NetworkList\DefaultMediaCost", "Ethernet", 1, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\NetworkList\DefaultMediaCost", "Ethernet", 2, RegistryValueKind.DWord)
                ),

                ["OptimizeQoS"] = new TweakAction(
                    "Оптимизация настроек QoS",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Psched", "NonBestEffortLimit", 0, RegistryValueKind.DWord),
                    () => DeleteRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Psched", "NonBestEffortLimit")
                ),

                ["DisableNetBIOS"] = new TweakAction(
                    "Отключить NetBIOS через TCP/IP (безопасность)",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\NetBT\Parameters", "NodeType", 2, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\NetBT\Parameters", "NodeType", 1, RegistryValueKind.DWord)
                ),

                ["DisableLLMNR"] = new TweakAction(
                    "Отключить LLMNR (безопасность)",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows NT\DNSClient", "EnableMulticast", 0, RegistryValueKind.DWord),
                    () => DeleteRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows NT\DNSClient", "EnableMulticast")
                ),

                ["OptimizeMTU"] = new TweakAction(
                    "Оптимизировать MTU для лучшей производительности",
                    () => ExecuteCommand("netsh interface ipv4 set subinterface \"Ethernet\" mtu=1500 store=persistent"),
                    () => ExecuteCommand("netsh interface ipv4 set subinterface \"Ethernet\" mtu=1500 store=persistent")
                ),

                // ═══════════════════════════════════════════════
                // ВНЕШНИЙ ВИД
                // ═══════════════════════════════════════════════

                ["EnableDarkTheme"] = new TweakAction(
                    "Включение темной темы Windows",
                    () => {
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 0, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 0, RegistryValueKind.DWord);
                        try { ExecuteCommand("taskkill /f /im explorer.exe & start explorer.exe"); } catch { }
                    },
                    () => {
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 1, RegistryValueKind.DWord);
                        try { ExecuteCommand("taskkill /f /im explorer.exe & start explorer.exe"); } catch { }
                    }
                ),

                ["ShowFileExtensions"] = new TweakAction(
                    "Показывать расширения файлов",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt", 0, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt", 1, RegistryValueKind.DWord)
                ),

                ["ShowHiddenFiles"] = new TweakAction(
                    "Показывать скрытые файлы и папки",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "Hidden", 1, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "Hidden", 2, RegistryValueKind.DWord)
                ),

                ["ClassicContextMenu"] = new TweakAction(
                    "Классическое контекстное меню (Windows 11)",
                    () => {
                        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32"))
                        {
                            key?.SetValue("", "", RegistryValueKind.String);
                        }
                    },
                    () => {
                        try { Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}"); }
                        catch { }
                    }
                ),

                ["DisableTaskbarGrouping"] = new TweakAction(
                    "Отключение группировки кнопок на панели задач",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarGlomLevel", 2, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarGlomLevel", 0, RegistryValueKind.DWord)
                ),

                ["SmallTaskbarIcons"] = new TweakAction(
                    "Использовать мелкие значки на панели задач",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarSmallIcons", 1, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarSmallIcons", 0, RegistryValueKind.DWord)
                ),

                ["RemoveTaskbarWidgets"] = new TweakAction(
                    "Убрать виджеты с панели задач (Windows 11)",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarDa", 0, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarDa", 1, RegistryValueKind.DWord)
                ),

                ["ShowFullPath"] = new TweakAction(
                    "Показывать полный путь в заголовке Проводника",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\CabinetState", "FullPath", 1, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\CabinetState", "FullPath", 0, RegistryValueKind.DWord)
                ),

                ["DisableShakeToMinimize"] = new TweakAction(
                    "Отключить встряхивание окна для сворачивания",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "DisallowShaking", 1, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "DisallowShaking", 0, RegistryValueKind.DWord)
                ),

                ["EnableSecondsInClock"] = new TweakAction(
                    "Показывать секунды в системных часах",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSecondsInSystemClock", 1, RegistryValueKind.DWord),
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSecondsInSystemClock", 0, RegistryValueKind.DWord)
                ),

                ["DisableLockScreen"] = new TweakAction(
                    "Отключить экран блокировки",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Personalization", "NoLockScreen", 1, RegistryValueKind.DWord),
                    () => DeleteRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Personalization", "NoLockScreen")
                ),

                // НОВЫЕ ТВИКИ ВНЕШНЕГО ВИДА

                ["DisableWallpaperCompression"] = new TweakAction(
                    "Отключение сжатия обоев (сохранение исходного качества)",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "JPEGImportQuality", 100, RegistryValueKind.DWord),
                    () => DeleteRegistryValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "JPEGImportQuality")
                ),

                ["DisableStickyKeys"] = new TweakAction(
                    "Отключение залипания клавиш",
                    () => {
                        SetRegistryValue(@"HKEY_CURRENT_USER\Control Panel\Accessibility\StickyKeys", "Flags", "506", RegistryValueKind.String);
                        SetRegistryValue(@"HKEY_CURRENT_USER\Control Panel\Accessibility\Keyboard Response", "Flags", "122", RegistryValueKind.String);
                        SetRegistryValue(@"HKEY_CURRENT_USER\Control Panel\Accessibility\ToggleKeys", "Flags", "58", RegistryValueKind.String);
                    },
                    () => {
                        SetRegistryValue(@"HKEY_CURRENT_USER\Control Panel\Accessibility\StickyKeys", "Flags", "510", RegistryValueKind.String);
                        SetRegistryValue(@"HKEY_CURRENT_USER\Control Panel\Accessibility\Keyboard Response", "Flags", "126", RegistryValueKind.String);
                        SetRegistryValue(@"HKEY_CURRENT_USER\Control Panel\Accessibility\ToggleKeys", "Flags", "62", RegistryValueKind.String);
                    }
                ),

                // ═══════════════════════════════════════════════
                // СЛУЖБЫ WINDOWS
                // ═══════════════════════════════════════════════

                ["DisableWindowsUpdate"] = new TweakAction(
                    "Отключение Windows Update (осторожно!)",
                    () => {
                        ExecuteCommand("sc stop wuauserv");
                        ExecuteCommand("sc config wuauserv start=disabled");
                    },
                    () => {
                        ExecuteCommand("sc config wuauserv start=auto");
                        ExecuteCommand("sc start wuauserv");
                    }
                ),

                ["DisableWindowsSearch"] = new TweakAction(
                    "Отключение службы Windows Search",
                    () => {
                        ExecuteCommand("sc stop WSearch");
                        ExecuteCommand("sc config WSearch start=disabled");
                    },
                    () => {
                        ExecuteCommand("sc config WSearch start=auto");
                        ExecuteCommand("sc start WSearch");
                    }
                ),

                ["DisablePrintSpooler"] = new TweakAction(
                    "Отключение службы печати (Print Spooler)",
                    () => {
                        ExecuteCommand("sc stop Spooler");
                        ExecuteCommand("sc config Spooler start=disabled");
                    },
                    () => {
                        ExecuteCommand("sc config Spooler start=auto");
                        ExecuteCommand("sc start Spooler");
                    }
                ),

                ["DisableFax"] = new TweakAction(
                    "Отключение службы факса",
                    () => {
                        ExecuteCommand("sc stop Fax");
                        ExecuteCommand("sc config Fax start=disabled");
                    },
                    () => {
                        ExecuteCommand("sc config Fax start=demand");
                    }
                ),

                ["DisableBluetooth"] = new TweakAction(
                    "Отключение службы Bluetooth",
                    () => {
                        ExecuteCommand("sc stop bthserv");
                        ExecuteCommand("sc config bthserv start=disabled");
                    },
                    () => {
                        ExecuteCommand("sc config bthserv start=demand");
                        ExecuteCommand("sc start bthserv");
                    }
                ),

                ["DisableDiagnostic"] = new TweakAction(
                    "Отключение служб диагностики",
                    () => {
                        ExecuteCommand("sc stop DiagTrack");
                        ExecuteCommand("sc config DiagTrack start=disabled");
                        ExecuteCommand("sc stop diagnosticshub.standardcollector.service");
                        ExecuteCommand("sc config diagnosticshub.standardcollector.service start=disabled");
                    },
                    () => {
                        ExecuteCommand("sc config DiagTrack start=auto");
                        ExecuteCommand("sc start DiagTrack");
                        ExecuteCommand("sc config diagnosticshub.standardcollector.service start=demand");
                    }
                ),

                ["DisableRemoteRegistry"] = new TweakAction(
                    "Отключить службу удаленного реестра",
                    () => {
                        ExecuteCommand("sc stop RemoteRegistry");
                        ExecuteCommand("sc config RemoteRegistry start=disabled");
                    },
                    () => {
                        ExecuteCommand("sc config RemoteRegistry start=demand");
                    }
                ),

                ["DisableHomeGroup"] = new TweakAction(
                    "Отключить службы домашней группы",
                    () => {
                        ExecuteCommand("sc stop HomeGroupListener");
                        ExecuteCommand("sc config HomeGroupListener start=disabled");
                        ExecuteCommand("sc stop HomeGroupProvider");
                        ExecuteCommand("sc config HomeGroupProvider start=disabled");
                    },
                    () => {
                        ExecuteCommand("sc config HomeGroupListener start=demand");
                        ExecuteCommand("sc config HomeGroupProvider start=demand");
                    }
                ),

                ["DisableErrorReporting"] = new TweakAction(
                    "Отключить службу отчетов об ошибках Windows",
                    () => {
                        ExecuteCommand("sc stop WerSvc");
                        ExecuteCommand("sc config WerSvc start=disabled");
                    },
                    () => {
                        ExecuteCommand("sc config WerSvc start=demand");
                    }
                ),

                // НОВЫЙ ТВИК СЛУЖБ — уже частично покрывается DelayedServicesStart
                // Восстановление запуска командной строки из папки — в секции Администрирование (через реестр)

                ["RestoreCmdHereContext"] = new TweakAction(
                    "Восстановление запуска командной строки из папки",
                    () => {
                        // Возвращает «Открыть окно команд здесь» в контекстное меню
                        // Используем команду reg для работы с HKCR с правами администратора
                        ExecuteCommand("reg delete \"HKEY_CLASSES_ROOT\\Directory\\shell\\cmd\" /v HideBasedOnVelocityId /f");
                        ExecuteCommand("reg delete \"HKEY_CLASSES_ROOT\\Directory\\Background\\shell\\cmd\" /v HideBasedOnVelocityId /f");
                        ExecuteCommand("reg delete \"HKEY_CLASSES_ROOT\\Directory\\shell\\cmd\" /v ShowBasedOnVelocityId /f");
                        ExecuteCommand("reg delete \"HKEY_CLASSES_ROOT\\Directory\\Background\\shell\\cmd\" /v ShowBasedOnVelocityId /f");
                    },
                    () => {
                        // Скрывает обратно пункт меню
                        ExecuteCommand("reg add \"HKEY_CLASSES_ROOT\\Directory\\shell\\cmd\" /v HideBasedOnVelocityId /t REG_DWORD /d 6604448 /f");
                        ExecuteCommand("reg add \"HKEY_CLASSES_ROOT\\Directory\\Background\\shell\\cmd\" /v HideBasedOnVelocityId /t REG_DWORD /d 6604448 /f");
                    }
                ),
            };
        }

        private void SetRegistryValue(string keyPath, string valueName, object value, RegistryValueKind valueKind)
        {
            try
            {
                string[] parts = keyPath.Split('\\');
                RegistryKey baseKey = parts[0] switch
                {
                    "HKEY_LOCAL_MACHINE" => Registry.LocalMachine,
                    "HKEY_CURRENT_USER" => Registry.CurrentUser,
                    "HKEY_CLASSES_ROOT" => Registry.ClassesRoot,
                    "HKEY_USERS" => Registry.Users,
                    "HKEY_CURRENT_CONFIG" => Registry.CurrentConfig,
                    _ => throw new ArgumentException($"Неверный корневой ключ реестра: {parts[0]}")
                };

                string subKeyPath = string.Join("\\", parts.Skip(1));

                using (RegistryKey key = baseKey.CreateSubKey(subKeyPath, true))
                {
                    if (key == null)
                        throw new Exception($"Не удалось создать или открыть ключ: {subKeyPath}");
                    key.SetValue(valueName, value, valueKind);
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException($"Недостаточно прав для записи в реестр: {keyPath}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка записи в реестр ({keyPath}\\{valueName}): {ex.Message}");
            }
        }

        private void DeleteRegistryValue(string keyPath, string valueName)
        {
            try
            {
                string[] parts = keyPath.Split('\\');
                RegistryKey baseKey = parts[0] switch
                {
                    "HKEY_LOCAL_MACHINE" => Registry.LocalMachine,
                    "HKEY_CURRENT_USER" => Registry.CurrentUser,
                    "HKEY_CLASSES_ROOT" => Registry.ClassesRoot,
                    "HKEY_USERS" => Registry.Users,
                    "HKEY_CURRENT_CONFIG" => Registry.CurrentConfig,
                    _ => throw new ArgumentException($"Неверный корневой ключ реестра: {parts[0]}")
                };

                string subKeyPath = string.Join("\\", parts.Skip(1));

                using (RegistryKey key = baseKey.OpenSubKey(subKeyPath, true))
                {
                    if (key != null)
                    {
                        try { key.DeleteValue(valueName, false); }
                        catch { }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException($"Недостаточно прав для удаления значения в реестре: {keyPath}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка удаления значения в реестре ({keyPath}\\{valueName}): {ex.Message}");
            }
        }

        private void ExecuteCommand(string command)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    // Важно: запускаем с повышением прав для команд реестра
                    Verb = command.Contains("reg ") || command.Contains("sc ") || 
                           command.Contains("powercfg") || command.Contains("netsh") 
                           ? "runas" : ""
                };

                var process = Process.Start(psi);

                if (process != null)
                {
                    bool finished = process.WaitForExit(30000);
                    if (!finished)
                    {
                        process.Kill();
                        throw new TimeoutException($"Команда выполнялась слишком долго: {command}");
                    }
                }
            }
            catch (System.ComponentModel.Win32Exception ex) when (ex.NativeErrorCode == 1223)
            {
                throw new OperationCanceledException("Операция отменена пользователем (UAC)");
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException($"Недостаточно прав для выполнения команды: {command}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка выполнения команды ({command}): {ex.Message}");
            }
        }

        private class TweakAction
        {
            public string Description { get; }
            public Action Apply { get; }
            public Action Revert { get; }

            public TweakAction(string description, Action apply, Action revert)
            {
                Description = description;
                Apply = apply;
                Revert = revert;
            }
        }
    }
}
