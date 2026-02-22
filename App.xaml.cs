using System;
using System.Windows;

namespace WindowsTweaks
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;

            ShowWelcomeMessage();
        }

        private void ShowWelcomeMessage()
        {
            bool isFirstRun = !System.IO.File.Exists(
                System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "WindowsTweaks",
                    "first_run.flag"
                )
            );

            if (isFirstRun)
            {
                // Сначала создаём флаг — независимо от ответа пользователя
                try
                {
                    string appDataPath = System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "WindowsTweaks"
                    );
                    System.IO.Directory.CreateDirectory(appDataPath);
                    System.IO.File.WriteAllText(
                        System.IO.Path.Combine(appDataPath, "first_run.flag"),
                        DateTime.Now.ToString()
                    );
                }
                catch { }

                MessageBox.Show(
                    "Добро пожаловать в WindowsTweaks Pro!\n\n" +
                    "ВАЖНАЯ ИНФОРМАЦИЯ:\n\n" +
                    "✓  Программа запускается с правами администратора\n" +
                    "   для корректной работы всех функций\n\n" +
                    "✓  Все изменения применяются напрямую к системе Windows\n\n" +
                    "✓  Рекомендуется создать точку восстановления\n" +
                    "   перед применением твиков\n\n" +
                    "Разработчик: Виталий Николаевич (vitalikkontr)",
                    "WindowsTweaks Pro — Первый запуск",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                LogException(exception);
                MessageBox.Show(
                    $"Произошла критическая ошибка:\n\n{exception.Message}\n\n" +
                    "Приложение будет закрыто. Информация сохранена в лог-файл.",
                    "Критическая ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogException(e.Exception);
            MessageBox.Show(
                $"Произошла ошибка:\n\n{e.Exception.Message}\n\n" +
                "Информация сохранена в лог-файл.",
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            e.Handled = true;
        }

        private void LogException(Exception exception)
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
                    $"error_{DateTime.Now:yyyyMMdd}.log"
                );

                string separator = new string('=', 60);
                string logEntry = $"\n{separator}\n" +
                                  $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n" +
                                  $"Ошибка: {exception.Message}\n" +
                                  $"StackTrace:\n{exception.StackTrace}\n" +
                                  $"{separator}\n";

                System.IO.File.AppendAllText(logFile, logEntry);
            }
            catch { }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
