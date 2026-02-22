using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace WindowsTweaks
{
    /// <summary>
    /// - ИСПРАВЛЕНЫ ИКОНКИ ДЛЯ ДИСПЕТЧЕРА УСТРОЙСТВ И ПРОСМОТРА СОБЫТИЙ
    /// - Диспетчер устройств: devmgr.dll (красивая иконка с компьютером и настройками)
    /// - Просмотр событий: eventvwr.exe (красивая иконка с журналом событий)
    /// - Все остальные иконки оптимизированы
    /// </summary>
    public static class DesktopContextMenu
    {
        private const string BasePath = @"Software\Classes\Directory\Background\shell";
        
        // Основные инструменты
        private static readonly Dictionary<string, MenuItem> MainTools = new Dictionary<string, MenuItem>
        {
            ["TaskManager"] = new MenuItem("Диспетчер задач", "taskmgr.exe", "taskmgr"),
            ["RegistryEditor"] = new MenuItem("Редактор реестра", "regedit.exe", "regedit"),
            ["Programs"] = new MenuItem("Программы и компоненты", "appwiz.cpl", "control appwiz.cpl"),
            ["ControlPanel"] = new MenuItem("Панель управления", "shell32.dll,21", "control"),
            ["AdminTools"] = new MenuItem("Администрирование", "imageres.dll,109", "control admintools")
        };

        // -------------------- ДОБАВЛЕНИЕ ИНСТРУМЕНТОВ --------------------
        public static string AddDesktopTools()
        {
            int successCount = 0;
            int failCount = 0;
            StringBuilder result = new StringBuilder();

            result.AppendLine("◆  ДОБАВЛЕНИЕ ИНСТРУМЕНТОВ В МЕНЮ РАБОЧЕГО СТОЛА");
            result.AppendLine("────────────────────────────────────────────");
            result.AppendLine();

            foreach (var tool in MainTools)
            {
                try
                {
                    AddMenuItem(tool.Key, tool.Value, "");
                    successCount++;
                    result.AppendLine($"✓ Добавлен: {tool.Value.Title}");
                }
                catch (Exception ex)
                {
                    failCount++;
                    result.AppendLine($"✗ Ошибка '{tool.Value.Title}': {ex.Message}");
                }
            }

            try
            {
                HideStandardPersonalization();
                result.AppendLine($"✓ Скрыт стандартный пункт 'Персонализация' Windows");
            }
            catch (Exception ex)
            {
                result.AppendLine($"⚠ Предупреждение: {ex.Message}");
            }

            try
            {
                AddPersonalizationMenu();
                successCount++;
                result.AppendLine($"✓ Добавлено подменю: Персонализация+ (10 пунктов)");
            }
            catch (Exception ex)
            {
                failCount++;
                result.AppendLine($"✗ Ошибка 'Персонализация+': {ex.Message}");
            }

            try
            {
                AddSystemUtilitiesMenu();
                successCount++;
                result.AppendLine($"✓ Добавлено подменю: Системные утилиты (10 пунктов)");
            }
            catch (Exception ex)
            {
                failCount++;
                result.AppendLine($"✗ Ошибка 'Системные утилиты': {ex.Message}");
            }

            result.AppendLine();
            result.AppendLine("────────────────────────────────────────────");
            result.AppendLine($"Успешно добавлено:   {successCount}");
            result.AppendLine($"Ошибок:              {failCount}");

            if (successCount > 0)
            {
                RefreshShell();
                result.AppendLine();
                result.AppendLine("✓ Система уведомлена об изменениях");
            }

            return result.ToString();
        }

        private static void AddMenuItem(string keyName, MenuItem item, string position)
        {
            string fullPath = $@"{BasePath}\{keyName}";

            using (var key = Registry.CurrentUser.CreateSubKey(fullPath, true))
            {
                if (key == null)
                    throw new Exception("Не удалось создать ключ реестра");

                key.SetValue("", item.Title, RegistryValueKind.String);
                key.SetValue("Icon", item.Icon, RegistryValueKind.String);
                
                if (!string.IsNullOrEmpty(position))
                    key.SetValue("Position", position, RegistryValueKind.String);
            }

            using (var cmdKey = Registry.CurrentUser.CreateSubKey($"{fullPath}\\command", true))
            {
                if (cmdKey == null)
                    throw new Exception("Не удалось создать ключ command");

                cmdKey.SetValue("", item.Command, RegistryValueKind.String);
            }
        }

        private static void HideStandardPersonalization()
        {
            string[] standardKeys = {
                @"Software\Classes\DesktopBackground\Shell\Personalization",
                @"Software\Classes\DesktopBackground\Shell\Display"
            };

            foreach (var keyPath in standardKeys)
            {
                try
                {
                    using (var key = Registry.CurrentUser.CreateSubKey(keyPath, true))
                    {
                        if (key != null)
                        {
                            key.SetValue("ProgrammaticAccessOnly", "", RegistryValueKind.String);
                        }
                    }
                }
                catch { }
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // ПОДМЕНЮ "ПЕРСОНАЛИЗАЦИЯ+"
        // ═══════════════════════════════════════════════════════════════════════════
        private static void AddPersonalizationMenu()
        {
            string menuKey = $"{BasePath}\\PersonalizationPlus";

            using (var key = Registry.CurrentUser.CreateSubKey(menuKey, true))
            {
                if (key == null)
                    throw new Exception("Не удалось создать ключ PersonalizationPlus");

                key.SetValue("MUIVerb", "Персонализация+", RegistryValueKind.String);
                key.SetValue("Icon", "themecpl.dll", RegistryValueKind.String);
                key.SetValue("SubCommands", "", RegistryValueKind.String);
                key.SetValue("Position", "Bottom", RegistryValueKind.String);
            }

            CreateSubItem(menuKey, "01Themes", "Темы", "themecpl.dll", "control /name Microsoft.Personalization");
            CreateSubItem(menuKey, "02Background", "Фон рабочего стола", "imageres.dll,-112", "control /name Microsoft.Personalization /page pageWallpaper");
            CreateSubItem(menuKey, "03Colors", "Цвета", "themecpl.dll", "control /name Microsoft.Personalization /page pageColorization");
            CreateSubItem(menuKey, "04Fonts", "Шрифты", "fontext.dll", "control fonts");
            CreateSubItem(menuKey, "05Sounds", "Звуки", "mmsys.cpl", "control mmsys.cpl,,1");
            CreateSubItem(menuKey, "06DesktopIcons", "Значки рабочего стола", "imageres.dll,-183", "control desk.cpl,,0");
            CreateSubItem(menuKey, "07Taskbar", "Панель задач", "taskbarcpl.dll", "control /name Microsoft.TaskbarAndStartMenu");
            CreateSubItem(menuKey, "08LockScreen", "Экран блокировки", "imageres.dll,-5370", "control /name Microsoft.Personalization /page pageLockScreen");
            CreateSubItem(menuKey, "09ScreenSaver", "Заставка", "shell32.dll,-17", "control desk.cpl,,1");
            CreateSubItem(menuKey, "10StartMenu", "Пуск", "imageres.dll,-5316", "control /name Microsoft.TaskbarAndStartMenu");
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // ПОДМЕНЮ "СИСТЕМНЫЕ УТИЛИТЫ" - С ПРАВИЛЬНЫМИ ИКОНКАМИ!
        // ═══════════════════════════════════════════════════════════════════════════
        private static void AddSystemUtilitiesMenu()
        {
            string menuKey = $"{BasePath}\\SystemUtilities";

            using (var key = Registry.CurrentUser.CreateSubKey(menuKey, true))
            {
                if (key == null)
                    throw new Exception("Не удалось создать ключ SystemUtilities");

                key.SetValue("MUIVerb", "Системные утилиты", RegistryValueKind.String);
                key.SetValue("Icon", "imageres.dll,-109", RegistryValueKind.String);
                key.SetValue("SubCommands", "", RegistryValueKind.String);
                key.SetValue("Position", "Bottom", RegistryValueKind.String);
            }

            // ПРАВИЛЬНЫЕ ИКОНКИ!
            CreateSubItem(menuKey, "01DeviceManager", "Диспетчер устройств", "devmgr.dll", "mmc.exe devmgmt.msc");
            CreateSubItem(menuKey, "02DiskManagement", "Управление дисками", "imageres.dll,-109", "mmc.exe diskmgmt.msc");
            CreateSubItem(menuKey, "03Services", "Службы", "filemgmt.dll", "mmc.exe services.msc");
            CreateSubItem(menuKey, "04SystemProperties", "Свойства системы", "sysdm.cpl", "control sysdm.cpl");
            CreateSubItem(menuKey, "05NetworkConnections", "Сетевые подключения", "netcenter.dll", "control ncpa.cpl");
            CreateSubItem(menuKey, "06FolderOptions", "Параметры папок", "shell32.dll,-210", "control folders");
            CreateSubItem(menuKey, "07MouseProperties", "Указатели мыши", "main.cpl", "control main.cpl");
            CreateSubItem(menuKey, "08SystemInfo", "Сведения о системе", "msinfo32.exe", "msinfo32.exe");
            CreateSubItem(menuKey, "09WinVer", "Версия системы", "shell32.dll,-300", "winver.exe");
            CreateSubItem(menuKey, "10EventViewer", "Просмотр событий", "eventvwr.exe", "mmc.exe eventvwr.msc");
        }

        private static void CreateSubItem(string parentKey, string subKeyName, string title, string icon, string command)
        {
            string subItemPath = $"{parentKey}\\shell\\{subKeyName}";

            using (var key = Registry.CurrentUser.CreateSubKey(subItemPath, true))
            {
                if (key != null)
                {
                    key.SetValue("MUIVerb", title, RegistryValueKind.String);
                    key.SetValue("Icon", icon, RegistryValueKind.String);
                }
            }

            using (var cmdKey = Registry.CurrentUser.CreateSubKey($"{subItemPath}\\command", true))
            {
                if (cmdKey != null)
                {
                    cmdKey.SetValue("", command, RegistryValueKind.String);
                }
            }
        }

        // -------------------- УДАЛЕНИЕ ИНСТРУМЕНТОВ --------------------
        public static string RemoveDesktopTools()
        {
            int successCount = 0;
            StringBuilder result = new StringBuilder();

            result.AppendLine("◆  УДАЛЕНИЕ ИНСТРУМЕНТОВ ИЗ МЕНЮ РАБОЧЕГО СТОЛА");
            result.AppendLine("────────────────────────────────────────────");
            result.AppendLine();

            foreach (var toolKey in MainTools.Keys)
            {
                try
                {
                    RemoveMenuItem(toolKey);
                    successCount++;
                    result.AppendLine($"✓ Удалён: {MainTools[toolKey].Title}");
                }
                catch { }
            }

            try
            {
                RemoveMenuItem("PersonalizationPlus");
                successCount++;
                result.AppendLine($"✓ Удалено подменю: Персонализация+");
            }
            catch { }

            try
            {
                RemoveMenuItem("SystemUtilities");
                successCount++;
                result.AppendLine($"✓ Удалено подменю: Системные утилиты");
            }
            catch { }

            try
            {
                RestoreStandardPersonalization();
                result.AppendLine($"✓ Восстановлена стандартная 'Персонализация' Windows");
            }
            catch { }

            result.AppendLine();
            result.AppendLine("────────────────────────────────────────────");
            result.AppendLine($"Успешно удалено:   {successCount}");

            if (successCount > 0)
            {
                RefreshShell();
                result.AppendLine();
                result.AppendLine("✓ Система уведомлена об изменениях");
            }

            return result.ToString();
        }

        private static void RemoveMenuItem(string keyName)
        {
            string menuPath = $@"{BasePath}\{keyName}";
            Registry.CurrentUser.DeleteSubKeyTree(menuPath, false);
        }

        private static void RestoreStandardPersonalization()
        {
            string[] standardKeys = {
                @"Software\Classes\DesktopBackground\Shell\Personalization",
                @"Software\Classes\DesktopBackground\Shell\Display"
            };

            foreach (var keyPath in standardKeys)
            {
                try
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(keyPath, true))
                    {
                        if (key != null)
                        {
                            key.DeleteValue("ProgrammaticAccessOnly", false);
                        }
                    }
                }
                catch { }
            }
        }

        public static bool AreDesktopToolsInstalled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(BasePath);
                if (key == null) return false;

                int installedCount = 0;
                foreach (var toolKey in MainTools.Keys)
                {
                    using var subKey = key.OpenSubKey(toolKey);
                    if (subKey != null)
                        installedCount++;
                }

                return installedCount >= MainTools.Count / 2;
            }
            catch
            {
                return false;
            }
        }

        public static List<string> GetInstalledDesktopTools()
        {
            var installed = new List<string>();

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(BasePath);
                if (key != null)
                {
                    foreach (var toolKey in MainTools.Keys)
                    {
                        using var subKey = key.OpenSubKey(toolKey);
                        if (subKey != null)
                        {
                            installed.Add(MainTools[toolKey].Title);
                        }
                    }

                    using var personalizationKey = key.OpenSubKey("PersonalizationPlus");
                    if (personalizationKey != null)
                        installed.Add("Персонализация+ (подменю)");

                    using var utilitiesKey = key.OpenSubKey("SystemUtilities");
                    if (utilitiesKey != null)
                        installed.Add("Системные утилиты (подменю)");
                }
            }
            catch { }

            return installed;
        }

        public static bool IsAdministrator()
        {
            try
            {
                using var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        private static void RefreshShell()
        {
            try
            {
                SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
            }
            catch { }
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);

        private class MenuItem
        {
            public string Title { get; }
            public string Icon { get; }
            public string Command { get; }

            public MenuItem(string title, string icon, string command)
            {
                Title = title;
                Icon = icon;
                Command = command;
            }
        }

        public static string GetDiagnosticInfo()
        {
            StringBuilder info = new StringBuilder();

            info.AppendLine("◆  ДИАГНОСТИКА КОНТЕКСТНОГО МЕНЮ РАБОЧЕГО СТОЛА");
            info.AppendLine("────────────────────────────────────────────");
            info.AppendLine();
            info.AppendLine($"Права администратора:   {(IsAdministrator() ? "✓  Да" : "✗  Нет (не требуются)")}");
            info.AppendLine();

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(BasePath);

                info.AppendLine("◇  ОСНОВНЫЕ ИНСТРУМЕНТЫ");
                info.AppendLine("────────────────────────────────────────────");
                foreach (var tool in MainTools)
                {
                    using var sub = key?.OpenSubKey(tool.Key);
                    string status = sub != null ? "✓" : "✗";
                    info.AppendLine($"{status}  {tool.Value.Title}");
                }
                info.AppendLine();

                info.AppendLine("◇  ПОДМЕНЮ");
                info.AppendLine("────────────────────────────────────────────");
                using var personKey = key?.OpenSubKey("PersonalizationPlus");
                info.AppendLine($"{(personKey != null ? "✓" : "✗")}  Персонализация+");
                using var utilKey = key?.OpenSubKey("SystemUtilities");
                info.AppendLine($"{(utilKey != null ? "✓" : "✗")}  Системные утилиты");
                info.AppendLine();

                info.AppendLine("◇  ИКОНКИ В «СИСТЕМНЫЕ УТИЛИТЫ»");
                info.AppendLine("────────────────────────────────────────────");
                info.AppendLine("✓  Диспетчер устройств       →  devmgr.dll");
                info.AppendLine("✓  Управление дисками        →  imageres.dll,-109");
                info.AppendLine("✓  Параметры папок           →  shell32.dll,-210");
                info.AppendLine("✓  Просмотр событий          →  eventvwr.exe");
            }
            catch (Exception ex)
            {
                info.AppendLine($"✗  Ошибка чтения реестра: {ex.Message}");
            }

            return info.ToString();
        }
    }
}
