using System;
using System.Collections.Generic;
using System.Windows;

namespace WindowsTweaks
{
    public partial class MainWindow
    {
        private async void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            // Собираем все твики, которые включены (галочка стоит), но ещё не применены
            var tweaksToApply = tweakEngine.GetEnabledButNotAppliedTweaks();

            if (tweaksToApply.Count == 0)
            {
                ThemedDialog.Show("Нет твиков для применения.\n\n" +
                    "Поставьте галочки напротив твиков, которые хотите применить,\n" +
                    "затем нажмите эту кнопку.",
                    "Информация", DialogIcon.Info, this);
                return;
            }

            bool result = ThemedDialog.Confirm(
                $"Будет применено твиков: {tweaksToApply.Count}\n\n" +
                "Рекомендуется создать точку восстановления перед применением.", "Подтверждение", DialogIcon.Question, this);
            if (result)
            {
                StatusText.Text = $"⏳ Применение {tweaksToApply.Count} твиков...";

                try
                {
                    await tweakEngine.ApplySelectedTweaksAsync(tweaksToApply);
                    StatusText.Text = $"✅ Успешно применено {tweaksToApply.Count} твиков!";

                    ThemedDialog.Show(
                        $"Применено твиков: {tweaksToApply.Count}\n\n" +
                        "Некоторые изменения вступят в силу\n" +
                        "после перезагрузки системы.", "Изменения применены", DialogIcon.Success, this);

                    RefreshAllCheckboxes();
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка при применении изменений";
                    ThemedDialog.Show(
                        $"Произошла ошибка:\n{ex.Message}", "Ошибка", DialogIcon.Error, this);
                }
            }
        }

        private void CreateRestorePoint_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Создание точки восстановления...";

            try
            {
                tweakEngine.CreateRestorePoint("WindowsTweaks - Перед изменениями");
                StatusText.Text = "Точка восстановления создана";

                ThemedDialog.Show(
                    "Точка восстановления системы успешно создана!", "Успешно", DialogIcon.Info, this);
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка создания точки восстановления";
                ThemedDialog.Show(
                    $"Не удалось создать точку восстановления:\n{ex.Message}", "Ошибка", DialogIcon.Error, this);
            }
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            ThemedDialog.Show(
                "ИНСТРУКЦИЯ ПО ИСПОЛЬЗОВАНИЮ:\n\n" +
                "1. Выберите категорию настроек в левом меню\n" +
                "   (Производительность, Конфиденциальность и т.д.)\n\n" +
                "2. Отметьте нужные твики галочками\n" +
                "   Ожидайте применения через кнопку «Применить»\n\n" +
                "3. Для отмены — снимите галочку и нажмите «Отменить»\n\n" +
                "⚠ ВАЖНЫЕ РЕКОМЕНДАЦИИ:\n\n" +
                "• Создавайте точку восстановления перед изменениями!\n" +
                "• Некоторые изменения требуют перезагрузки\n" +
                "• Твики с ⚠ в названии требуют осторожности\n\n" +
                "Раздел «Администрирование» позволяет добавить системные\n" +
                "утилиты в контекстные меню «Этот компьютер» и Рабочего стола.\n\n" +
                "Разработчик: Виталий Николаевич (vitalikkontr)",
                "Справка — WindowsTweaks Pro",
                DialogIcon.Info, this);
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            ThemedDialog.Show(
                "WindowsTweaks Pro Edition v3.0.2.0\n\n" +
                "Профессиональный инструмент для оптимизации\n" +
                "и настройки операционной системы Windows.\n\n" +
                "ОСНОВНЫЕ ВОЗМОЖНОСТИ:\n" +
                "• 80+ твиков для оптимизации системы\n" +
                "• Оптимизация производительности и питания\n" +
                "• Настройка конфиденциальности\n" +
                "• Управление службами Windows\n" +
                "• Мгновенное применение и отмена твиков\n" +
                "• Контекстные меню «Этот компьютер» и Рабочего стола\n\n" +
                "НОВОЕ В v3.0.2.0:\n" +
                "• +16 новых твиков (игры, приватность, сеть, UI)\n" +
                "• Отложенный запуск служб\n" +
                "• Восстановление CMD в контекстном меню\n" +
                "• Подсказки для каждого твика\n\n" +
                "Разработчик: Виталий Николаевич (vitalikkontr)\n" +
                "Версия: 3.0.2.0  |  24.06.2026  |  © 2026 WindowsTweaks Pro",
                "О программе WindowsTweaks Pro",
                DialogIcon.Info, this);
        }

        // ═══════════════════════════════════════════════════════════════════
        // УТИЛИТЫ
        // ═══════════════════════════════════════════════════════════════════

        private void CleanupDisk() => StartProcess("cleanmgr.exe");
        private void OpenTaskManager() => StartProcess("taskmgr.exe");
        private void OpenSystemInfo() => StartProcess("msinfo32.exe");
        private void OpenRegistryEditor() => StartProcess("regedit.exe");
        private void OpenPowerConfig() => StartProcess("powercfg.cpl");
        private void OpenServices() => StartMmc("services.msc");
        private void OpenDiskManagement() => StartMmc("diskmgmt.msc");
        private void OpenNetworkConnections() => StartProcess("ncpa.cpl");

        private void BackupDrivers()
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string backupFolder = System.IO.Path.Combine(desktopPath, "DriverBackup");

                if (!System.IO.Directory.Exists(backupFolder))
                    System.IO.Directory.CreateDirectory(backupFolder);

                string installBatPath = System.IO.Path.Combine(backupFolder, "Install-all-drivers.bat");
                string installBatContent = "@echo off\r\npnputil /add-driver *.inf /install /subdirs\r\necho.\r\necho Finished.\r\necho.\r\necho Reboot after pressing button.\r\necho.\r\nshutdown /r /t 3\r\n";
                System.IO.File.WriteAllText(installBatPath, installBatContent);

                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c dism /online /export-driver /destination:\"{backupFolder}\"",
                    UseShellExecute = true,
                    Verb = "runas",
                    CreateNoWindow = false,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
                };

                var process = System.Diagnostics.Process.Start(psi);

                if (process != null)
                {
                    StatusText.Text = "⏳ Резервное копирование драйверов... Дождитесь завершения.";

                    System.Threading.Tasks.Task.Run(() =>
                    {
                        process.WaitForExit();

                        Dispatcher.Invoke(() =>
                        {
                            if (process.ExitCode == 0)
                            {
                                StatusText.Text = $"✅ Драйверы скопированы на Рабочий стол в папку DriverBackup";
                                ThemedDialog.Show(
                                    $"Резервное копирование драйверов завершено!\n\n" +
                                    $"Папка: {backupFolder}\n\n" +
                                    $"Для восстановления драйверов запустите:\n" +
                                    $"Install-all-drivers.bat", "Успешно", DialogIcon.Info, this);

                                System.Diagnostics.Process.Start("explorer.exe", backupFolder);
                            }
                            else
                            {
                                StatusText.Text = "❌ Ошибка при создании резервной копии драйверов";
                                ThemedDialog.Show(
                                    "Не удалось создать резервную копию драйверов.\n\n" +
                                    "Убедитесь что:\n" +
                                    "• Вы запустили программу с правами администратора\n" +
                                    "• Достаточно места на диске", "Ошибка", DialogIcon.Error, this);
                            }
                        });
                    });
                }
            }
            catch (System.ComponentModel.Win32Exception)
            {
                StatusText.Text = "❌ Требуются права администратора для резервного копирования драйверов";
                ThemedDialog.Show(
                    "Для резервного копирования драйверов требуются права администратора.\n\nПодтвердите запрос UAC.", "Требуются права администратора", DialogIcon.Warning, this);
            }
            catch (Exception ex)
            {
                StatusText.Text = "❌ Ошибка при резервном копировании драйверов";
                ThemedDialog.Show($"Произошла ошибка:\n{ex.Message}", "Ошибка", DialogIcon.Error, this);
            }
        }

        private void StartProcess(string fileName, string arguments = "")
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
                StatusText.Text = $"✅ Запущено: {fileName}";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"❌ Ошибка запуска: {fileName}";
                ThemedDialog.Show($"Не удалось открыть: {ex.Message}", "Ошибка", DialogIcon.Error, this);
            }
        }

        private void StartMmc(string snapin)
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "mmc.exe",
                    Arguments = snapin,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
                StatusText.Text = $"✅ Запущено: {snapin}";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"❌ Ошибка запуска: {snapin}";
                ThemedDialog.Show($"Не удалось открыть: {ex.Message}", "Ошибка", DialogIcon.Error, this);
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // АДМИНИСТРАТИВНЫЕ ИНСТРУМЕНТЫ
        // ═══════════════════════════════════════════════════════════════════

        private void OpenAdministration() => StartProcess("control", "admintools");

        private void OpenSafeMode()
        {
            bool result = ThemedDialog.Confirm("Вы хотите перезагрузить компьютер в безопасном режиме?\n\n" +
                "Компьютер будет перезагружен, и при следующем запуске откроется меню выбора режима загрузки.",
                "Безопасный режим", DialogIcon.Question, this);
            if (result)
            {
                try
                {
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "shutdown",
                        Arguments = "/r /o /f /t 0",
                        Verb = "runas",
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(psi);
                }
                catch (Exception ex)
                {
                    ThemedDialog.Show($"Не удалось перезагрузить: {ex.Message}", "Ошибка", DialogIcon.Error, this);
                }
            }
        }

        private void OpenDeviceManager() => StartMmc("devmgmt.msc");
        private void OpenControlPanel() => StartProcess("control");
        private void OpenProgramsAndFeatures() => StartProcess("appwiz.cpl");

        private void OpenGroupPolicy()
        {
            try { StartMmc("gpedit.msc"); }
            catch
            {
                ThemedDialog.Show("Редактор групповой политики недоступен в данной версии Windows.\n\n" +
                    "Он доступен только в Pro, Enterprise и Education версиях Windows.",
                    "Недоступно", DialogIcon.Info, this);
            }
        }

        private void OpenComputerManagement() => StartMmc("compmgmt.msc");
        private void OpenResourceMonitor() => StartProcess("resmon.exe");
        private void OpenEventViewer() => StartMmc("eventvwr.msc");

        // ═══════════════════════════════════════════════════════════════════
        // ОБРАБОТЧИКИ КОНТЕКСТНОГО МЕНЮ "ЭТОТ КОМПЬЮТЕР"
        // ═══════════════════════════════════════════════════════════════════


        private async void RevertChanges_Click(object sender, RoutedEventArgs e)
        {
            // Отменяем только те твики, которые ПРИМЕНЕНЫ, но галочка с них СНЯТА
            var tweaksToRevert = new List<string>();

            foreach (var appliedTweak in tweakEngine.GetAppliedTweaks())
            {
                if (!tweakEngine.IsTweakEnabled(appliedTweak))
                    tweaksToRevert.Add(appliedTweak);
            }

            if (tweaksToRevert.Count == 0)
            {
                ThemedDialog.Show("Нет твиков для отмены.\n\n" +
                    "Снимите галочки с тех твиков, которые хотите отменить,\n" +
                    "затем нажмите эту кнопку.",
                    "Информация", DialogIcon.Info, this);
                return;
            }

            bool result = ThemedDialog.Confirm($"⚠️ Будет отменено твиков: {tweaksToRevert.Count}\n\n" +
                "Отменяются только те твики, с которых СНЯТЫ галочки.\n" +
                "Твики с установленными галочками останутся активными.\n\n" +
                "⚠️ ВНИМАНИЕ: Некоторые изменения могут потребовать перезагрузки!",
                "Подтверждение отмены твиков", DialogIcon.Question, this);
            if (result)
            {
                StatusText.Text = $"⏳ Отмена {tweaksToRevert.Count} твиков...";

                try
                {
                    await tweakEngine.RevertSelectedTweaksAsync(tweaksToRevert);

                    StatusText.Text = $"✅ Успешно отменено {tweaksToRevert.Count} твиков!";

                    ThemedDialog.Show(
                        $"Отменено твиков: {tweaksToRevert.Count}\n\n" +
                        "Отменены только снятые вами твики.\n" +
                        "Некоторые изменения вступят в силу\n" +
                        "после перезагрузки системы.", "Твики отменены", DialogIcon.Success, this);

                    RefreshAllCheckboxes();
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка при отмене твиков";
                    ThemedDialog.Show(
                        $"{ex.Message}\n\n" +
                        "Попробуйте запустить программу\n" +
                        "от имени администратора.", "Ошибка при отмене", DialogIcon.Error, this);
                }
            }
        }

        private void RefreshAllCheckboxes()
        {
            try
            {
                if (NavigationList.SelectedIndex >= 0 && contentLoaders.ContainsKey(NavigationList.SelectedIndex))
                    contentLoaders[NavigationList.SelectedIndex]();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка обновления: {ex.Message}");
            }
        }
    }
}
