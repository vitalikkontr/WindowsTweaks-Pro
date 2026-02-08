using System;
using System.Windows;

namespace WindowsTweaks
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Устанавливаем обработчик необработанных исключений
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;

            // Показываем приветственное сообщение при первом запуске
            ShowWelcomeMessage();
        }

        private void ShowWelcomeMessage()
        {
            // Проверяем, первый ли это запуск
            var settings = System.Configuration.ConfigurationManager.AppSettings;
            bool isFirstRun = !System.IO.File.Exists(
                System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "WindowsTweaks",
                    "first_run.flag"
                )
            );

            if (isFirstRun)
            {
                var result = MessageBox.Show(
                    "╔═══════════════════════════════════════════════════╗\n" +
                    "║    Добро пожаловать в WindowsTweaks Pro!         ║\n" +
                    "╚═══════════════════════════════════════════════════╝\n\n" +
                    "🎯 ВАЖНАЯ ИНФОРМАЦИЯ:\n\n" +
                    "✅ Для большинства функций НЕ требуются права\n" +
                    "   администратора (включая добавление пунктов\n" +
                    "   в контекстное меню \"Этот компьютер\")\n\n" +
                    "⚠️ Права администратора нужны только для:\n" +
                    "   • Изменения системных служб\n" +
                    "   • Некоторых твиков производительности\n" +
                    "   • Создания точек восстановления\n\n" +
                    "📋 Рекомендуется создать точку восстановления\n" +
                    "   перед применением изменений!\n\n" +
                    "👤 Разработчик: Виталий Николаевич (vitalikkontr)\n\n" +
                    "Показать это сообщение снова?",
                    "WindowsTweaks Pro - Первый запуск",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                // Создаём флаг первого запуска
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
            }
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                LogException(exception);
                MessageBox.Show(
                    $"Произошла критическая ошибка:\n\n{exception.Message}\n\n" +
                    "Приложение будет закрыто. Информация об ошибке сохранена в лог-файл.",
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
                "Информация об ошибке сохранена в лог-файл.",
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            e.Handled = true; // Предотвращаем закрытие приложения
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

                string logEntry = $"\n{'=' * 60}\n" +
                                 $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n" +
                                 $"Ошибка: {exception.Message}\n" +
                                 $"StackTrace:\n{exception.StackTrace}\n" +
                                 $"{'=' * 60}\n";

                System.IO.File.AppendAllText(logFile, logEntry);
            }
            catch
            {
                // Игнорируем ошибки логирования
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // Очистка ресурсов при выходе (если нужно)
        }
    }
}