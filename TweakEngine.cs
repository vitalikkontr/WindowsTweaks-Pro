using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace WindowsTweaks
{
    /// <summary>
    /// Движок для применения системных твиков Windows
    /// Версия 2.0 - Улучшенная обработка ошибок и логирование
    /// </summary>
    public class TweakEngine
    {
        private readonly HashSet<string> enabledTweaks;
        private readonly Dictionary<string, TweakAction> tweakActions;
        private readonly List<string> appliedTweaks;
        private readonly List<string> failedTweaks;

        public TweakEngine()
        {
            enabledTweaks = new HashSet<string>();
            appliedTweaks = new List<string>();
            failedTweaks = new List<string>();
            tweakActions = InitializeTweakActions();
        }

        public void EnableTweak(string tweakKey)
        {
            enabledTweaks.Add(tweakKey);
        }

        public void DisableTweak(string tweakKey)
        {
            enabledTweaks.Remove(tweakKey);
        }

        public async Task ApplyAllTweaksAsync()
        {
            appliedTweaks.Clear();
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
                            appliedTweaks.Add(tweakKey);
                            Debug.WriteLine($"✓ Успешно применен твик: {tweakKey}");
                        }
                        catch (UnauthorizedAccessException)
                        {
                            failedTweaks.Add($"{tweakKey} (требуются права администратора)");
                            Debug.WriteLine($"✗ Недостаточно прав для твика: {tweakKey}");
                        }
                        catch (Exception ex)
                        {
                            failedTweaks.Add($"{tweakKey} ({ex.Message})");
                            Debug.WriteLine($"✗ Ошибка применения твика {tweakKey}: {ex.Message}");
                        }
                    }
                }
            });

            // Логируем результаты
            LogResults();
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
                    result.AppendLine($"   • {tweakActions[tweak].Description}");
                }
                result.AppendLine();
            }

            if (failedTweaks.Count > 0)
            {
                result.AppendLine($"❌ Ошибки: {failedTweaks.Count}");
                foreach (var tweak in failedTweaks)
                {
                    result.AppendLine($"   • {tweak}");
                }
                result.AppendLine();
            }

            result.AppendLine("═══════════════════════════════════════════");
            return result.ToString();
        }

        private void LogResults()
        {
            try
            {
                string logPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "WindowsTweaks",
                    "Logs"
                );
                System.IO.Directory.CreateDirectory(logPath);

                string logFile = System.IO.Path.Combine(
                    logPath,
                    $"tweaks_{DateTime.Now:yyyyMMdd_HHmmss}.log"
                );

                System.IO.File.WriteAllText(logFile, GetApplyResults());
            }
            catch
            {
                // Игнорируем ошибки логирования
            }
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
                process?.WaitForExit(60000); // Таймаут 60 секунд

                if (process?.ExitCode != 0)
                {
                    throw new Exception("Не удалось создать точку восстановления");
                }
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
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting", 2, RegistryValueKind.DWord)
                ),

                ["DisableSearchIndexing"] = new TweakAction(
                    "Отключение службы индексирования поиска",
                    () => {
                        ExecuteCommand("sc stop WSearch");
                        ExecuteCommand("sc config WSearch start=disabled");
                    }
                ),

                ["DisableSuperfetch"] = new TweakAction(
                    "Отключение службы SuperFetch/SysMain",
                    () => {
                        ExecuteCommand("sc stop SysMain");
                        ExecuteCommand("sc config SysMain start=disabled");
                    }
                ),

                ["OptimizePageFile"] = new TweakAction(
                    "Оптимизация размера файла подкачки",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "PagingFiles", new string[] { "C:\\pagefile.sys 2048 4096" }, RegistryValueKind.MultiString)
                ),

                ["DisableHibernation"] = new TweakAction(
                    "Отключение режима гибернации (удаление hiberfil.sys)",
                    () => ExecuteCommand("powercfg -h off")
                ),

                ["DisableScheduledDefrag"] = new TweakAction(
                    "Отключение дефрагментации по расписанию",
                    () => ExecuteCommand("schtasks /Change /TN \"\\Microsoft\\Windows\\Defrag\\ScheduledDefrag\" /DISABLE")
                ),

                ["IncreaseDNSCache"] = new TweakAction(
                    "Увеличение размера кэша DNS",
                    () => {
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters", "CacheHashTableBucketSize", 1, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters", "CacheHashTableSize", 384, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters", "MaxCacheEntryTtlLimit", 64000, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters", "MaxSOACacheEntryTtlLimit", 301, RegistryValueKind.DWord);
                    }
                ),

                ["DisableDefender"] = new TweakAction(
                    "Отключение Windows Defender (требует осторожности!)",
                    () => {
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", 1, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableRealtimeMonitoring", 1, RegistryValueKind.DWord);
                    }
                ),

                // ═══════════════════════════════════════════════
                // КОНФИДЕНЦИАЛЬНОСТЬ
                // ═══════════════════════════════════════════════

                ["DisableTelemetry"] = new TweakAction(
                    "Отключение телеметрии Windows",
                    () => {
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
                        ExecuteCommand("sc stop DiagTrack");
                        ExecuteCommand("sc config DiagTrack start=disabled");
                        ExecuteCommand("sc stop dmwappushservice");
                        ExecuteCommand("sc config dmwappushservice start=disabled");
                    }
                ),

                ["DisableStartMenuAds"] = new TweakAction(
                    "Отключение рекламы в меню Пуск",
                    () => {
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SystemPaneSuggestionsEnabled", 0, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SubscribedContent-338388Enabled", 0, RegistryValueKind.DWord);
                    }
                ),

                ["DisableCortana"] = new TweakAction(
                    "Отключение Cortana",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 0, RegistryValueKind.DWord)
                ),

                ["DisableLocationTracking"] = new TweakAction(
                    "Отключение отслеживания местоположения",
                    () => {
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableLocation", 1, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\location", "Value", "Deny", RegistryValueKind.String);
                    }
                ),

                ["DisableWindowsTips"] = new TweakAction(
                    "Отключение советов и подсказок Windows",
                    () => {
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\CloudContent", "DisableSoftLanding", 1, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SoftLandingEnabled", 0, RegistryValueKind.DWord);
                    }
                ),

                ["DisableAdvertisingID"] = new TweakAction(
                    "Отключение рекламного идентификатора",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled", 0, RegistryValueKind.DWord)
                ),

                ["BlockDiagnosticData"] = new TweakAction(
                    "Блокировка сбора диагностических данных",
                    () => {
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
                    }
                ),

                ["DisableCloudSync"] = new TweakAction(
                    "Отключение облачной синхронизации настроек",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\SettingSync", "SyncPolicy", 5, RegistryValueKind.DWord)
                ),

                // ═══════════════════════════════════════════════
                // СЕТЕВЫЕ НАСТРОЙКИ
                // ═══════════════════════════════════════════════

                ["DisableIPv6"] = new TweakAction(
                    "Отключение протокола IPv6",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters", "DisabledComponents", 0xFF, RegistryValueKind.DWord)
                ),

                ["OptimizeTCPIP"] = new TweakAction(
                    "Оптимизация параметров TCP/IP",
                    () => {
                        ExecuteCommand("netsh int tcp set global autotuninglevel=normal");
                        ExecuteCommand("netsh int tcp set global chimney=enabled");
                        ExecuteCommand("netsh int tcp set global dca=enabled");
                        ExecuteCommand("netsh int tcp set global netdma=enabled");
                        ExecuteCommand("netsh int tcp set global rss=enabled");
                    }
                ),

                ["FlushDNSCache"] = new TweakAction(
                    "Очистка кэша DNS",
                    () => ExecuteCommand("ipconfig /flushdns")
                ),

                ["ResetNetworkAdapters"] = new TweakAction(
                    "Сброс настроек сетевых адаптеров",
                    () => {
                        ExecuteCommand("netsh winsock reset");
                        ExecuteCommand("netsh int ip reset");
                        ExecuteCommand("netsh advfirewall reset");
                    }
                ),

                ["DisableMeteredConnection"] = new TweakAction(
                    "Отключение лимитированного подключения",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\NetworkList\DefaultMediaCost", "Ethernet", 1, RegistryValueKind.DWord)
                ),

                ["OptimizeQoS"] = new TweakAction(
                    "Оптимизация QoS (Quality of Service)",
                    () => SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Psched", "NonBestEffortLimit", 0, RegistryValueKind.DWord)
                ),

                // ═══════════════════════════════════════════════
                // ВНЕШНИЙ ВИД И ПЕРСОНАЛИЗАЦИЯ
                // ═══════════════════════════════════════════════

                ["EnableDarkTheme"] = new TweakAction(
                    "Включение темной темы оформления",
                    () => {
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 0, RegistryValueKind.DWord);
                        SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 0, RegistryValueKind.DWord);
                    }
                ),

                ["ShowFileExtensions"] = new TweakAction(
                    "Показывать расширения файлов",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt", 0, RegistryValueKind.DWord)
                ),

                ["ShowHiddenFiles"] = new TweakAction(
                    "Показывать скрытые файлы и папки",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "Hidden", 1, RegistryValueKind.DWord)
                ),

                ["ClassicContextMenu"] = new TweakAction(
                    "Классическое контекстное меню (Windows 11)",
                    () => {
                        // Создаем ключ с пустым значением для восстановления старого меню
                        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32"))
                        {
                            key?.SetValue("", "", RegistryValueKind.String);
                        }
                    }
                ),

                ["DisableTaskbarGrouping"] = new TweakAction(
                    "Отключение группировки кнопок на панели задач",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarGlomLevel", 2, RegistryValueKind.DWord)
                ),

                ["SmallTaskbarIcons"] = new TweakAction(
                    "Использовать мелкие значки на панели задач",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarSmallIcons", 1, RegistryValueKind.DWord)
                ),

                ["RemoveTaskbarWidgets"] = new TweakAction(
                    "Убрать виджеты с панели задач (Windows 11)",
                    () => SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarDa", 0, RegistryValueKind.DWord)
                ),

                // ═══════════════════════════════════════════════
                // УПРАВЛЕНИЕ СЛУЖБАМИ
                // ═══════════════════════════════════════════════

                ["DisableWindowsUpdate"] = new TweakAction(
                    "Отключение службы Windows Update (осторожно!)",
                    () => {
                        ExecuteCommand("sc stop wuauserv");
                        ExecuteCommand("sc config wuauserv start=disabled");
                    }
                ),

                ["DisableWindowsSearch"] = new TweakAction(
                    "Отключение службы Windows Search",
                    () => {
                        ExecuteCommand("sc stop WSearch");
                        ExecuteCommand("sc config WSearch start=disabled");
                    }
                ),

                ["DisablePrintSpooler"] = new TweakAction(
                    "Отключение службы печати (Print Spooler)",
                    () => {
                        ExecuteCommand("sc stop Spooler");
                        ExecuteCommand("sc config Spooler start=disabled");
                    }
                ),

                ["DisableFax"] = new TweakAction(
                    "Отключение службы факса",
                    () => {
                        ExecuteCommand("sc stop Fax");
                        ExecuteCommand("sc config Fax start=disabled");
                    }
                ),

                ["DisableBluetooth"] = new TweakAction(
                    "Отключение службы Bluetooth",
                    () => {
                        ExecuteCommand("sc stop bthserv");
                        ExecuteCommand("sc config bthserv start=disabled");
                    }
                ),

                ["DisableDiagnostic"] = new TweakAction(
                    "Отключение служб диагностики",
                    () => {
                        ExecuteCommand("sc stop DiagTrack");
                        ExecuteCommand("sc config DiagTrack start=disabled");
                        ExecuteCommand("sc stop diagnosticshub.standardcollector.service");
                        ExecuteCommand("sc config diagnosticshub.standardcollector.service start=disabled");
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
                    {
                        throw new Exception($"Не удалось создать или открыть ключ: {subKeyPath}");
                    }

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

        private void ExecuteCommand(string command)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                var process = Process.Start(psi);

                if (process != null)
                {
                    bool finished = process.WaitForExit(30000); // Таймаут 30 секунд

                    if (!finished)
                    {
                        process.Kill();
                        throw new TimeoutException($"Команда выполнялась слишком долго: {command}");
                    }
                }
            }
            catch (System.ComponentModel.Win32Exception ex) when (ex.NativeErrorCode == 1223)
            {
                // Пользователь отменил UAC
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

            public TweakAction(string description, Action apply)
            {
                Description = description;
                Apply = apply;
            }
        }
    }
}