using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowsTweaks
{
    /// <summary>
    /// ОБНОВЛЁННАЯ ВЕРСИЯ - добавлен winver в системные утилиты
    /// Скрывает стандартный пункт Windows "Персонализация"
    /// </summary>
    public static class DesktopContextMenu
    {
        // HKEY_CURRENT_USER\Software\Classes\Directory\Background\shell
        private const string BasePath = @"Software\Classes\Directory\Background\shell";
        
        private static readonly Dictionary<string, MenuItem> DesktopTools = new Dictionary<string, MenuItem>
        {
            // Основные инструменты
            ["TaskManager"] = new MenuItem("Диспетчер задач", "taskmgr.exe", "taskmgr"),
            ["RegistryEditor"] = new MenuItem("Редактор реестра", "regedit.exe", "regedit"),
            ["ControlPanel"] = new MenuItem("Панель управления", "shell32.dll,21", "control.exe"),
            ["AdminTools"] = new MenuItem("Администрирование", "imageres.dll,109", "control.exe /name Microsoft.AdministrativeTools"),
            ["Programs"] = new MenuItem("Программы и компоненты", "appwiz.cpl,0", "control.exe appwiz.cpl"),
            ["DeviceManager"] = new MenuItem("Диспетчер устройств", "devmgr.dll,5", "mmc.exe devmgmt.msc"),
            ["DiskManagement"] = new MenuItem("Управление дисками", "dmdskres.dll", "mmc.exe diskmgmt.msc"),
            ["Services"] = new MenuItem("Службы", "filemgmt.dll,0", "mmc.exe services.msc"),
            ["SystemProperties"] = new MenuItem("Свойства системы", "sysdm.cpl", "control.exe sysdm.cpl"),
            ["NetworkConnections"] = new MenuItem("Сетевые подключения", "netshell.dll", "control.exe ncpa.cpl"),
            ["MouseProperties"] = new MenuItem("Указатели мыши", "main.cpl", "control.exe main.cpl")
        };

        // -------------------- ДОБАВЛЕНИЕ ИНСТРУМЕНТОВ --------------------
        public static string AddDesktopTools()
        {
            int successCount = 0;
            int failCount = 0;
            StringBuilder result = new StringBuilder();

            result.AppendLine("╔════════════════════════════════════════════════════╗");
            result.AppendLine("║   ДОБАВЛЕНИЕ ИНСТРУМЕНТОВ В МЕНЮ РАБОЧЕГО СТОЛА    ║");
            result.AppendLine("╚════════════════════════════════════════════════════╝");
            result.AppendLine();
            result.AppendLine($"Права администратора: {(IsAdministrator() ? "✓ Да" : "✗ Нет (не требуются)")}");
            result.AppendLine($"Используется: Registry.CurrentUser");
            result.AppendLine($"Путь: HKCU\\{BasePath}");
            result.AppendLine();

            foreach (var tool in DesktopTools)
            {
                try
                {
                    AddMenuItem(tool.Key, tool.Value);
                    successCount++;
                    result.AppendLine($"✓ Добавлен: {tool.Value.Title}");
                }
                catch (Exception ex)
                {
                    failCount++;
                    result.AppendLine($"✗ Ошибка '{tool.Value.Title}': {ex.Message}");
                }
            }

            // Скрываем стандартную "Персонализацию" Windows
            try
            {
                HideStandardPersonalization();
                result.AppendLine($"✓ Скрыт стандартный пункт 'Персонализация'");
            }
            catch (Exception ex)
            {
                result.AppendLine($"⚠ Предупреждение при скрытии 'Персонализация': {ex.Message}");
            }

            // Добавляем расширенное подменю "Персонализация+"
            try
            {
                AddPersonalizationMenu();
                successCount++;
                result.AppendLine($"✓ Добавлен: Персонализация+ (расширенное подменю)");
            }
            catch (Exception ex)
            {
                failCount++;
                result.AppendLine($"✗ Ошибка 'Персонализация+': {ex.Message}");
            }

            // Добавляем подменю "Системные утилиты"
            try
            {
                AddSystemUtilitiesMenu();
                successCount++;
                result.AppendLine($"✓ Добавлен: Системные утилиты (с подменю + winver)");
            }
            catch (Exception ex)
            {
                failCount++;
                result.AppendLine($"✗ Ошибка 'Системные утилиты': {ex.Message}");
            }

            result.AppendLine();
            result.AppendLine("════════════════════════════════════════════════════");
            result.AppendLine($"Успешно добавлено: {successCount}");
            result.AppendLine($"Ошибок: {failCount}");
            result.AppendLine("════════════════════════════════════════════════════");

            if (successCount > 0)
            {
                RefreshShell();
                result.AppendLine();
                result.AppendLine("✓ Система уведомлена об изменениях");
                result.AppendLine();
                result.AppendLine("Для отображения изменений:");
                result.AppendLine("• Нажмите F5 на рабочем столе");
                result.AppendLine("• Щёлкните ПКМ по пустому месту рабочего стола");
            }

            return result.ToString();
        }

        private static void AddMenuItem(string keyName, MenuItem item)
        {
            string fullPath = $"{BasePath}\\{keyName}";

            using (var key = Registry.CurrentUser.CreateSubKey(fullPath, true))
            {
                if (key == null)
                    throw new Exception("Не удалось создать ключ реестра");

                key.SetValue("", item.Title, RegistryValueKind.String);
                key.SetValue("Icon", item.Icon, RegistryValueKind.String);
            }

            using (var cmdKey = Registry.CurrentUser.CreateSubKey($"{fullPath}\\command", true))
            {
                if (cmdKey == null)
                    throw new Exception("Не удалось создать ключ command");

                cmdKey.SetValue("", item.Command, RegistryValueKind.String);
            }
        }

        // Скрываем стандартную "Персонализацию" Windows
        private static void HideStandardPersonalization()
        {
            // Windows использует GUID для персонализации
            // Путь: HKCU\Software\Classes\DesktopBackground\Shell\Personalize
            string personalizePath = @"Software\Classes\DesktopBackground\Shell\Personalize";

            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(personalizePath, true))
                {
                    if (key != null)
                    {
                        // Добавляем параметр, который скрывает пункт меню
                        key.SetValue("ProgrammaticAccessOnly", "", RegistryValueKind.String);
                    }
                }
            }
            catch
            {
                // Если не удалось - не критично, просто будет два пункта персонализации
            }
        }

        // Подменю "Персонализация+"
        private static void AddPersonalizationMenu()
        {
            string menuPath = $"{BasePath}\\PersonalizationPlus";

            using (var key = Registry.CurrentUser.CreateSubKey(menuPath, true))
            {
                if (key == null)
                    throw new Exception("Не удалось создать ключ PersonalizationPlus");

                key.SetValue("MUIVerb", "🎨 Персонализация+", RegistryValueKind.String);
                key.SetValue("Icon", "themecpl.dll", RegistryValueKind.String);
                key.SetValue("SubCommands", "", RegistryValueKind.String);
            }

            // Исправленные команды персонализации
            CreateSubMenuItem("PersonalizationPlus", "01Themes", "Темы", "themecpl.dll", "control.exe /name Microsoft.Personalization");
            CreateSubMenuItem("PersonalizationPlus", "02Background", "Фон рабочего стола", "imageres.dll,112", "control.exe /name Microsoft.Personalization /page pageWallpaper");
            CreateSubMenuItem("PersonalizationPlus", "03Colors", "Цвета", "themecpl.dll", "control.exe /name Microsoft.Personalization /page pageColorization");
            CreateSubMenuItem("PersonalizationPlus", "04Fonts", "Шрифты", "fontext.dll", "control.exe fonts");
            CreateSubMenuItem("PersonalizationPlus", "05Mouse", "Указатели мыши", "main.cpl", "control.exe main.cpl");
            CreateSubMenuItem("PersonalizationPlus", "06Sounds", "Звуки", "mmsys.cpl", "control.exe mmsys.cpl");
            CreateSubMenuItem("PersonalizationPlus", "07Icons", "Значки рабочего стола", "imageres.dll,3", "rundll32.exe shell32.dll,Control_RunDLL desk.cpl,,5");
            CreateSubMenuItem("PersonalizationPlus", "08ScreenSaver", "Заставка", "shell32.dll,16", "control.exe desk.cpl,,1");
        }

        // Подменю "Системные утилиты" - ОБНОВЛЕНО с winver
        private static void AddSystemUtilitiesMenu()
        {
            string menuPath = $"{BasePath}\\SystemUtilities";

            using (var key = Registry.CurrentUser.CreateSubKey(menuPath, true))
            {
                if (key == null)
                    throw new Exception("Не удалось создать ключ SystemUtilities");

                key.SetValue("MUIVerb", "⚙️ Системные утилиты", RegistryValueKind.String);
                key.SetValue("Icon", "shell32.dll,316", RegistryValueKind.String);
                key.SetValue("SubCommands", "", RegistryValueKind.String);
            }

            // Системные утилиты с добавленным winver
            CreateSubMenuItem("SystemUtilities", "01WindowsVersion", "О версии Windows", "shell32.dll,1", "winver");
            CreateSubMenuItem("SystemUtilities", "02Display", "Параметры экрана", "desk.cpl", "control.exe desk.cpl");
            CreateSubMenuItem("SystemUtilities", "03Sound", "Звук", "mmsys.cpl", "control.exe mmsys.cpl");
            CreateSubMenuItem("SystemUtilities", "04Power", "Электропитание", "powercpl.dll", "control.exe powercfg.cpl");
            CreateSubMenuItem("SystemUtilities", "05DateTime", "Дата и время", "timedate.cpl", "control.exe timedate.cpl");
            CreateSubMenuItem("SystemUtilities", "06Region", "Язык и региональные стандарты", "intl.cpl", "control.exe intl.cpl");
            CreateSubMenuItem("SystemUtilities", "07FolderOptions", "Параметры папок", "shell32.dll,210", "control.exe folders");
            CreateSubMenuItem("SystemUtilities", "08Indexing", "Параметры индексирования", "shell32.dll", "control.exe /name Microsoft.IndexingOptions");
            CreateSubMenuItem("SystemUtilities", "09Performance", "Счётчики производительности", "perfmon.exe", "perfmon.exe");
        }

        private static void CreateSubMenuItem(string parentKey, string keyName, string title, string icon, string command)
        {
            string subItemPath = $"{BasePath}\\{parentKey}\\shell\\{keyName}";

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
            int failCount = 0;
            StringBuilder result = new StringBuilder();

            result.AppendLine("╔════════════════════════════════════════════════════╗");
            result.AppendLine("║   УДАЛЕНИЕ ИНСТРУМЕНТОВ ИЗ МЕНЮ РАБОЧЕГО СТОЛА     ║");
            result.AppendLine("╚════════════════════════════════════════════════════╝");
            result.AppendLine();

            foreach (var tool in DesktopTools)
            {
                try
                {
                    RemoveMenuItem(tool.Key);
                    successCount++;
                    result.AppendLine($"✓ Удалён: {tool.Value.Title}");
                }
                catch (Exception ex)
                {
                    failCount++;
                    result.AppendLine($"✗ Ошибка '{tool.Value.Title}': {ex.Message}");
                }
            }

            // Восстанавливаем стандартную "Персонализацию"
            try
            {
                RestoreStandardPersonalization();
                result.AppendLine($"✓ Восстановлен стандартный пункт 'Персонализация'");
            }
            catch (Exception ex)
            {
                result.AppendLine($"⚠ Предупреждение: {ex.Message}");
            }

            // Удаляем подменю
            try
            {
                RemoveMenuItem("PersonalizationPlus");
                successCount++;
                result.AppendLine($"✓ Удалён: Персонализация+");
            }
            catch { failCount++; }

            try
            {
                RemoveMenuItem("SystemUtilities");
                successCount++;
                result.AppendLine($"✓ Удалён: Системные утилиты");
            }
            catch { failCount++; }

            result.AppendLine();
            result.AppendLine("════════════════════════════════════════════════════");
            result.AppendLine($"Успешно удалено: {successCount}");
            result.AppendLine($"Ошибок: {failCount}");
            result.AppendLine("════════════════════════════════════════════════════");

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

        // Восстанавливаем стандартную "Персонализацию"
        private static void RestoreStandardPersonalization()
        {
            string personalizePath = @"Software\Classes\DesktopBackground\Shell\Personalize";

            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(personalizePath, true))
                {
                    if (key != null)
                    {
                        // Удаляем параметр, который скрывал пункт
                        key.DeleteValue("ProgrammaticAccessOnly", false);
                    }
                }
            }
            catch
            {
                // Если не удалось - не критично
            }
        }

        // -------------------- ПРОВЕРКА УСТАНОВКИ --------------------
        public static bool AreDesktopToolsInstalled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(BasePath);
                if (key == null) return false;

                int installedCount = 0;
                foreach (var toolKey in DesktopTools.Keys)
                {
                    using var subKey = key.OpenSubKey(toolKey);
                    if (subKey != null)
                        installedCount++;
                }

                return installedCount >= DesktopTools.Count / 2;
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
                    foreach (var toolKey in DesktopTools.Keys)
                    {
                        using var subKey = key.OpenSubKey(toolKey);
                        if (subKey != null)
                            installed.Add(DesktopTools[toolKey].Title);
                    }

                    using var personalizationKey = key.OpenSubKey("PersonalizationPlus");
                    if (personalizationKey != null)
                        installed.Add("🎨 Персонализация+");

                    using var systemUtilitiesKey = key.OpenSubKey("SystemUtilities");
                    if (systemUtilitiesKey != null)
                        installed.Add("⚙️ Системные утилиты");
                }
            }
            catch { }

            return installed;
        }

        // -------------------- ПРОВЕРКА ПРАВ --------------------
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

        // -------------------- ОБНОВЛЕНИЕ EXPLORER --------------------
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

        // -------------------- КЛАСС MENUITEM --------------------
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

        // -------------------- ДИАГНОСТИКА --------------------
        public static string GetDiagnosticInfo()
        {
            StringBuilder info = new StringBuilder();

            info.AppendLine("╔═══════════════════════════════════════════════════════════╗");
            info.AppendLine("║       ДИАГНОСТИКА КОНТЕКСТНОГО МЕНЮ РАБОЧЕГО СТОЛА        ║");
            info.AppendLine("╚═══════════════════════════════════════════════════════════╝");
            info.AppendLine();
            info.AppendLine($"Права администратора: {(IsAdministrator() ? "✓ Да" : "✗ Нет (не требуются)")}");
            info.AppendLine($"Используется: Registry.CurrentUser");
            info.AppendLine();
            info.AppendLine("ПУТЬ В РЕЕСТРЕ:");
            info.AppendLine($"HKEY_CURRENT_USER\\{BasePath}");
            info.AppendLine();

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(BasePath);
                info.AppendLine($"Базовый ключ существует: {(key != null ? "✓ Да" : "✗ Нет")}");

                if (key != null)
                {
                    var subKeys = key.GetSubKeyNames();
                    info.AppendLine($"Найдено подключей: {subKeys.Length}");

                    if (subKeys.Length > 0)
                    {
                        info.AppendLine();
                        info.AppendLine("Установленные инструменты:");

                        foreach (var toolKey in DesktopTools.Keys)
                        {
                            using var subKey = key.OpenSubKey(toolKey);
                            if (subKey != null)
                            {
                                string title = subKey.GetValue("", "").ToString();
                                info.AppendLine($"  ✓ {toolKey}: {title}");
                            }
                        }

                        using var personalizationKey = key.OpenSubKey("PersonalizationPlus");
                        if (personalizationKey != null)
                        {
                            info.AppendLine($"  ✓ PersonalizationPlus: 🎨 Персонализация+");
                            
                            using var shellKey = personalizationKey.OpenSubKey("shell");
                            if (shellKey != null)
                            {
                                var subMenuKeys = shellKey.GetSubKeyNames();
                                info.AppendLine($"    └─ Подпунктов: {subMenuKeys.Length}");
                            }
                        }

                        using var systemUtilitiesKey = key.OpenSubKey("SystemUtilities");
                        if (systemUtilitiesKey != null)
                        {
                            info.AppendLine($"  ✓ SystemUtilities: ⚙️ Системные утилиты");
                            
                            using var shellKey = systemUtilitiesKey.OpenSubKey("shell");
                            if (shellKey != null)
                            {
                                var subMenuKeys = shellKey.GetSubKeyNames();
                                info.AppendLine($"    └─ Подпунктов: {subMenuKeys.Length}");
                                info.AppendLine($"    └─ НОВОЕ: Добавлен 'О версии Windows' (winver)");
                            }
                        }
                    }
                }

                // Проверяем статус стандартной персонализации
                info.AppendLine();
                info.AppendLine("СТАНДАРТНАЯ ПЕРСОНАЛИЗАЦИЯ WINDOWS:");
                string personalizePath = @"Software\Classes\DesktopBackground\Shell\Personalize";
                using var personalizeKey = Registry.CurrentUser.OpenSubKey(personalizePath);
                if (personalizeKey != null)
                {
                    bool isHidden = personalizeKey.GetValue("ProgrammaticAccessOnly") != null;
                    info.AppendLine($"  Статус: {(isHidden ? "✓ Скрыта" : "✗ Видима")}");
                }
                else
                {
                    info.AppendLine("  Статус: Ключ не найден");
                }
            }
            catch (Exception ex)
            {
                info.AppendLine($"❌ ОШИБКА: {ex.Message}");
            }

            info.AppendLine();
            info.AppendLine("═══════════════════════════════════════════════════════════");
            info.AppendLine();
            info.AppendLine("ОБНОВЛЕНИЯ В ЭТОЙ ВЕРСИИ:");
            info.AppendLine("✅ Добавлен 'О версии Windows' (winver) в системные утилиты");
            info.AppendLine("   → Первый пункт в подменю 'Системные утилиты'");
            info.AppendLine("   → Показывает версию и сборку Windows");
            info.AppendLine();
            info.AppendLine("СТРУКТУРА МЕНЮ 'СИСТЕМНЫЕ УТИЛИТЫ':");
            info.AppendLine("⚙️ Системные утилиты");
            info.AppendLine("  ├─ О версии Windows (winver) ← НОВОЕ!");
            info.AppendLine("  ├─ Параметры экрана");
            info.AppendLine("  ├─ Звук");
            info.AppendLine("  ├─ Электропитание");
            info.AppendLine("  ├─ Дата и время");
            info.AppendLine("  ├─ Язык и региональные стандарты");
            info.AppendLine("  ├─ Параметры папок");
            info.AppendLine("  ├─ Параметры индексирования");
            info.AppendLine("  └─ Счётчики производительности");

            return info.ToString();
        }
    }
}
