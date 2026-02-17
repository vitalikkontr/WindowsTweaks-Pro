using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindowsTweaks
{
    public partial class MainWindow : Window
    {
        private readonly TweakEngine tweakEngine;
        private readonly Dictionary<int, Action> contentLoaders;

        public MainWindow()
        {
            tweakEngine = new TweakEngine();

            contentLoaders = new Dictionary<int, Action>
            {
                { 0, LoadPerformanceContent },
                { 1, LoadPrivacyContent },
                { 2, LoadNetworkContent },
                { 3, LoadAppearanceContent },
                { 4, LoadServicesContent },
                { 5, LoadAdministrationContent },
                { 6, LoadUtilitiesContent }
            };

            InitializeComponent();
            LoadPerformanceContent();
        }

        private void NavigationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contentLoaders == null || NavigationList.SelectedIndex < 0)
                return;

            if (contentLoaders.ContainsKey(NavigationList.SelectedIndex))
                contentLoaders[NavigationList.SelectedIndex]();
        }

        private void LoadPerformanceContent()
        {
            if (ContentPanel == null) return;

            ContentPanel.Children.Clear();

            var title = CreateTitle("⚡ Оптимизация производительности");
            ContentPanel.Children.Add(title);

            AddTweakCheckbox("Отключить визуальные эффекты Windows", "DisableVisualEffects",
                "Отключает анимации и эффекты Aero — ускоряет отзывчивость системы");
            AddTweakCheckbox("Отключить индексирование поиска", "DisableSearchIndexing",
                "Снижает нагрузку на диск, отключая службу WSearch");
            AddTweakCheckbox("Отключить SuperFetch/Prefetch", "DisableSuperfetch",
                "Освобождает ОЗУ, отключая предварительную загрузку приложений (SysMain)");
            AddTweakCheckbox("Оптимизировать файл подкачки", "OptimizePageFile",
                "Устанавливает фиксированный размер pagefile.sys (2–4 ГБ)");
            AddTweakCheckbox("Отключить спящий режим (hiberfil.sys)", "DisableHibernation",
                "Удаляет файл гибернации и освобождает место на диске (= объём ОЗУ)");
            AddTweakCheckbox("Отключить дефрагментацию по расписанию", "DisableScheduledDefrag",
                "Рекомендуется для SSD — плановая дефрагментация не нужна");
            AddTweakCheckbox("Увеличить кэш DNS", "IncreaseDNSCache",
                "Ускоряет разрешение доменных имён за счёт большего кэша");
            AddTweakCheckbox("Отключить Windows Defender (требует осторожности!)", "DisableDefender",
                "Полностью отключает встроенный антивирус — только если есть сторонний! перед приминением требуется отключение зашиты в самом антивируснке");
            AddTweakCheckbox("Отключить задержку запуска программ при старте", "DisableStartupDelay",
                "Убирает 10-секундную задержку перед запуском программ автозагрузки");
            AddTweakCheckbox("Отключить фоновые приложения", "DisableBackgroundApps",
                "Запрещает UWP-приложениям работать в фоне");
            AddTweakCheckbox("Отключить Xbox Game Bar", "DisableGameBar",
                "Отключает Game DVR и оверлей Game Bar — снижает нагрузку при играх");
            AddTweakCheckbox("Отключить прозрачность интерфейса", "DisableTransparency",
                "Отключает эффект Acrylic/Blur — немного ускоряет интерфейс");

            AddSectionSeparator("⏱️ Новые твики: загрузка и питание");

            AddTweakCheckbox("Отложенный запуск служб", "DelayedServicesStart",
                "Windows загружается быстрее: DiagTrack, WSearch, BITS и wuauserv переводятся в режим delayed-auto");
            AddTweakCheckbox("Отключение зарезервированного хранилища", "DisableReservedStorage",
                "Windows резервирует несколько ГБ под системные нужды — здесь это отключается");
            AddTweakCheckbox("Включить скрытую схему питания (макс. производительность)", "EnableUltimatePowerPlan",
                "Активирует схему Ultimate Performance — процессор работает без ограничений частоты");
            AddTweakCheckbox("Отключить автоотключение экрана", "DisableScreenOff",
                "Экран не будет выключаться при бездействии (только при питании от сети)");
            AddTweakCheckbox("Отключить автоотключение дисков", "DisableDiskSleep",
                "Предотвращает «засыпание» дисков: меньше износа от частых включений/отключений");
            AddTweakCheckbox("Увеличить кэш превью изображений", "IncreaseThumbnailCache",
                "На мощных ПК увеличивает кэш эскизов до 1 ГБ — меньше перезаписей на диск");
            AddTweakCheckbox("Перенести папку Temp в C:\\Temp", "MoveTempFolder",
                "Переносит папку временных файлов в корень диска C:. Требуется перезагрузка!");

            StatusText.Text = "Производительность: готов к настройке";
        }

        private void LoadPrivacyContent()
        {
            if (ContentPanel == null) return;

            ContentPanel.Children.Clear();

            var title = CreateTitle("🔒 Конфиденциальность и телеметрия");
            ContentPanel.Children.Add(title);

            AddTweakCheckbox("Отключить телеметрию Windows", "DisableTelemetry",
                "Запрещает сбор и отправку данных о работе системы в Microsoft");
            AddTweakCheckbox("Отключить рекламу в меню Пуск", "DisableStartMenuAds",
                "Убирает рекламные предложения из меню Пуск");
            AddTweakCheckbox("Отключить Cortana", "DisableCortana",
                "Полностью отключает голосового помощника Cortana");
            AddTweakCheckbox("Отключить отслеживание местоположения", "DisableLocationTracking",
                "Запрещает приложениям определять ваше местоположение");
            AddTweakCheckbox("Отключить советы Windows", "DisableWindowsTips",
                "Убирает всплывающие подсказки и советы от Microsoft");
            AddTweakCheckbox("Отключить рекламный ID", "DisableAdvertisingID",
                "Отключает персональный идентификатор для таргетированной рекламы");
            AddTweakCheckbox("Блокировать сбор диагностических данных", "BlockDiagnosticData",
                "Устанавливает минимальный уровень сбора диагностики (политика)");
            AddTweakCheckbox("Отключить облачную синхронизацию", "DisableCloudSync",
                "Прекращает синхронизацию настроек Windows через OneDrive/аккаунт");
            AddTweakCheckbox("Отключить историю действий", "DisableActivityHistory",
                "Запрещает Timeline — историю открытых документов и сайтов");
            AddTweakCheckbox("Отключить веб-поиск в меню Пуск", "DisableWebSearch",
                "Убирает поиск в интернете из строки поиска Windows");
            AddTweakCheckbox("Отключить предложения приложений", "DisableAppSuggestions",
                "Запрещает Windows автоматически устанавливать рекомендованные приложения");

            AddSectionSeparator("🔇 Новые твики: уведомления и звук");

            AddTweakCheckbox("Отключить уведомления игрового режима", "DisableGameModeNotifications",
                "Windows уведомляет о включении игрового режима — этот твик убирает лишние уведомления");
            AddTweakCheckbox("Минимизация системных отчётов", "MinimizeSystemReports",
                "Уменьшает объём и количество отчётов об ошибках — снижает нагрузку на HDD/SSD");
            AddTweakCheckbox("Отключить автоприглушение звука при микрофоне", "DisableAudioDucking",
                "Windows автоматически снижает громкость других приложений при работе микрофона — здесь это отключается");

            StatusText.Text = "Конфиденциальность: готов к настройке";
        }

        private void LoadNetworkContent()
        {
            if (ContentPanel == null) return;

            ContentPanel.Children.Clear();

            var title = CreateTitle("🌐 Сетевые настройки");
            ContentPanel.Children.Add(title);

            AddTweakCheckbox("Отключить IPv6", "DisableIPv6",
                "Отключает стек IPv6 — полезно если провайдер использует только IPv4");
            AddTweakCheckbox("Оптимизировать TCP/IP", "OptimizeTCPIP",
                "Включает Chimney Offload, DCA и NetDMA для ускорения сети");
            AddTweakCheckbox("Очистить кэш DNS", "FlushDNSCache",
                "Сбрасывает кэш DNS-резолвера (аналог ipconfig /flushdns)");
            AddTweakCheckbox("Сбросить сетевые адаптеры", "ResetNetworkAdapters",
                "Выполняет полный сброс winsock и стека IP — помогает при проблемах с сетью");
            AddTweakCheckbox("Отключить лимитированное подключение", "DisableMeteredConnection",
                "Переключает Ethernet-соединение в режим «без лимита»");
            AddTweakCheckbox("Оптимизировать настройки QoS", "OptimizeQoS",
                "Убирает резервирование 20% пропускной способности для QoS-служб");
            AddTweakCheckbox("Отключить NetBIOS через TCP/IP (безопасность)", "DisableNetBIOS",
                "Снижает риски атак через NetBIOS — рекомендуется для домашней сети");
            AddTweakCheckbox("Отключить LLMNR (безопасность)", "DisableLLMNR",
                "Отключает Link-Local Multicast Name Resolution — защита от LLMNR-спуфинга");
            AddTweakCheckbox("Оптимизировать MTU для лучшей производительности", "OptimizeMTU",
                "Устанавливает MTU=1500 для Ethernet-адаптера");

            StatusText.Text = "Сеть: готов к настройке";
        }

        private void LoadAppearanceContent()
        {
            if (ContentPanel == null) return;

            ContentPanel.Children.Clear();

            var title = CreateTitle("🎨 Внешний вид и персонализация");
            ContentPanel.Children.Add(title);

            AddTweakCheckbox("Включить темную тему", "EnableDarkTheme",
                "Переключает интерфейс системы и приложений в тёмный режим");
            AddTweakCheckbox("Показывать расширения файлов", "ShowFileExtensions",
                "Отображает .exe, .txt и другие расширения в Проводнике");
            AddTweakCheckbox("Показывать скрытые файлы", "ShowHiddenFiles",
                "Делает видимыми системные и скрытые папки/файлы");
            AddTweakCheckbox("Классический контекстное меню (Win11)", "ClassicContextMenu",
                "Возвращает старое контекстное меню из Windows 10 в Windows 11");
            AddTweakCheckbox("Отключить группировку на панели задач", "DisableTaskbarGrouping",
                "Каждое окно показывается отдельной кнопкой без группировки");
            AddTweakCheckbox("Мелкие значки на панели задач", "SmallTaskbarIcons",
                "Уменьшает размер иконок на панели задач");
            AddTweakCheckbox("Убрать виджеты с панели задач (Win11)", "RemoveTaskbarWidgets",
                "Скрывает кнопку виджетов (погода/новости) с панели задач");
            AddTweakCheckbox("Показывать полный путь в заголовке Проводника", "ShowFullPath",
                "В заголовке окна Проводника отображается полный путь к папке");
            AddTweakCheckbox("Отключить встряхивание окна для сворачивания", "DisableShakeToMinimize",
                "Отключает функцию Aero Shake (встряхивание для минимизации остальных окон)");
            AddTweakCheckbox("Показывать секунды в системных часах", "EnableSecondsInClock",
                "Добавляет секунды в часы на панели задач");
            AddTweakCheckbox("Отключить экран блокировки", "DisableLockScreen",
                "Пропускает экран блокировки при выходе из сна или блокировке");

            AddSectionSeparator("🖼️ Новые твики: качество изображения и клавиатура");

            AddTweakCheckbox("Отключить сжатие обоев", "DisableWallpaperCompression",
                "По умолчанию Windows снижает качество обоев — этот твик сохраняет их в исходном качестве (JPEG 100%)");
            AddTweakCheckbox("Отключить залипание клавиш", "DisableStickyKeys",
                "Отключает срабатывание залипания при 5-кратном нажатии Shift и связанные уведомления");

            StatusText.Text = "Внешний вид: готов к настройке";
        }

        private void LoadServicesContent()
        {
            if (ContentPanel == null) return;

            ContentPanel.Children.Clear();

            var title = CreateTitle("⚙️ Управление службами Windows");
            ContentPanel.Children.Add(title);

            var warning = new TextBlock
            {
                Text = "⚠️ Внимание! Отключение служб может нарушить работу системы.",
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0)),
                Margin = new Thickness(0, 0, 0, 15),
                TextWrapping = TextWrapping.Wrap
            };
            ContentPanel.Children.Add(warning);

            AddTweakCheckbox("Отключить Windows Update (осторожно!)", "DisableWindowsUpdate",
                "Полностью останавливает службу обновлений — не рекомендуется надолго");
            AddTweakCheckbox("Отключить Windows Search", "DisableWindowsSearch",
                "Отключает фоновую индексацию файлов — освобождает ресурсы диска и CPU");
            AddTweakCheckbox("Отключить печать (Print Spooler)", "DisablePrintSpooler",
                "Если принтер не используется — отключение освобождает ресурсы");
            AddTweakCheckbox("Отключить факс", "DisableFax",
                "Служба факса не нужна большинству пользователей");
            AddTweakCheckbox("Отключить Bluetooth", "DisableBluetooth",
                "Отключает службу Bluetooth если адаптер не используется");
            AddTweakCheckbox("Отключить диагностику", "DisableDiagnostic",
                "Останавливает DiagTrack и Diagnostic Hub — снижает фоновую активность");
            AddTweakCheckbox("Отключить службу удаленного реестра", "DisableRemoteRegistry",
                "Предотвращает удалённый доступ к реестру системы — повышает безопасность");
            AddTweakCheckbox("Отключить службы домашней группы", "DisableHomeGroup",
                "Службы HomeGroup устарели в Windows 10/11 — безопасно отключить");
            AddTweakCheckbox("Отключить службу отчетов об ошибках Windows", "DisableErrorReporting",
                "Снижает нагрузку на диск: Windows не собирает дампы при сбоях приложений");

            StatusText.Text = "Службы: готов к настройке";
        }

        private void LoadAdministrationContent()
        {
            if (ContentPanel == null) return;

            ContentPanel.Children.Clear();

            var title = CreateTitle("👨‍💼 Инструменты администрирования");
            ContentPanel.Children.Add(title);

            var description = new TextBlock
            {
                Text = "Быстрый доступ к системным инструментам администрирования Windows",
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                Margin = new Thickness(0, 0, 0, 15),
                TextWrapping = TextWrapping.Wrap
            };
            ContentPanel.Children.Add(description);

            // НОВЫЙ ТВИК: Восстановление CMD в контекстном меню
            AddSectionSeparator("🖱️ Твики контекстного меню");
            AddTweakCheckbox("Восстановить запуск CMD из папки", "RestoreCmdHereContext",
                "Возвращает пункт «Открыть окно команд здесь» в контекстное меню папок");

            // Разделитель перед кнопками
            ContentPanel.Children.Add(new Separator
            {
                Margin = new Thickness(0, 10, 0, 20),
                Background = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                Height = 1
            });

            // ═══════════════════════════════════════════════════════════
            // Стиль для кнопок с эффектом наведения
            // ═══════════════════════════════════════════════════════════
            var hoverButtonStyle = new Style(typeof(Button));
            hoverButtonStyle.Setters.Add(new Setter(Button.ForegroundProperty, Brushes.White));
            hoverButtonStyle.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(0)));
            hoverButtonStyle.Setters.Add(new Setter(Button.CursorProperty, System.Windows.Input.Cursors.Hand));
            hoverButtonStyle.Setters.Add(new Setter(Button.FontSizeProperty, 13.0));
            hoverButtonStyle.Setters.Add(new Setter(Button.FontWeightProperty, FontWeights.SemiBold));

            var hoverTrigger = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
            hoverTrigger.Setters.Add(new Setter(Button.ForegroundProperty, Brushes.Black));
            hoverButtonStyle.Triggers.Add(hoverTrigger);

            // ═══════════════════════════════════════════════════════════
            // СЕКЦИЯ 1: КОНТЕКСТНОЕ МЕНЮ "ЭТОТ КОМПЬЮТЕР"
            // ═══════════════════════════════════════════════════════════

            var menuTitle = new TextBlock
            {
                Text = "📋 УПРАВЛЕНИЕ КОНТЕКСТНЫМ МЕНЮ \"ЭТОТ КОМПЬЮТЕР\" (ПКМ)",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(100, 181, 246)),
                Margin = new Thickness(0, 0, 0, 15)
            };
            ContentPanel.Children.Add(menuTitle);

            var menuDescription = new TextBlock
            {
                Text = "Добавьте системные инструменты в контекстное меню (ПКМ на \"Этот компьютер\"):\n" +
                       "• Администрирование • Панель управления • Диспетчер устройств\n" +
                       "• Управление дисками • Редактор групповой политики • Программы и компоненты\n" +
                       "• Редактор реестра • Безопасный режим (с подменю) • Службы",
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                Margin = new Thickness(0, 0, 0, 10),
                TextWrapping = TextWrapping.Wrap
            };
            ContentPanel.Children.Add(menuDescription);

            var statusPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 15) };
            var statusIcon = new TextBlock
            {
                Text = ComputerContextMenu.AreToolsInstalled() ? "✅" : "❌",
                FontSize = 16,
                Margin = new Thickness(0, 0, 10, 0)
            };
            var statusText = new TextBlock
            {
                Text = ComputerContextMenu.AreToolsInstalled()
                    ? "Статус: Системные инструменты установлены"
                    : "Статус: Системные инструменты не установлены",
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = ComputerContextMenu.AreToolsInstalled()
                    ? new SolidColorBrush(Color.FromRgb(76, 175, 80))
                    : new SolidColorBrush(Color.FromRgb(244, 67, 54))
            };
            statusPanel.Children.Add(statusIcon);
            statusPanel.Children.Add(statusText);
            ContentPanel.Children.Add(statusPanel);

            var menuButtonsPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 15) };

            var addMenuButton = new Button
            {
                Content = "➕ Добавить системные инструменты",
                Width = 280, Height = 40,
                Background = new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                Style = hoverButtonStyle
            };
            addMenuButton.Click += AddContextMenuItems_Click;

            var removeMenuButton = new Button
            {
                Content = "🗑️ Удалить системные инструменты",
                Width = 280, Height = 40,
                Background = new SolidColorBrush(Color.FromRgb(244, 67, 54)),
                Style = hoverButtonStyle
            };
            removeMenuButton.Click += RemoveContextMenuItems_Click;

            var diagnosticButton = new Button
            {
                Content = "🔍 Диагностика меню Этот компьютер",
                Width = 280, Height = 40,
                Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                Style = hoverButtonStyle
            };
            diagnosticButton.Click += DiagnosticContextMenu_Click;

            menuButtonsPanel.Children.Add(addMenuButton);
            menuButtonsPanel.Children.Add(removeMenuButton);
            menuButtonsPanel.Children.Add(diagnosticButton);
            ContentPanel.Children.Add(menuButtonsPanel);

            ContentPanel.Children.Add(new Separator
            {
                Margin = new Thickness(0, 10, 0, 20),
                Background = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                Height = 1
            });

            // ═══════════════════════════════════════════════════════════
            // СЕКЦИЯ 2: КОНТЕКСТНОЕ МЕНЮ РАБОЧЕГО СТОЛА
            // ═══════════════════════════════════════════════════════════

            var desktopMenuTitle = new TextBlock
            {
                Text = "🖥️ УПРАВЛЕНИЕ КОНТЕКСТНЫМ МЕНЮ \"РАБОЧЕГО СТОЛА\" (ПКМ)",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 152, 0)),
                Margin = new Thickness(0, 0, 0, 15)
            };
            ContentPanel.Children.Add(desktopMenuTitle);

            var desktopMenuDescription = new TextBlock
            {
                Text = "Добавьте системные инструменты в контекстное меню рабочего стола (ПКМ на пустом месте):\n\n" +
                       "📋 Основные инструменты:\n" +
                       "• Администрирование • Указатели мыши • Свойства папки\n" +
                       "• Сетевые подключения • Программы и компоненты\n" +
                       "• Редактор реестра • Диспетчер задач\n\n" +
                       "📂 Подменю:\n" +
                       "• Персонализация+ (темы, цвета, фон, шрифты и др.)\n" +
                       "• Панель настроек (система, дисплей, звук, питание и др.)",
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                Margin = new Thickness(0, 0, 0, 15),
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 20
            };
            ContentPanel.Children.Add(desktopMenuDescription);

            var desktopStatusPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 15) };
            var desktopStatusIcon = new TextBlock { FontSize = 16, Margin = new Thickness(0, 0, 10, 0) };
            var desktopStatusText = new TextBlock { FontSize = 13, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center };

            if (DesktopContextMenu.AreDesktopToolsInstalled())
            {
                desktopStatusIcon.Text = "✅";
                desktopStatusIcon.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                desktopStatusText.Text = "Инструменты установлены в контекстное меню рабочего стола";
                desktopStatusText.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
            }
            else
            {
                desktopStatusIcon.Text = "⭕";
                desktopStatusIcon.Foreground = new SolidColorBrush(Color.FromRgb(158, 158, 158));
                desktopStatusText.Text = "Инструменты не установлены";
                desktopStatusText.Foreground = new SolidColorBrush(Color.FromRgb(158, 158, 158));
            }

            desktopStatusPanel.Children.Add(desktopStatusIcon);
            desktopStatusPanel.Children.Add(desktopStatusText);
            ContentPanel.Children.Add(desktopStatusPanel);

            var desktopButtonsPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 10, 0, 20) };

            var addDesktopButton = new Button
            {
                Content = "➕ Добавить в меню Рабочего Стола",
                Width = 280, Height = 40,
                Background = new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                Style = hoverButtonStyle
            };
            addDesktopButton.Click += AddDesktopContextMenuItems_Click;

            var removeDesktopButton = new Button
            {
                Content = "🗑️ Удалить из меню Рабочего Стола",
                Width = 280, Height = 40,
                Background = new SolidColorBrush(Color.FromRgb(244, 67, 54)),
                Style = hoverButtonStyle
            };
            removeDesktopButton.Click += RemoveDesktopContextMenuItems_Click;

            var diagnosticDesktopButton = new Button
            {
                Content = "🔍 Диагностика меню Рабочего Стола",
                Width = 280, Height = 40,
                Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                Style = hoverButtonStyle
            };
            diagnosticDesktopButton.Click += DiagnosticDesktopContextMenu_Click;

            desktopButtonsPanel.Children.Add(addDesktopButton);
            desktopButtonsPanel.Children.Add(removeDesktopButton);
            desktopButtonsPanel.Children.Add(diagnosticDesktopButton);
            ContentPanel.Children.Add(desktopButtonsPanel);

            ContentPanel.Children.Add(new Separator
            {
                Margin = new Thickness(0, 20, 0, 20),
                Background = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                Height = 2
            });

            // ═══════════════════════════════════════════════════════════
            // СЕКЦИЯ 3: БЫСТРЫЙ ЗАПУСК
            // ═══════════════════════════════════════════════════════════

            var quickLaunchTitle = new TextBlock
            {
                Text = "🚀 БЫСТРЫЙ ЗАПУСК ИНСТРУМЕНТОВ",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(100, 181, 246)),
                Margin = new Thickness(0, 0, 0, 15)
            };
            ContentPanel.Children.Add(quickLaunchTitle);

            AddUtilityButton("🖥️ Администрирование", "Открыть раздел администрирования", OpenAdministration);
            AddUtilityButton("🛡️ Безопасный режим", "Перезагрузить в безопасном режиме", OpenSafeMode);
            AddUtilityButton("🔌 Диспетчер устройств", "Управление устройствами", OpenDeviceManager);
            AddUtilityButton("⚙️ Панель управления", "Классическая панель управления", OpenControlPanel);
            AddUtilityButton("📦 Программы и компоненты", "Удаление программ", OpenProgramsAndFeatures);
            AddUtilityButton("📋 Редактор групповой политики", "Открыть gpedit.msc", OpenGroupPolicy);
            AddUtilityButton("🔧 Службы", "Управление службами Windows", OpenServices);
            AddUtilityButton("💾 Управление дисками", "Открыть diskmgmt", OpenDiskManagement);
            AddUtilityButton("👤 Управление компьютером", "Открыть compmgmt.msc", OpenComputerManagement);
            AddUtilityButton("🌐 Сетевые подключения", "Открыть ncpa.cpl", OpenNetworkConnections);
            AddUtilityButton("📊 Монитор ресурсов", "Открыть resmon", OpenResourceMonitor);
            AddUtilityButton("🔍 Просмотр событий", "Открыть eventvwr", OpenEventViewer);

            StatusText.Text = "Администрирование: выберите инструмент";
        }

        private void LoadUtilitiesContent()
        {
            if (ContentPanel == null) return;

            ContentPanel.Children.Clear();

            var title = CreateTitle("🛠️ Системные утилиты");
            ContentPanel.Children.Add(title);

            AddUtilityButton("🧹 Очистка диска", "Запустить Disk Cleanup", CleanupDisk);
            AddUtilityButton("📊 Диспетчер задач", "Открыть Task Manager", OpenTaskManager);
            AddUtilityButton("🖥️ Системная информация", "Открыть msinfo32", OpenSystemInfo);
            AddUtilityButton("📁 Редактор реестра", "Открыть regedit", OpenRegistryEditor);
            AddUtilityButton("⚡ Управление энергопитанием", "Открыть powercfg", OpenPowerConfig);
            AddUtilityButton("🔧 Службы Windows", "Открыть services.msc", OpenServices);
            AddUtilityButton("💾 Управление дисками", "Открыть diskmgmt", OpenDiskManagement);
            AddUtilityButton("🌐 Сетевые подключения", "Открыть ncpa.cpl", OpenNetworkConnections);
            AddUtilityButton("💿 Резервное копирование драйверов", "Создать резервную копию на Рабочем столе", BackupDrivers);

            StatusText.Text = "Утилиты: выберите действие";
        }

        // ═══════════════════════════════════════════════════════
        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ UI
        // ═══════════════════════════════════════════════════════

        private TextBlock CreateTitle(string text)
        {
            return new TextBlock
            {
                Text = text,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
        }

        private void AddSectionSeparator(string sectionName)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 15, 0, 10)
            };

            var line1 = new System.Windows.Shapes.Rectangle
            {
                Height = 1,
                Width = 20,
                Fill = new SolidColorBrush(Color.FromRgb(100, 181, 246)),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 8, 0)
            };

            var sectionLabel = new TextBlock
            {
                Text = sectionName,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(100, 181, 246)),
                VerticalAlignment = VerticalAlignment.Center
            };

            panel.Children.Add(line1);
            panel.Children.Add(sectionLabel);
            ContentPanel.Children.Add(panel);
        }

        private void AddTweakCheckbox(string label, string tweakKey, string tooltip = "")
        {
            bool isApplied = tweakEngine.IsTweakApplied(tweakKey);

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 3, 0, 3)
            };

            var statusIcon = new TextBlock
            {
                Text = isApplied ? "✅" : "⬜",
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 8, 0),
                ToolTip = isApplied ? "Твик применён" : "Твик не применён"
            };

            var checkbox = new CheckBox
            {
                Content = label,
                FontSize = 14,
                Foreground = isApplied ? new SolidColorBrush(Color.FromRgb(76, 175, 80)) : Brushes.White,
                Tag = tweakKey,
                VerticalAlignment = VerticalAlignment.Center,
                ToolTip = string.IsNullOrEmpty(tooltip) ? null : tooltip
            };

            bool isUpdating = false;

            // При установке галочки — только помечаем твик как "нужно применить"
            checkbox.Checked += (s, e) =>
            {
                if (isUpdating) return;

                // Если твик уже применён — снимаем пометку "к отмене"
                tweakEngine.EnableTweak(tweakKey);

                // Визуально показываем "ожидает применения" (жёлтый цвет)
                checkbox.Foreground = new SolidColorBrush(Color.FromRgb(255, 193, 7));
                statusIcon.Text = "🔲";
                statusIcon.ToolTip = "Ожидает применения (нажмите «Применить»)";

                StatusText.Text = $"📋 Отмечено для применения: {label}";
            };

            // При снятии галочки — только помечаем твик как "нужно отменить"
            checkbox.Unchecked += (s, e) =>
            {
                if (isUpdating) return;

                tweakEngine.DisableTweak(tweakKey);

                // Визуально показываем "ожидает отмены" (красноватый цвет)
                checkbox.Foreground = new SolidColorBrush(Color.FromRgb(239, 83, 80));
                statusIcon.Text = "🔳";
                statusIcon.ToolTip = "Ожидает отмены (нажмите «Отменить»)";

                StatusText.Text = $"📋 Отмечено для отмены: {label}";
            };

            isUpdating = true;
            checkbox.IsChecked = isApplied;
            // После инициализации — восстановить корректный цвет (без жёлтого/красного)
            checkbox.Foreground = isApplied ? new SolidColorBrush(Color.FromRgb(76, 175, 80)) : Brushes.White;
            statusIcon.Text = isApplied ? "✅" : "⬜";
            isUpdating = false;

            stackPanel.Children.Add(statusIcon);
            stackPanel.Children.Add(checkbox);

            ContentPanel.Children.Add(stackPanel);
        }

        private void AddUtilityButton(string icon, string label, Action action)
        {
            var button = new Button
            {
                Content = $"{icon} {label}",
                Height = 45,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 5, 0, 5),
                FontSize = 14,
                Background = new SolidColorBrush(Color.FromRgb(66, 165, 245)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Padding = new Thickness(15, 8, 15, 8)
            };

            button.MouseEnter += (s, e) =>
            {
                button.Background = new SolidColorBrush(Color.FromRgb(100, 181, 246));
                button.Foreground = Brushes.Black;
            };

            button.MouseLeave += (s, e) =>
            {
                button.Background = new SolidColorBrush(Color.FromRgb(66, 165, 245));
                button.Foreground = Brushes.White;
            };

            button.Click += (s, e) => action?.Invoke();

            ContentPanel.Children.Add(button);
        }

        private async void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            // Собираем все твики, которые включены (галочка стоит), но ещё не применены
            var tweaksToApply = tweakEngine.GetEnabledButNotAppliedTweaks();

            if (tweaksToApply.Count == 0)
            {
                MessageBox.Show(
                    "Нет твиков для применения.\n\n" +
                    "Поставьте галочки напротив твиков, которые хотите применить,\n" +
                    "затем нажмите эту кнопку.",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Будет применено твиков: {tweaksToApply.Count}\n\n" +
                "Рекомендуется создать точку восстановления перед применением.",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                StatusText.Text = $"⏳ Применение {tweaksToApply.Count} твиков...";

                try
                {
                    await tweakEngine.ApplySelectedTweaksAsync(tweaksToApply);
                    StatusText.Text = $"✅ Успешно применено {tweaksToApply.Count} твиков!";

                    MessageBox.Show(
                        "╔═══════════════════════════════════════════════════╗\n" +
                        "║   ✅ ИЗМЕНЕНИЯ УСПЕШНО ПРИМЕНЕНЫ!                 ║\n" +
                        "╚═══════════════════════════════════════════════════╝\n\n" +
                        $"📋 Применено твиков: {tweaksToApply.Count}\n\n" +
                        "• Некоторые изменения вступят в силу после\n" +
                        "  перезагрузки системы\n\n" +
                        "• Проверьте логи в %AppData%\\WindowsTweaks\\Logs",
                        "Успешно",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    RefreshAllCheckboxes();
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка при применении изменений";
                    MessageBox.Show(
                        $"Произошла ошибка:\n{ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
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

                MessageBox.Show(
                    "Точка восстановления системы успешно создана!",
                    "Успешно",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка создания точки восстановления";
                MessageBox.Show(
                    $"Не удалось создать точку восстановления:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "╔═════════════════════════════════════════════╗\n" +
                "║       СПРАВКА - WindowsTweaks Pro           ║\n" +
                "╚═════════════════════════════════════════════╝\n\n" +
                "📋 ИНСТРУКЦИЯ ПО ИСПОЛЬЗОВАНИЮ:\n\n" +
                "1️⃣ Выберите категорию настроек в левом меню\n" +
                "   (Производительность, Конфиденциальность и т.д.)\n\n" +
                "2️⃣ Отметьте нужные твики галочками\n" +
                "   Твик применяется СРАЗУ при установке галочки!\n\n" +
                "3️⃣ Для отмены — просто снимите галочку\n\n" +
                "⚠️ ВАЖНЫЕ РЕКОМЕНДАЦИИ:\n\n" +
                "• Создавайте точку восстановления системы\n" +
                "  перед применением изменений!\n\n" +
                "• Некоторые изменения требуют перезагрузки\n\n" +
                "• Твики с ⚠️ в названии требуют осторожности\n\n" +
                "🎯 ДОБАВЛЕНИЕ ПУНКТОВ В МЕНЮ:\n\n" +
                "Раздел 'Администрирование' позволяет добавить\n" +
                "системные утилиты в контекстные меню:\n" +
                "• \"Этот компьютер\" (ПКМ)\n" +
                "• Рабочий стол (ПКМ на пустом месте)\n\n" +
                "👤 Разработчик: Виталий Николаевич (vitalikkontr)",
                "Справка - WindowsTweaks Pro",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "╔═════════════════════════════════════════════╗\n" +
                "║   WindowsTweaks Pro Edition v2.6            ║\n" +
                "╚═════════════════════════════════════════════╝\n\n" +
                "🎯 Профессиональный инструмент для оптимизации\n" +
                "   и настройки операционной системы Windows\n\n" +
                "✨ ОСНОВНЫЕ ВОЗМОЖНОСТИ:\n" +
                "   • 48 твиков для оптимизации системы\n" +
                "   • Оптимизация производительности и питания\n" +
                "   • Настройка конфиденциальности\n" +
                "   • Управление службами Windows\n" +
                "   • Мгновенное применение и отмена твиков\n" +
                "   • Контекстное меню \"Этот компьютер\"\n" +
                "   • Контекстное меню рабочего стола\n\n" +
                "🆕 НОВОЕ В v2.6:\n" +
                "   • +13 новых твиков (питание, звук, сжатие обоев)\n" +
                "   • Отложенный запуск служб\n" +
                "   • Восстановление CMD в контекстном меню\n" +
                "   • Подсказки для каждого твика\n\n" +
                "👤 Разработчик:\n" +
                "   Виталий Николаевич (vitalikkontr)\n\n" +
                "📅 Версия: 2.6 (18.02.2026)\n\n" +
                "© 2026 WindowsTweaks Pro Edition",
                "О программе WindowsTweaks Pro",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
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
                                MessageBox.Show(
                                    $"Резервное копирование драйверов завершено!\n\n" +
                                    $"Папка: {backupFolder}\n\n" +
                                    $"Для восстановления драйверов запустите:\n" +
                                    $"Install-all-drivers.bat",
                                    "Успешно",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                                System.Diagnostics.Process.Start("explorer.exe", backupFolder);
                            }
                            else
                            {
                                StatusText.Text = "❌ Ошибка при создании резервной копии драйверов";
                                MessageBox.Show(
                                    "Не удалось создать резервную копию драйверов.\n\n" +
                                    "Убедитесь что:\n" +
                                    "• Вы запустили программу с правами администратора\n" +
                                    "• Достаточно места на диске",
                                    "Ошибка",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                            }
                        });
                    });
                }
            }
            catch (System.ComponentModel.Win32Exception)
            {
                StatusText.Text = "❌ Требуются права администратора для резервного копирования драйверов";
                MessageBox.Show(
                    "Для резервного копирования драйверов требуются права администратора.\n\nПодтвердите запрос UAC.",
                    "Требуются права администратора",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                StatusText.Text = "❌ Ошибка при резервном копировании драйверов";
                MessageBox.Show($"Произошла ошибка:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Не удалось открыть: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Не удалось открыть: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // АДМИНИСТРАТИВНЫЕ ИНСТРУМЕНТЫ
        // ═══════════════════════════════════════════════════════════════════

        private void OpenAdministration() => StartProcess("control", "admintools");

        private void OpenSafeMode()
        {
            var result = MessageBox.Show(
                "Вы хотите перезагрузить компьютер в безопасном режиме?\n\n" +
                "Компьютер будет перезагружен, и при следующем запуске откроется меню выбора режима загрузки.",
                "Безопасный режим",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "shutdown",
                        Arguments = "/r /o /f /t 0",
                        Verb = "runas",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    };
                    System.Diagnostics.Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось перезагрузить: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(
                    "Редактор групповой политики недоступен в данной версии Windows.\n\n" +
                    "Он доступен только в Pro, Enterprise и Education версиях Windows.",
                    "Недоступно",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void OpenComputerManagement() => StartMmc("compmgmt.msc");
        private void OpenResourceMonitor() => StartProcess("resmon.exe");
        private void OpenEventViewer() => StartMmc("eventvwr.msc");

        // ═══════════════════════════════════════════════════════════════════
        // ОБРАБОТЧИКИ КОНТЕКСТНОГО МЕНЮ "ЭТОТ КОМПЬЮТЕР"
        // ═══════════════════════════════════════════════════════════════════

        private void AddContextMenuItems_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Добавить системные инструменты в контекстное меню \"Этот компьютер\"?\n\n" +
                "Будут добавлены следующие пункты:\n" +
                "• Администрирование\n• Панель управления\n• Диспетчер устройств\n" +
                "• Управление дисками\n• Редактор групповой политики\n• Программы и компоненты\n" +
                "• Редактор реестра\n• Безопасный режим (с подменю)\n• Службы",
                "Добавление системных инструментов",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    StatusText.Text = "Добавление пунктов в контекстное меню...";
                    string addResult = ComputerContextMenu.AddSystemTools();
                    MessageBox.Show(addResult, "Результат добавления", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAdministrationContent();
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка добавления пунктов меню";
                    MessageBox.Show($"Не удалось добавить пункты в контекстное меню:\n\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RemoveContextMenuItems_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Удалить системные инструменты из контекстного меню \"Этот компьютер\"?\n\n" +
                "Это действие можно отменить, снова добавив пункты через эту программу.",
                "Удаление системных инструментов",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    StatusText.Text = "Удаление пунктов из контекстного меню...";
                    string removeResult = ComputerContextMenu.RemoveSystemTools();
                    MessageBox.Show(removeResult, "Результат удаления", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAdministrationContent();
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка удаления пунктов меню";
                    MessageBox.Show($"Не удалось удалить пункты из контекстного меню:\n\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DiagnosticContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string diagnostic = ComputerContextMenu.GetDiagnosticInfo();
                ShowDiagnosticWindow("Диагностика контекстного меню", diagnostic);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка диагностики:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // ОБРАБОТЧИКИ КОНТЕКСТНОГО МЕНЮ РАБОЧЕГО СТОЛА
        // ═══════════════════════════════════════════════════════════════════

        private void AddDesktopContextMenuItems_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Добавить системные инструменты в контекстное меню рабочего стола?\n\n" +
                "📋 БУДУТ ДОБАВЛЕНЫ:\n\nОсновные инструменты:\n" +
                "• Администрирование\n• Указатели мыши\n• Свойства папки\n" +
                "• Сетевые подключения\n• Программы и компоненты\n" +
                "• Редактор реестра\n• Диспетчер задач\n\n" +
                "Подменю:\n• Персонализация+\n• Панель настроек",
                "Добавление в контекстное меню рабочего стола",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    StatusText.Text = "Добавление пунктов в контекстное меню рабочего стола...";
                    string addResult = DesktopContextMenu.AddDesktopTools();
                    MessageBox.Show(addResult, "Результат добавления", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAdministrationContent();
                    StatusText.Text = "✅ Инструменты успешно добавлены в меню рабочего стола";
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка добавления пунктов в меню";
                    MessageBox.Show($"Не удалось добавить пункты:\n\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RemoveDesktopContextMenuItems_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Удалить системные инструменты из контекстного меню рабочего стола?\n\n" +
                "Это действие можно отменить, снова добавив пункты через эту программу.",
                "Удаление из контекстного меню",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    StatusText.Text = "Удаление пунктов из контекстного меню рабочего стола...";
                    string removeResult = DesktopContextMenu.RemoveDesktopTools();
                    MessageBox.Show(removeResult, "Результат удаления", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAdministrationContent();
                    StatusText.Text = "✅ Инструменты успешно удалены из меню рабочего стола";
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка удаления пунктов из меню";
                    MessageBox.Show($"Не удалось удалить пункты:\n\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DiagnosticDesktopContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string diagnostic = DesktopContextMenu.GetDiagnosticInfo();
                ShowDiagnosticWindow("Диагностика контекстного меню рабочего стола", diagnostic);
                StatusText.Text = "Диагностика выполнена";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка диагностики:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowDiagnosticWindow(string title, string content)
        {
            var diagnosticWindow = new Window
            {
                Title = title,
                Width = 700,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30))
            };

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Padding = new Thickness(20)
            };

            var textBlock = new TextBlock
            {
                Text = content,
                Foreground = Brushes.White,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap
            };

            scrollViewer.Content = textBlock;
            diagnosticWindow.Content = scrollViewer;
            diagnosticWindow.ShowDialog();
        }

        // ═══════════════════════════════════════════════════════════════════
        // ОТМЕНА ПРИМЕНЕННЫХ ТВИКОВ
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
                MessageBox.Show(
                    "Нет твиков для отмены.\n\n" +
                    "Снимите галочки с тех твиков, которые хотите отменить,\n" +
                    "затем нажмите эту кнопку.",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"⚠️ Будет отменено твиков: {tweaksToRevert.Count}\n\n" +
                "Отменяются только те твики, с которых СНЯТЫ галочки.\n" +
                "Твики с установленными галочками останутся активными.\n\n" +
                "⚠️ ВНИМАНИЕ: Некоторые изменения могут потребовать перезагрузки!",
                "Подтверждение отмены твиков",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                StatusText.Text = $"⏳ Отмена {tweaksToRevert.Count} твиков...";

                try
                {
                    await tweakEngine.RevertSelectedTweaksAsync(tweaksToRevert);

                    StatusText.Text = $"✅ Успешно отменено {tweaksToRevert.Count} твиков!";

                    MessageBox.Show(
                        "╔═══════════════════════════════════════════════════╗\n" +
                        "║   ✅ ВЫБРАННЫЕ ТВИКИ УСПЕШНО ОТМЕНЕНЫ!            ║\n" +
                        "╚═══════════════════════════════════════════════════╝\n\n" +
                        $"🔄 Отменено твиков: {tweaksToRevert.Count}\n\n" +
                        "📋 Что было сделано:\n" +
                        "   • Отменены только снятые вами твики\n" +
                        "   • Твики с галочками остались активными\n" +
                        "   • Восстановлены настройки реестра\n\n" +
                        "⚠️ ВАЖНО:\n" +
                        "   Некоторые изменения вступят в силу после\n" +
                        "   перезагрузки системы.",
                        "Отмена твиков завершена",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    RefreshAllCheckboxes();
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка при отмене твиков";
                    MessageBox.Show(
                        "╔═══════════════════════════════════════════════════╗\n" +
                        "║   ❌ ОШИБКА ПРИ ОТМЕНЕ ТВИКОВ                     ║\n" +
                        "╚═══════════════════════════════════════════════════╝\n\n" +
                        $"Описание ошибки:\n{ex.Message}\n\n" +
                        "💡 Попробуйте:\n" +
                        "   • Запустить программу от имени администратора\n" +
                        "   • Проверить логи в папке AppData\\WindowsTweaks\\Logs",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
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
