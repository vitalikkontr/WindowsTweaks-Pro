using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace WindowsTweaks
{
    /// <summary>
    /// Класс для управления контекстным меню "Этот компьютер"
    /// Исправленная версия с правильными путями реестра и экранированием команд
    /// </summary>
    public static class ComputerContextMenu
    {
        // Правильный базовый путь для контекстного меню "Этот компьютер"
        private const string BaseRegistryPath = @"Software\Classes\CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\shell";

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
            string safeModeKey = $@"{BaseRegistryPath}\SafeMode";

            // 1. Создаем Главный пункт меню
            using (var key = Registry.CurrentUser.CreateSubKey(safeModeKey, true))
            {
                if (key == null)
                    throw new Exception("Не удалось создать ключ SafeMode");

                key.SetValue("MUIVerb", "Безопасный режим", RegistryValueKind.String);
                key.SetValue("Icon", "shell32.dll,47", RegistryValueKind.String);
                key.SetValue("SubCommands", "", RegistryValueKind.String);
            }

            string subShellPath = $@"{safeModeKey}\shell";

            // 2. Создаем подпункты с исправленным экранированием кавычек для PowerShell

            // Перезагрузка в среду восстановления (Диагностика)
            CreateSafeModeSubItem(subShellPath, "01Reboot", "Перезагрузка в меню диагностики", "shell32.dll,238",
                "powershell.exe -WindowStyle Hidden -Command \"Start-Process shutdown.exe -ArgumentList \\\"/r /o /f /t 0\\\" -Verb RunAs\"");

            // Безопасный режим (Минимальный)
            CreateSafeModeSubItem(subShellPath, "02SafeMode", "Включить безопасный режим", "shell32.dll,47",
                "powershell.exe -WindowStyle Hidden -Command \"Start-Process bcdedit -ArgumentList \\\"/set {current} safeboot minimal\\\" -Verb RunAs; shutdown /r /t 0\"");

            // Безопасный режим с командной строкой (Две последовательные команды bcdedit)
            CreateSafeModeSubItem(subShellPath, "03SafeModeCmd", "Безопасный режим с командной строкой", "cmd.exe,0",
                "powershell.exe -WindowStyle Hidden -Command \"Start-Process bcdedit -ArgumentList \\\"/set {current} safeboot minimal\\\" -Verb RunAs; Start-Process bcdedit -ArgumentList \\\"/set {current} safebootalternateshell yes\\\" -Verb RunAs; shutdown /r /t 0\"");

            // Безопасный режим с сетью
            CreateSafeModeSubItem(subShellPath, "04SafeModeNetwork", "Безопасный режим с поддержкой сети", "netcenter.dll,0",
                "powershell.exe -WindowStyle Hidden -Command \"Start-Process bcdedit -ArgumentList \\\"/set {current} safeboot network\\\" -Verb RunAs; shutdown /r /t 0\"");

            // Отмена безопасного режима
            CreateSafeModeSubItem(subShellPath, "05SafeModeCancel", "Вернуть обычную загрузку Windows", "shell32.dll,81",
                "powershell.exe -WindowStyle Hidden -Command \"Start-Process bcdedit -ArgumentList \\\"/deletevalue {current} safeboot\\\" -Verb RunAs; shutdown /r /t 0\"");
        }

        private static void CreateSafeModeSubItem(string parentShellPath, string subItemName, string title, string icon, string command)
        {
            string itemKeyPath = $@"{parentShellPath}\{subItemName}";

            using (var key = Registry.CurrentUser.CreateSubKey(itemKeyPath, true))
            {
                if (key == null) return;

                key.SetValue("", title, RegistryValueKind.String);
                key.SetValue("Icon", icon, RegistryValueKind.String);

                using (var cmdKey = key.CreateSubKey("command", true))
                {
                    if (cmdKey != null)
                    {
                        cmdKey.SetValue("", command, RegistryValueKind.String);
                    }
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

        // -------------------- Удаление инструментов (БЕЗОПАСНОЕ) --------------------
        public static string RemoveSystemTools()
        {
            int successCount = 0;
            int failCount = 0;
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            result.AppendLine("◆  УДАЛЕНИЕ ИНСТРУМЕНТОВ ИЗ КОНТЕКСТНОГО МЕНЮ");
            result.AppendLine("────────────────────────────────────────────");
            result.AppendLine();

            // Безопасное удаление: удаляем только НАШИ ключи поочередно, чтобы не задеть чужие твики в shell
            foreach (var toolKey in SystemTools.Keys)
            {
                try
                {
                    string fullPath = $"{BaseRegistryPath}\\{toolKey}";
                    Registry.CurrentUser.DeleteSubKeyTree(fullPath, false);
                    successCount++;
                }
                catch { failCount++; }
            }

            // Удаляем блок SafeMode
            try
            {
                string safeModeFullPath = $"{BaseRegistryPath}\\SafeMode";
                Registry.CurrentUser.DeleteSubKeyTree(safeModeFullPath, false);
                successCount++;
                result.AppendLine("✓ Все добавленные пункты и меню SafeMode успешно удалены");
            }
            catch (Exception ex)
            {
                result.AppendLine($"✗ Ошибка при удалении блока SafeMode: {ex.Message}");
                failCount++;
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

        private static bool IsAdministrator()
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
            var info = new System.Text.StringBuilder();

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

                        foreach (var subKeyName in subKeys)
                        {
                            if (subKeyName.Equals("SafeMode", StringComparison.OrdinalIgnoreCase))
                                continue;

                            using var subKey = key.OpenSubKey(subKeyName);
                            if (subKey != null)
                            {
                                string title = subKey.GetValue("MUIVerb", subKey.GetValue("", "")).ToString();
                                if (string.IsNullOrWhiteSpace(title)) title = "Инструмент без названия";
                                info.AppendLine($"✓  {title} [{subKeyName}]");
                            }
                        }

                        info.AppendLine();
                        info.AppendLine("◇  ДОПОЛНИТЕЛЬНЫЕ РЕЖИМЫ");
                        info.AppendLine("────────────────────────────────────────────");

                        using var safeModeKey = key.OpenSubKey("SafeMode");
                        if (safeModeKey != null)
                        {
                            string title = safeModeKey.GetValue("MUIVerb", "Перезагрузка в SafeMode").ToString();
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
                info.AppendLine($"✗  Ошибка чтения реестра: {ex.Message}");
            }

            return info.ToString();
        }
    }
}