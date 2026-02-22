using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace WindowsTweaks
{
    /// <summary>
    /// Класс для управления контекстным меню "Этот компьютер"
    /// Исправленная версия с правильными путями реестра
    /// </summary>
    public static class ComputerContextMenu
    {
        // Правильный базовый путь для контекстного меню "Этот компьютер"
        private const string BaseRegistryPath =
@"Software\Classes\CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\shell";

        private static readonly Dictionary<string, MenuItem> SystemTools =
        new Dictionary<string, MenuItem>
        {
            ["AdminTools"] = new MenuItem(
                "Администрирование",
                "imageres.dll,109",
                "control.exe /name Microsoft.AdministrativeTools"
            ),

            ["ControlPanel"] = new MenuItem(
                "Панель управления",
                "shell32.dll,21",
                "control.exe"
            ),

            ["DeviceManager"] = new MenuItem(
                "Диспетчер устройств",
                "devmgr.dll,5",
                "mmc.exe devmgmt.msc"
            ),

            ["DiskManagement"] = new MenuItem(
                "Управление дисками",
                "dmdskres.dll,0",
                "mmc.exe diskmgmt.msc"
            ),

            ["GroupPolicy"] = new MenuItem(
                "Редактор групповой политики",
                "gpedit.dll,0",
                "mmc.exe gpedit.msc"
            ),

            ["Programs"] = new MenuItem(
                "Программы и компоненты",
                "appwiz.cpl,0",
                "control.exe appwiz.cpl"
            ),

            ["Registry"] = new MenuItem(
                "Редактор реестра",
                "regedit.exe,0",
                "regedit.exe"
            ),

            ["Services"] = new MenuItem(
                "Службы",
                "filemgmt.dll,0",
                "mmc.exe services.msc"
            )
        };


        // -------------------- Добавление инструментов --------------------
        public static string AddSystemTools()
        {
            int successCount = 0;
            int failCount = 0;
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            result.AppendLine("◆  ДОБАВЛЕНИЕ ИНСТРУМЕНТОВ В КОНТЕКСТНОЕ МЕНЮ");
            result.AppendLine("────────────────────────────────────────────");
            result.AppendLine();

            foreach (var tool in SystemTools)
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

            // Добавляем подменю "Безопасный режим"
            try
            {
                AddSafeModeMenu();
                successCount++;
                result.AppendLine($"✓ Добавлен: Безопасный режим (с подменю)");
            }
            catch (Exception ex)
            {
                failCount++;
                result.AppendLine($"✗ Ошибка 'Безопасный режим': {ex.Message}");
            }

            // Скрываем стандартный пункт "Управление"
            try
            {
                HideManageItem();
                result.AppendLine($"✓ Скрыт: стандартный пункт 'Управление'");
            }
            catch (Exception ex)
            {
                result.AppendLine($"⚠ Предупреждение 'Управление': {ex.Message}");
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
                result.AppendLine();
                result.AppendLine("Для отображения изменений:");
                result.AppendLine("• Откройте 'Этот компьютер' заново");
                result.AppendLine("• Или нажмите F5 для обновления");
            }

            return result.ToString();
        }

        private static void AddMenuItem(string keyName, MenuItem item)
        {
            // Используем HKEY_CURRENT_USER - не требует прав администратора
            string fullPath = $"{BaseRegistryPath}\\{keyName}";

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

        // Добавление подменю "Безопасный режим"
        private static void AddSafeModeMenu()
        {
            string safeModeKey = $"{BaseRegistryPath}\\SafeMode";

            // Главный пункт меню
            using (var key = Registry.CurrentUser.CreateSubKey(safeModeKey, true))
            {
                if (key == null)
                    throw new Exception("Не удалось создать ключ SafeMode");

                key.SetValue("MUIVerb", "Безопасный режим", RegistryValueKind.String);
                key.SetValue("Icon", "shell32.dll,47", RegistryValueKind.String);
                key.SetValue("SubCommands", "", RegistryValueKind.String);
            }

            // Подменю: Перезагрузка
            CreateSafeModeSubItem("01Reboot", "Перезагрузка", "shell32.dll,238",
                "shutdown.exe /r /o /f /t 00");

            // Подменю: Безопасный режим
            CreateSafeModeSubItem("02SafeMode", "Безопасный режим", "shell32.dll,47",
                "bcdedit /set {current} safeboot minimal & shutdown /r /t 0");

            // Подменю: Безопасный режим с командной строкой
            CreateSafeModeSubItem("03SafeModeCmd", "Безопасный режим с поддержкой командной строки",
                "cmd.exe,0", "bcdedit /set {current} safeboot minimal & bcdedit /set {current} safebootalternateshell yes & shutdown /r /t 0");

            // Подменю: Безопасный режим с сетью
            CreateSafeModeSubItem("04SafeModeNetwork", "Безопасный режим с поддержкой сети",
                "netcenter.dll,0", "bcdedit /set {current} safeboot network & shutdown /r /t 0");
        }

        private static void CreateSafeModeSubItem(string keyName, string title, string icon, string command)
        {
            string subItemPath = $"{BaseRegistryPath}\\SafeMode\\shell\\{keyName}";

            using (var key = Registry.CurrentUser.CreateSubKey(subItemPath, true))
            {
                if (key != null)
                {
                    key.SetValue("MUIVerb", title, RegistryValueKind.String);
                    key.SetValue("Icon", icon, RegistryValueKind.String);
                    key.SetValue("HasLUAShield", "", RegistryValueKind.String);
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

        // Скрыть стандартный пункт "Управление"
        private static void HideManageItem()
        {
            string managePath = $"{BaseRegistryPath}\\Manage";

            using (var key = Registry.CurrentUser.CreateSubKey(managePath, true))
            {
                if (key != null)
                {
                    key.SetValue("ProgrammaticAccessOnly", "", RegistryValueKind.String);
                }
            }
        }

        // -------------------- Удаление инструментов --------------------
        public static string RemoveSystemTools()
        {
            int successCount = 0;
            int failCount = 0;
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            result.AppendLine("◆  УДАЛЕНИЕ ИНСТРУМЕНТОВ ИЗ КОНТЕКСТНОГО МЕНЮ");
            result.AppendLine("────────────────────────────────────────────");
            result.AppendLine();

            // Удаляем всю ветку shell целиком (быстрее и надёжнее)
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(BaseRegistryPath, false);
                result.AppendLine("✓ Все пункты меню успешно удалены");
                successCount = SystemTools.Count + 1; // +1 за SafeMode
            }
            catch (Exception ex)
            {
                result.AppendLine($"✗ Ошибка при удалении: {ex.Message}");
                failCount = SystemTools.Count + 1;
            }

            // Восстанавливаем стандартный пункт "Управление"
            try
            {
                RestoreManageItem();
                result.AppendLine("✓ Восстановлен: стандартный пункт 'Управление'");
            }
            catch (Exception ex)
            {
                result.AppendLine($"⚠ Предупреждение 'Управление': {ex.Message}");
            }

            result.AppendLine();
            result.AppendLine("────────────────────────────────────────────");
            result.AppendLine($"Успешно удалено:   {successCount}");
            result.AppendLine($"Ошибок:            {failCount}");

            if (successCount > 0)
            {
                RefreshShell();
                result.AppendLine();
                result.AppendLine("✓ Система уведомлена об изменениях");
            }

            return result.ToString();
        }

        // Восстановить стандартный пункт "Управление"
        private static void RestoreManageItem()
        {
            string managePath = $"{BaseRegistryPath}\\Manage";
            Registry.CurrentUser.DeleteSubKeyTree(managePath, false);
        }

        // -------------------- Проверка установки --------------------
        public static bool AreToolsInstalled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(BaseRegistryPath);
                if (key == null) return false;

                int installedCount = 0;
                foreach (var toolKey in SystemTools.Keys)
                {
                    using var subKey = key.OpenSubKey(toolKey);
                    if (subKey != null)
                        installedCount++;
                }

                // Проверяем SafeMode
                using var safeModeKey = key.OpenSubKey("SafeMode");
                if (safeModeKey != null)
                    installedCount++;

                return installedCount >= SystemTools.Count / 2;
            }
            catch
            {
                return false;
            }
        }

        public static List<string> GetInstalledTools()
        {
            var installed = new List<string>();

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(BaseRegistryPath);
                if (key != null)
                {
                    foreach (var toolKey in SystemTools.Keys)
                    {
                        using var subKey = key.OpenSubKey(toolKey);
                        if (subKey != null)
                        {
                            installed.Add(SystemTools[toolKey].Title);
                        }
                    }

                    // Проверяем SafeMode
                    using var safeModeKey = key.OpenSubKey("SafeMode");
                    if (safeModeKey != null)
                    {
                        installed.Add("Безопасный режим");
                    }
                }
            }
            catch { }

            return installed;
        }

        // -------------------- Проверка прав администратора --------------------
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

        // -------------------- Обновление Shell --------------------
        private static void RefreshShell()
        {
            try
            {
                // Уведомляем Windows об изменении ассоциаций
                SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
            }
            catch { }
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);

        // -------------------- Вложенный класс MenuItem --------------------
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

        // -------------------- Диагностика --------------------
        public static string GetDiagnosticInfo()
        {
            System.Text.StringBuilder info = new System.Text.StringBuilder();

            info.AppendLine("◆  ДИАГНОСТИКА КОНТЕКСТНОГО МЕНЮ");
            info.AppendLine("────────────────────────────────────────────");
            info.AppendLine();
            info.AppendLine($"Права администратора:   {(IsAdministrator() ? "✓  Да" : "✗  Нет (не требуются)")}");
            info.AppendLine();

            info.AppendLine("◇  РЕЕСТР  —  HKEY_CURRENT_USER");
            info.AppendLine("────────────────────────────────────────────");
            info.AppendLine($"Путь:   HKCU\\{BaseRegistryPath}");
            info.AppendLine();

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(BaseRegistryPath);
                info.AppendLine($"Ключ существует:     {(key != null ? "✓  Да" : "✗  Нет")}");

                if (key != null)
                {
                    var subKeys = key.GetSubKeyNames();
                    info.AppendLine($"Найдено подключей:   {subKeys.Length}");
                    info.AppendLine();

                    if (subKeys.Length > 0)
                    {
                        info.AppendLine("◇  УСТАНОВЛЕННЫЕ ИНСТРУМЕНТЫ");
                        info.AppendLine("────────────────────────────────────────────");

                        foreach (var toolKey in SystemTools.Keys)
                        {
                            using var subKey = key.OpenSubKey(toolKey);
                            if (subKey != null)
                            {
                                string title = subKey.GetValue("", "").ToString();
                                info.AppendLine($"✓  {title}");
                            }
                        }

                        // Проверяем SafeMode
                        using var safeModeKey = key.OpenSubKey("SafeMode");
                        if (safeModeKey != null)
                        {
                            string title = safeModeKey.GetValue("MUIVerb", "").ToString();
                            info.AppendLine($"✓  {title}");

                            using var shellKey = safeModeKey.OpenSubKey("shell");
                            if (shellKey != null)
                            {
                                var subMenuKeys = shellKey.GetSubKeyNames();
                                info.AppendLine($"    └─  подпунктов: {subMenuKeys.Length}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                info.AppendLine($"✗  Ошибка: {ex.Message}");
            }



            return info.ToString();
        }
    }
}