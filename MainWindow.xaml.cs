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
            NavigationList.SelectedIndex = 0;
            LoadPerformanceContent();
        }

        private void Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
            else
            {
                DragMove();
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;

        private void BtnClose_Click(object sender, RoutedEventArgs e)
            => Close();

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
                "Полностью отключает встроенный антивирус — только если есть сторонний! Перед применением отключите защиту в настройках самого Defender");
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

            AddSectionSeparator("🎮 Игровые и системные оптимизации");

            AddTweakCheckbox("Аппаратное ускорение GPU (HAGS)", "EnableHAGS",
                "Позволяет GPU самостоятельно управлять своей очередью задач — снижает задержку в играх (Win10 2004+, требует актуального драйвера)");
            AddTweakCheckbox("Отключить HPET (High Precision Event Timer)", "DisableHPET",
                "На некоторых системах отключение HPET снижает задержку ввода — эффект зависит от железа, рекомендуется проверить в тестах");
            AddTweakCheckbox("Принудительно включить TRIM для SSD", "EnableTRIM",
                "TRIM сообщает SSD какие блоки можно очистить — повышает скорость записи и продлевает срок службы накопителя");
            AddTweakCheckbox("Отключить ускорение мыши (линейный ввод)", "DisableMouseAcceleration",
                "Убирает 'усиление' курсора при быстром движении — важно для точного прицеливания в играх и графических редакторах");
            AddTweakCheckbox("Приоритет CPU для активного приложения", "SetHighCpuPriority",
                "Win32PrioritySeparation=38: активное окно получает максимальный квант времени процессора — быстрее реагирует на ввод");

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

            AddSectionSeparator("🔐 Приватность устройств");

            AddTweakCheckbox("Отключить службу биометрии (Windows Hello)", "DisableBiometrics",
                "Останавливает WbioSrvc — отпечатки и распознавание лица не работают. Если не используете Windows Hello — безопасно отключить");
            AddTweakCheckbox("Запретить приложениям доступ к камере", "DisableCameraAccess",
                "Запрещает всем приложениям (кроме системных) использовать веб-камеру через политику CapabilityAccessManager");
            AddTweakCheckbox("Запретить приложениям доступ к микрофону", "DisableMicrophoneAccess",
                "Запрещает всем приложениям (кроме системных) использовать микрофон через политику CapabilityAccessManager");
            AddTweakCheckbox("Очищать историю последних файлов при выходе", "ClearRecentOnExit",
                "При каждом выходе из Windows автоматически удаляет список последних открытых документов и папок из меню Пуск и Проводника");
            AddTweakCheckbox("Отключить Центр уведомлений (Action Center)", "DisableNotificationCenter",
                "Скрывает иконку и панель уведомлений — уведомления от приложений не накапливаются и не отвлекают");

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
                "Оптимизирует параметры стека TCP/IP — RSS, RSC, ECN, InitialRto");
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
                "Устанавливает MTU=1500 для Ethernet-адаптера (откат: 1492)");

            AddSectionSeparator("⚡ Продвинутые сетевые оптимизации");

            AddTweakCheckbox("Включить ECN (Explicit Congestion Notification)", "EnableECN",
                "ECN позволяет маршрутизаторам сигнализировать о перегрузке без потери пакетов — снижает задержку при нагруженном канале");
            AddTweakCheckbox("Отключить алгоритм Nagle (снижение пинга в играх)", "DisableNagle",
                "Nagle объединяет мелкие пакеты для экономии трафика, но добавляет задержку. Отключение снижает пинг в онлайн-играх");

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
            AddTweakCheckbox("Классическое контекстное меню (Win11)", "ClassicContextMenu",
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

            AddSectionSeparator("🖥️ Анимации и поведение интерфейса");

            AddTweakCheckbox("Отключить анимацию открытия/закрытия окон", "DisableWindowAnimations",
                "Убирает плавное появление и скрытие окон — интерфейс реагирует мгновенно. Хорошо сочетается с отключением визуальных эффектов");
            AddTweakCheckbox("Всегда показывать строку меню в Проводнике", "ShowMenuBar",
                "Возвращает классическую строку меню (Файл, Правка, Вид...) постоянно видимой без нажатия Alt");
            AddTweakCheckbox("Отключить динамическую подсветку поиска (Win11)", "DisableSearchHighlights",
                "Убирает анимированные обои и рекламный контент из строки поиска Windows 11 — поиск становится чище и быстрее");
            AddTweakCheckbox("Включать NumLock при запуске Windows", "EnableNumLockOnStartup",
                "NumLock будет автоматически активирован после входа в систему — не нужно каждый раз нажимать вручную");

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
                Margin = new Thickness(0, 0, 0, 10),
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
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(168, 196, 174)),
                Margin = new Thickness(0, 0, 0, 10),
                TextWrapping = TextWrapping.Wrap
            };
            ContentPanel.Children.Add(description);

            AddSectionSeparator("🖱️ Твики контекстного меню");
            AddTweakCheckbox("Восстановить запуск CMD из папки", "RestoreCmdHereContext",
                "Возвращает пункт «Открыть окно команд здесь» в контекстное меню папок");

            AddThemedSeparator();

            // ─── Секция 1: Этот компьютер ───
            AddSectionHeader("📋 УПРАВЛЕНИЕ КОНТЕКСТНЫМ МЕНЮ «ЭТОТ КОМПЬЮТЕР» (ПКМ)");

            ContentPanel.Children.Add(new TextBlock
            {
                Text = "Добавьте системные инструменты в контекстное меню (ПКМ на «Этот компьютер»):\n" +
                       "• Администрирование • Панель управления • Диспетчер устройств\n" +
                       "• Управление дисками • Редактор групповой политики • Программы и компоненты\n" +
                       "• Редактор реестра • Безопасный режим (с подменю) • Службы",
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(168, 196, 174)),
                Margin = new Thickness(0, 0, 0, 8),
                TextWrapping = TextWrapping.Wrap
            });

            // Статус
            AddStatusBadge(
                ComputerContextMenu.AreToolsInstalled(),
                "Статус: Системные инструменты установлены",
                "Статус: Системные инструменты не установлены");

            // Кнопки
            var menuButtons = MakeButtonRow();
            menuButtons.Children.Add(MakeActionButton("+ Добавить системные инструменты",  ButtonKind.Add,      AddContextMenuItems_Click));
            menuButtons.Children.Add(MakeActionButton("🗑 Удалить системные инструменты",   ButtonKind.Remove,   RemoveContextMenuItems_Click));
            menuButtons.Children.Add(MakeActionButton("🔍 Диагностика меню Этот компьютер", ButtonKind.Neutral,  DiagnosticContextMenu_Click));
            ContentPanel.Children.Add(menuButtons);

            AddThemedSeparator();

            // ─── Секция 2: Рабочий стол ───
            AddSectionHeader("🖥 УПРАВЛЕНИЕ КОНТЕКСТНЫМ МЕНЮ «РАБОЧЕГО СТОЛА» (ПКМ)");

            ContentPanel.Children.Add(new TextBlock
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
                Foreground = new SolidColorBrush(Color.FromRgb(168, 196, 174)),
                Margin = new Thickness(0, 0, 0, 8),
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 20
            });

            AddStatusBadge(
                DesktopContextMenu.AreDesktopToolsInstalled(),
                "Инструменты установлены в контекстное меню рабочего стола",
                "Инструменты не установлены");

            var desktopButtons = MakeButtonRow();
            desktopButtons.Children.Add(MakeActionButton("+ Добавить в меню Рабочего Стола",   ButtonKind.Add,     AddDesktopContextMenuItems_Click));
            desktopButtons.Children.Add(MakeActionButton("🗑 Удалить из меню Рабочего Стола",   ButtonKind.Remove,  RemoveDesktopContextMenuItems_Click));
            desktopButtons.Children.Add(MakeActionButton("🔍 Диагностика меню Рабочего Стола",  ButtonKind.Neutral, DiagnosticDesktopContextMenu_Click));
            ContentPanel.Children.Add(desktopButtons);

            AddThemedSeparator();

            // ─── Секция 3: Быстрый запуск ───
            AddSectionHeader("🚀 БЫСТРЫЙ ЗАПУСК ИНСТРУМЕНТОВ");

            AddUtilityButton("🖥️", "Администрирование", "Открыть раздел администрирования", OpenAdministration);
            AddUtilityButton("🛡️", "Безопасный режим", "Перезагрузить в безопасном режиме", OpenSafeMode);
            AddUtilityButton("🔌", "Диспетчер устройств", "Управление устройствами", OpenDeviceManager);
            AddUtilityButton("⚙️", "Панель управления", "Классическая панель управления", OpenControlPanel);
            AddUtilityButton("📦", "Программы и компоненты", "Удаление программ", OpenProgramsAndFeatures);
            AddUtilityButton("📋", "Редактор групповой политики", "Открыть gpedit.msc", OpenGroupPolicy);
            AddUtilityButton("🔧", "Службы", "Управление службами Windows", OpenServices);
            AddUtilityButton("💾", "Управление дисками", "Открыть diskmgmt", OpenDiskManagement);
            AddUtilityButton("👤", "Управление компьютером", "Открыть compmgmt.msc", OpenComputerManagement);
            AddUtilityButton("🌐", "Сетевые подключения", "Открыть ncpa.cpl", OpenNetworkConnections);
            AddUtilityButton("📊", "Монитор ресурсов", "Открыть resmon", OpenResourceMonitor);
            AddUtilityButton("🔍", "Просмотр событий", "Открыть eventvwr", OpenEventViewer);

            StatusText.Text = "Администрирование: выберите инструмент";
        }

        private void LoadUtilitiesContent()
        {
            if (ContentPanel == null) return;

            ContentPanel.Children.Clear();

            var title = CreateTitle("🛠️ Системные утилиты");
            ContentPanel.Children.Add(title);

            AddUtilityButton("🧹", "Очистка диска",                 "Запустить Disk Cleanup",                     CleanupDisk);
            AddUtilityButton("📊", "Диспетчер задач",               "Открыть Task Manager",                       OpenTaskManager);
            AddUtilityButton("🖥️", "Системная информация",          "Открыть msinfo32",                           OpenSystemInfo);
            AddUtilityButton("📁", "Редактор реестра",              "Открыть regedit",                            OpenRegistryEditor);
            AddUtilityButton("⚡", "Управление энергопитанием",     "Открыть powercfg",                           OpenPowerConfig);
            AddUtilityButton("🔧", "Службы Windows",                "Открыть services.msc",                       OpenServices);
            AddUtilityButton("💾", "Управление дисками",            "Открыть diskmgmt",                           OpenDiskManagement);
            AddUtilityButton("🌐", "Сетевые подключения",           "Открыть ncpa.cpl",                           OpenNetworkConnections);
            AddUtilityButton("💿", "Резервное копирование драйверов","Создать резервную копию на Рабочем столе",   BackupDrivers);

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
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(232, 245, 236)),
                Margin = new Thickness(0, 0, 0, 12)
            };
        }

        private void AddSectionSeparator(string sectionName)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 10, 0, 8)
            };

            var line1 = new System.Windows.Shapes.Rectangle
            {
                Height = 1,
                Width = 20,
                Fill = new SolidColorBrush(Color.FromRgb(76, 175, 120)),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 8, 0)
            };

            var sectionLabel = new TextBlock
            {
                Text = sectionName,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 120)),
                VerticalAlignment = VerticalAlignment.Center
            };

            panel.Children.Add(line1);
            panel.Children.Add(sectionLabel);
            ContentPanel.Children.Add(panel);
        }

        private void AddTweakCheckbox(string label, string tweakKey, string tooltip = "")
        {
            bool isApplied = tweakEngine.IsTweakApplied(tweakKey);

            // Карточка-строка
            var card = new Border
            {
                Background   = new SolidColorBrush(Color.FromRgb(24, 32, 25)),
                BorderBrush  = new SolidColorBrush(Color.FromRgb(36, 51, 40)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Margin       = new Thickness(0, 1, 0, 1),
                Padding      = new Thickness(8, 4, 8, 4),
                Cursor       = System.Windows.Input.Cursors.Hand
            };

            // Подсветка при наведении
            card.MouseEnter += (s, e) =>
                card.Background = new SolidColorBrush(Color.FromRgb(30, 48, 33));
            card.MouseLeave += (s, e) =>
                card.Background = new SolidColorBrush(
                    card.Tag is bool t && t
                        ? Color.FromRgb(20, 45, 28)
                        : Color.FromRgb(24, 32, 25));

            var rowPanel = new StackPanel { Orientation = Orientation.Horizontal };

            var checkbox = new CheckBox
            {
                Style       = (Style)Application.Current.FindResource("ModernCheckBox"),
                Content     = label,
                Tag         = tweakKey,
                VerticalAlignment = VerticalAlignment.Center,
                ToolTip     = string.IsNullOrEmpty(tooltip) ? null : tooltip
            };

            bool isUpdating = false;

            checkbox.Checked += (s, e) =>
            {
                if (isUpdating) return;
                tweakEngine.EnableTweak(tweakKey);
                // Жёлтая рамка — "ожидает применения"
                card.BorderBrush = new SolidColorBrush(Color.FromRgb(180, 140, 20));
                card.Background  = new SolidColorBrush(Color.FromRgb(40, 36, 15));
                card.Tag         = false;
                StatusText.Text  = $"Отмечено для применения: {label}";
            };

            checkbox.Unchecked += (s, e) =>
            {
                if (isUpdating) return;
                tweakEngine.DisableTweak(tweakKey);
                // Красноватая рамка — "ожидает отмены"
                card.BorderBrush = new SolidColorBrush(Color.FromRgb(160, 50, 45));
                card.Background  = new SolidColorBrush(Color.FromRgb(38, 22, 22));
                card.Tag         = false;
                StatusText.Text  = $"Отмечено для отмены: {label}";
            };

            isUpdating = true;
            checkbox.IsChecked = isApplied;
            isUpdating = false;

            // Начальный вид: применён → зелёная рамка
            if (isApplied)
            {
                card.BorderBrush = new SolidColorBrush(Color.FromRgb(46, 125, 79));
                card.Background  = new SolidColorBrush(Color.FromRgb(20, 45, 28));
                card.Tag         = true;
            }

            // Кликабельность по всей карточке
            card.MouseLeftButtonDown += (s, e) =>
            {
                checkbox.IsChecked = !checkbox.IsChecked;
            };

            rowPanel.Children.Add(checkbox);
            card.Child = rowPanel;
            ContentPanel.Children.Add(card);
        }

        // ═══════════════════════════════════════════════════════
        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ UI — ТЕМА
        // ═══════════════════════════════════════════════════════

        private enum ButtonKind { Add, Remove, Neutral }

        private StackPanel MakeButtonRow() =>
            new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 7, 0, 10) };

        private Border MakeActionButton(string text, ButtonKind kind, RoutedEventHandler onClick)
        {
            // Цвета по типу
            Color bgNormal, bgHover, border;
            switch (kind)
            {
                case ButtonKind.Add:
                    bgNormal = Color.FromRgb(30, 80, 48);
                    bgHover  = Color.FromRgb(38, 105, 62);
                    border   = Color.FromRgb(46, 125, 79);
                    break;
                case ButtonKind.Remove:
                    bgNormal = Color.FromRgb(80, 28, 28);
                    bgHover  = Color.FromRgb(105, 35, 35);
                    border   = Color.FromRgb(160, 50, 45);
                    break;
                default:
                    bgNormal = Color.FromRgb(24, 38, 42);
                    bgHover  = Color.FromRgb(30, 50, 56);
                    border   = Color.FromRgb(40, 85, 95);
                    break;
            }

            var card = new Border
            {
                Background      = new SolidColorBrush(bgNormal),
                BorderBrush     = new SolidColorBrush(border),
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(5),
                Padding         = new Thickness(8, 4, 8, 4),
                Margin          = new Thickness(0, 0, 6, 0),
                Cursor          = System.Windows.Input.Cursors.Hand,
                MinWidth        = 130
            };

            var label = new TextBlock
            {
                Text       = text,
                Foreground = new SolidColorBrush(Color.FromRgb(232, 245, 236)),
                FontSize   = 11,
                FontWeight = FontWeights.SemiBold,
                TextAlignment = TextAlignment.Center
            };
            card.Child = label;

            card.MouseEnter += (s, e) => card.Background = new SolidColorBrush(bgHover);
            card.MouseLeave += (s, e) => card.Background = new SolidColorBrush(bgNormal);

            // Преобразуем Border в кликабельную кнопку через MouseLeftButtonDown
            card.MouseLeftButtonDown += (s, e) => onClick?.Invoke(s, e);

            return card;
        }

        private void AddThemedSeparator()
        {
            ContentPanel.Children.Add(new Border
            {
                Height          = 1,
                Background      = new SolidColorBrush(Color.FromRgb(36, 51, 40)),
                Margin          = new Thickness(0, 10, 0, 12)
            });
        }

        private void AddSectionHeader(string text)
        {
            ContentPanel.Children.Add(new TextBlock
            {
                Text       = text,
                FontSize   = 11,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 120)),
                Margin     = new Thickness(0, 8, 0, 6)
            });
        }

        private void AddStatusBadge(bool installed, string textOn, string textOff)
        {
            var panel = new Border
            {
                Background      = installed
                    ? new SolidColorBrush(Color.FromRgb(18, 42, 26))
                    : new SolidColorBrush(Color.FromRgb(40, 20, 18)),
                BorderBrush     = installed
                    ? new SolidColorBrush(Color.FromRgb(46, 100, 65))
                    : new SolidColorBrush(Color.FromRgb(120, 40, 38)),
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(6),
                Padding         = new Thickness(10, 5, 10, 5),
                Margin          = new Thickness(0, 0, 0, 8)
            };

            var row = new StackPanel { Orientation = Orientation.Horizontal };
            row.Children.Add(new TextBlock
            {
                Text      = installed ? "✓" : "✕",
                FontSize  = 12,
                Foreground = installed
                    ? new SolidColorBrush(Color.FromRgb(76, 175, 120))
                    : new SolidColorBrush(Color.FromRgb(200, 80, 75)),
                VerticalAlignment = VerticalAlignment.Center,
                Margin    = new Thickness(0, 0, 8, 0)
            });
            row.Children.Add(new TextBlock
            {
                Text      = installed ? textOn : textOff,
                FontSize  = 12,
                FontWeight = FontWeights.SemiBold,
                Foreground = installed
                    ? new SolidColorBrush(Color.FromRgb(76, 175, 120))
                    : new SolidColorBrush(Color.FromRgb(200, 80, 75)),
                VerticalAlignment = VerticalAlignment.Center
            });

            panel.Child = row;
            ContentPanel.Children.Add(panel);
        }

        private void AddUtilityButton(string emoji, string name, string description, Action action)
        {
            var card = new Border
            {
                Background      = new SolidColorBrush(Color.FromRgb(24, 32, 25)),
                BorderBrush     = new SolidColorBrush(Color.FromRgb(36, 51, 40)),
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(5),
                Margin          = new Thickness(0, 2, 0, 2),
                Padding         = new Thickness(8, 5, 8, 5),
                Cursor          = System.Windows.Input.Cursors.Hand
            };

            var outerGrid = new Grid();
            outerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            outerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            outerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Иконка
            var iconBorder = new Border
            {
                Width           = 22,
                Height          = 22,
                Background      = new SolidColorBrush(Color.FromRgb(28, 52, 38)),
                BorderBrush     = new SolidColorBrush(Color.FromRgb(46, 100, 65)),
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(4),
                Margin          = new Thickness(0, 0, 8, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            iconBorder.Child = new TextBlock
            {
                Text                = emoji,
                FontSize            = 11,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            };

            // Текст
            var textStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            textStack.Children.Add(new TextBlock
            {
                Text       = name,
                Foreground = new SolidColorBrush(Color.FromRgb(232, 245, 236)),
                FontSize   = 11,
                FontWeight = FontWeights.SemiBold
            });
            textStack.Children.Add(new TextBlock
            {
                Text       = description,
                Foreground = new SolidColorBrush(Color.FromRgb(107, 155, 117)),
                FontSize   = 10,
                Margin     = new Thickness(0, 1, 0, 0)
            });

            // Стрелка
            var arrow = new TextBlock
            {
                Text                = "›",
                Foreground          = new SolidColorBrush(Color.FromRgb(61, 120, 85)),
                FontSize            = 13,
                VerticalAlignment   = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            Grid.SetColumn(iconBorder, 0);
            Grid.SetColumn(textStack,  1);
            Grid.SetColumn(arrow,      2);
            outerGrid.Children.Add(iconBorder);
            outerGrid.Children.Add(textStack);
            outerGrid.Children.Add(arrow);
            card.Child = outerGrid;

            card.MouseEnter += (s, e) =>
            {
                card.Background  = new SolidColorBrush(Color.FromRgb(30, 48, 33));
                card.BorderBrush = new SolidColorBrush(Color.FromRgb(46, 125, 79));
                arrow.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 120));
            };
            card.MouseLeave += (s, e) =>
            {
                card.Background  = new SolidColorBrush(Color.FromRgb(24, 32, 25));
                card.BorderBrush = new SolidColorBrush(Color.FromRgb(36, 51, 40));
                arrow.Foreground = new SolidColorBrush(Color.FromRgb(61, 120, 85));
            };
            card.MouseLeftButtonDown += (s, e) => action?.Invoke();

            ContentPanel.Children.Add(card);
        }

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
                "WindowsTweaks Pro Edition v3.0.1\n\n" +
                "Профессиональный инструмент для оптимизации\n" +
                "и настройки операционной системы Windows.\n\n" +
                "ОСНОВНЫЕ ВОЗМОЖНОСТИ:\n" +
                "• 80+ твиков для оптимизации системы\n" +
                "• Оптимизация производительности и питания\n" +
                "• Настройка конфиденциальности\n" +
                "• Управление службами Windows\n" +
                "• Мгновенное применение и отмена твиков\n" +
                "• Контекстные меню «Этот компьютер» и Рабочего стола\n\n" +
                "НОВОЕ В v3.0.1:\n" +
                "• +16 новых твиков (игры, приватность, сеть, UI)\n" +
                "• Отложенный запуск служб\n" +
                "• Восстановление CMD в контекстном меню\n" +
                "• Подсказки для каждого твика\n\n" +
                "Разработчик: Виталий Николаевич (vitalikkontr)\n" +
                "Версия: 3.0.1  |  23.02.2026  |  © 2026 WindowsTweaks Pro",
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

        private void AddContextMenuItems_Click(object sender, RoutedEventArgs e)
        {
            bool result = ThemedDialog.Confirm(
                "Добавить системные инструменты в контекстное меню \"Этот компьютер\"?\n\n" +
                "Будут добавлены следующие пункты:\n" +
                "• Администрирование\n• Панель управления\n• Диспетчер устройств\n" +
                "• Управление дисками\n• Редактор групповой политики\n• Программы и компоненты\n" +
                "• Редактор реестра\n• Безопасный режим (с подменю)\n• Службы", "Добавление системных инструментов", DialogIcon.Question, this);
            if (result)
            {
                try
                {
                    StatusText.Text = "Добавление пунктов в контекстное меню...";
                    string addResult = ComputerContextMenu.AddSystemTools();
                    ThemedDialog.Show(addResult, "Результат добавления", DialogIcon.Info, this);
                    LoadAdministrationContent();
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка добавления пунктов меню";
                    ThemedDialog.Show($"Не удалось добавить пункты в контекстное меню:\n\n{ex.Message}", "Ошибка", DialogIcon.Error, this);
                }
            }
        }

        private void RemoveContextMenuItems_Click(object sender, RoutedEventArgs e)
        {
            bool result = ThemedDialog.Confirm("Удалить системные инструменты из контекстного меню \"Этот компьютер\"?\n\n" +
                "Это действие можно отменить, снова добавив пункты через эту программу.",
                "Удаление системных инструментов", DialogIcon.Question, this);
            if (result)
            {
                try
                {
                    StatusText.Text = "Удаление пунктов из контекстного меню...";
                    string removeResult = ComputerContextMenu.RemoveSystemTools();
                    ThemedDialog.Show(removeResult, "Результат удаления", DialogIcon.Info, this);
                    LoadAdministrationContent();
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка удаления пунктов меню";
                    ThemedDialog.Show($"Не удалось удалить пункты из контекстного меню:\n\n{ex.Message}", "Ошибка", DialogIcon.Error, this);
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
                ThemedDialog.Show($"Ошибка диагностики:\n{ex.Message}", "Ошибка", DialogIcon.Error, this);
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // ОБРАБОТЧИКИ КОНТЕКСТНОГО МЕНЮ РАБОЧЕГО СТОЛА
        // ═══════════════════════════════════════════════════════════════════

        private void AddDesktopContextMenuItems_Click(object sender, RoutedEventArgs e)
        {
            bool result = ThemedDialog.Confirm(
                "Добавить системные инструменты в контекстное меню рабочего стола?\n\n" +
                "📋 БУДУТ ДОБАВЛЕНЫ:\n\nОсновные инструменты:\n" +
                "• Администрирование\n• Указатели мыши\n• Свойства папки\n" +
                "• Сетевые подключения\n• Программы и компоненты\n" +
                "• Редактор реестра\n• Диспетчер задач\n\n" +
                "Подменю:\n• Персонализация+\n• Панель настроек", "Добавление в контекстное меню рабочего стола", DialogIcon.Question, this);
            if (result)
            {
                try
                {
                    StatusText.Text = "Добавление пунктов в контекстное меню рабочего стола...";
                    string addResult = DesktopContextMenu.AddDesktopTools();
                    ThemedDialog.Show(addResult, "Результат добавления", DialogIcon.Info, this);
                    LoadAdministrationContent();
                    StatusText.Text = "✅ Инструменты успешно добавлены в меню рабочего стола";
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка добавления пунктов в меню";
                    ThemedDialog.Show($"Не удалось добавить пункты:\n\n{ex.Message}", "Ошибка", DialogIcon.Error, this);
                }
            }
        }

        private void RemoveDesktopContextMenuItems_Click(object sender, RoutedEventArgs e)
        {
            bool result = ThemedDialog.Confirm("Удалить системные инструменты из контекстного меню рабочего стола?\n\n" +
                "Это действие можно отменить, снова добавив пункты через эту программу.",
                "Удаление из контекстного меню", DialogIcon.Question, this);
            if (result)
            {
                try
                {
                    StatusText.Text = "Удаление пунктов из контекстного меню рабочего стола...";
                    string removeResult = DesktopContextMenu.RemoveDesktopTools();
                    ThemedDialog.Show(removeResult, "Результат удаления", DialogIcon.Info, this);
                    LoadAdministrationContent();
                    StatusText.Text = "✅ Инструменты успешно удалены из меню рабочего стола";
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка удаления пунктов из меню";
                    ThemedDialog.Show($"Не удалось удалить пункты:\n\n{ex.Message}", "Ошибка", DialogIcon.Error, this);
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
                ThemedDialog.Show($"Ошибка диагностики:\n{ex.Message}", "Ошибка", DialogIcon.Error, this);
            }
        }

        private void ShowDiagnosticWindow(string title, string content)
        {
            var win = new Window
            {
                Title                 = title,
                Width                 = 560,
                Height                = 480,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner                 = this,
                WindowStyle           = WindowStyle.None,
                AllowsTransparency    = true,
                Background            = Brushes.Transparent,
                ResizeMode            = ResizeMode.CanResizeWithGrip
            };

            // Внешний контейнер с тенью и скруглением
            var root = new Border
            {
                Background      = new SolidColorBrush(Color.FromRgb(18, 26, 20)),
                BorderBrush     = new SolidColorBrush(Color.FromRgb(36, 51, 40)),
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(12)
            };
            root.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color       = Colors.Black,
                BlurRadius  = 32,
                ShadowDepth = 0,
                Opacity     = 0.80
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // ── Шапка ──
            var header = new Border
            {
                Background      = new SolidColorBrush(Color.FromRgb(24, 32, 25)),
                BorderBrush     = new SolidColorBrush(Color.FromRgb(36, 51, 40)),
                BorderThickness = new Thickness(0, 0, 0, 1),
                CornerRadius    = new CornerRadius(12, 12, 0, 0),
                Padding         = new Thickness(16, 12, 12, 12)
            };
            header.MouseLeftButtonDown += (s, e) => win.DragMove();

            var headerRow = new Grid();
            headerRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(28) });

            var titleText = new TextBlock
            {
                Text                = title,
                Foreground          = new SolidColorBrush(Color.FromRgb(232, 245, 236)),
                FontSize            = 13,
                FontWeight          = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            };
            Grid.SetColumn(titleText, 0);

            var closeBtn = new Border
            {
                Width             = 28,
                Height            = 28,
                Background        = Brushes.Transparent,
                CornerRadius      = new CornerRadius(5),
                Cursor            = System.Windows.Input.Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            var closeTxt = new TextBlock
            {
                Text                = "✕",
                Foreground          = new SolidColorBrush(Color.FromRgb(107, 155, 117)),
                FontSize            = 13,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            };
            closeBtn.Child = closeTxt;
            closeBtn.MouseEnter += (s, e) => closeBtn.Background = new SolidColorBrush(Color.FromRgb(139, 32, 32));
            closeBtn.MouseLeave += (s, e) => closeBtn.Background = Brushes.Transparent;
            closeBtn.MouseLeftButtonDown += (s, e) => win.Close();
            Grid.SetColumn(closeBtn, 1);

            headerRow.Children.Add(titleText);
            headerRow.Children.Add(closeBtn);
            header.Child = headerRow;
            Grid.SetRow(header, 0);

            // ── Контент ──
            var scroll = new ScrollViewer
            {
                VerticalScrollBarVisibility   = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                HorizontalContentAlignment    = HorizontalAlignment.Stretch
            };

            var outerPad = new Border
            {
                Padding    = new Thickness(32, 16, 32, 16),
                Background = Brushes.Transparent
            };

            var text = new TextBlock
            {
                Text          = content,
                Foreground    = new SolidColorBrush(Color.FromRgb(168, 196, 174)),
                FontFamily    = new FontFamily("Consolas"),
                FontSize      = 12,
                LineHeight    = 24,
                TextWrapping  = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Left
            };

            outerPad.Child = text;
            scroll.Content = outerPad;
            Grid.SetRow(scroll, 1);

            // ── Нижняя кнопка ──
            var footer = new Border
            {
                Background      = new SolidColorBrush(Color.FromRgb(24, 32, 25)),
                BorderBrush     = new SolidColorBrush(Color.FromRgb(36, 51, 40)),
                BorderThickness = new Thickness(0, 1, 0, 0),
                CornerRadius    = new CornerRadius(0, 0, 12, 12),
                Padding         = new Thickness(16, 12, 16, 14)
            };

            var okBtn = new Border
            {
                Background      = new SolidColorBrush(Color.FromRgb(30, 80, 48)),
                BorderBrush     = new SolidColorBrush(Color.FromRgb(46, 125, 79)),
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(8),
                Padding         = new Thickness(24, 7, 24, 7),
                Cursor          = System.Windows.Input.Cursors.Hand,
                HorizontalAlignment = HorizontalAlignment.Center  // ← по центру
            };
            okBtn.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color       = Color.FromRgb(76, 175, 120),
                BlurRadius  = 10,
                ShadowDepth = 0,
                Opacity     = 0.20
            };
            var okTxt = new TextBlock
            {
                Text                = "Закрыть",
                Foreground          = new SolidColorBrush(Color.FromRgb(232, 245, 236)),
                FontSize            = 13,
                FontWeight          = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            };
            okBtn.Child = okTxt;
            okBtn.MouseEnter += (s, e) => okBtn.Background = new SolidColorBrush(Color.FromRgb(38, 105, 62));
            okBtn.MouseLeave += (s, e) => okBtn.Background = new SolidColorBrush(Color.FromRgb(30, 80, 48));
            okBtn.MouseLeftButtonDown += (s, e) => win.Close();

            footer.Child = okBtn;
            Grid.SetRow(footer, 2);

            grid.Children.Add(header);
            grid.Children.Add(scroll);
            grid.Children.Add(footer);
            root.Child = grid;
            win.Content = root;
            win.ShowDialog();
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
