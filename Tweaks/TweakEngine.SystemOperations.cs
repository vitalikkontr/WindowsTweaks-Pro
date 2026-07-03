using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;

namespace WindowsTweaks
{
    public partial class TweakEngine
    {
        public void CreateRestorePoint(string description)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName        = "powershell.exe",
                    Arguments       = $"-Command \"Checkpoint-Computer -Description '{description}' -RestorePointType 'MODIFY_SETTINGS'\"",
                    Verb            = "runas",
                    UseShellExecute = true,
                    WindowStyle     = ProcessWindowStyle.Hidden
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
                // UseShellExecute=true нужен для Verb="runas",
                // но тогда CreateNoWindow не работает — используем WindowStyle.Hidden
                var psi = new ProcessStartInfo
                {
                    FileName               = "cmd.exe",
                    Arguments              = $"/c {command}",
                    UseShellExecute        = true,
                    WindowStyle            = ProcessWindowStyle.Hidden,
                    Verb                   = "runas"
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

    }
}
