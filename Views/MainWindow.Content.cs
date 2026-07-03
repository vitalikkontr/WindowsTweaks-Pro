using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindowsTweaks
{
    public partial class MainWindow
    {
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
            // Наш новый твик содержимого папок
            AddTweakCheckbox("Добавить Пункт <<Удалить содержимое папки>>", "EmptyFolderMenu",
                "Добавляет в контекстное меню папок команду быстрой очистки всех файлов и подпапок без удаления самой папки");
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

            AddTweakCheckbox("Включить классический Просмотр фотографий", "EnableClassicPhotoViewer",
                "Разблокировать классический «Просмотр фотографий Windows»");
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

            var title = CreateTitle("👨‍💼 Администрирование");
            ContentPanel.Children.Add(title);

            // ─── Твик CMD ───
            AddTweakCheckbox("Восстановить запуск CMD из папки", "RestoreCmdHereContext",
                "Возвращает пункт «Открыть окно команд здесь» в контекстное меню папок");

            AddSectionSeparator("🖱️ Контекстное меню «Этот компьютер»");

            // Статус + кнопки в одну строку
            AddCompactContextMenuBlock(
                ComputerContextMenu.AreToolsInstalled(),
                AddContextMenuItems_Click,
                RemoveContextMenuItems_Click,
                DiagnosticContextMenu_Click
            );

            AddSectionSeparator("🖥 Контекстное меню «Рабочего стола»");

            AddCompactContextMenuBlock(
                DesktopContextMenu.AreDesktopToolsInstalled(),
                AddDesktopContextMenuItems_Click,
                RemoveDesktopContextMenuItems_Click,
                DiagnosticDesktopContextMenu_Click
            );

            AddSectionSeparator("🚀 Быстрый запуск");

            // Сетка 3 колонки
            var grid = new System.Windows.Controls.WrapPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 2, 0, 0)
            };

            grid.Children.Add(MakeQuickTile("🖥️", "Администрирование",    OpenAdministration));
            grid.Children.Add(MakeQuickTile("🛡️", "Безопасный режим",     OpenSafeMode));
            grid.Children.Add(MakeQuickTile("🔌", "Диспетчер устройств",  OpenDeviceManager));
            grid.Children.Add(MakeQuickTile("⚙️", "Панель управления",    OpenControlPanel));
            grid.Children.Add(MakeQuickTile("📦", "Программы",            OpenProgramsAndFeatures));
            grid.Children.Add(MakeQuickTile("📋", "Групп. политика",      OpenGroupPolicy));
            grid.Children.Add(MakeQuickTile("🔧", "Службы",               OpenServices));
            grid.Children.Add(MakeQuickTile("💾", "Управл. дисками",      OpenDiskManagement));
            grid.Children.Add(MakeQuickTile("👤", "Управл. компьютером",  OpenComputerManagement));
            grid.Children.Add(MakeQuickTile("🌐", "Сеть",                 OpenNetworkConnections));
            grid.Children.Add(MakeQuickTile("📊", "Монитор ресурсов",     OpenResourceMonitor));
            grid.Children.Add(MakeQuickTile("🔍", "Просмотр событий",     OpenEventViewer));

            ContentPanel.Children.Add(grid);

            StatusText.Text = "Администрирование: выберите инструмент";
        }

        // Компактный блок статус + 3 кнопки
        private void AddCompactContextMenuBlock(
            bool installed,
            RoutedEventHandler onAdd,
            RoutedEventHandler onRemove,
            RoutedEventHandler onDiag)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 2, 0, 6)
            };

            // Индикатор статуса
            var badge = new Border
            {
                Background      = installed
                    ? new SolidColorBrush(Color.FromRgb(18, 42, 26))
                    : new SolidColorBrush(Color.FromRgb(40, 20, 18)),
                BorderBrush     = installed
                    ? new SolidColorBrush(Color.FromRgb(46, 100, 65))
                    : new SolidColorBrush(Color.FromRgb(120, 40, 38)),
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(5),
                Padding         = new Thickness(8, 4, 8, 4),
                Margin          = new Thickness(0, 0, 6, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            var badgeRow = new StackPanel { Orientation = Orientation.Horizontal };
            badgeRow.Children.Add(new TextBlock
            {
                Text      = installed ? "✓" : "✕",
                FontSize  = 11,
                Foreground = installed
                    ? new SolidColorBrush(Color.FromRgb(76, 175, 120))
                    : new SolidColorBrush(Color.FromRgb(200, 80, 75)),
                VerticalAlignment = VerticalAlignment.Center,
                Margin    = new Thickness(0, 0, 5, 0)
            });
            badgeRow.Children.Add(new TextBlock
            {
                Text      = installed ? "Установлено" : "Не установлено",
                FontSize  = 10,
                FontWeight = FontWeights.SemiBold,
                Foreground = installed
                    ? new SolidColorBrush(Color.FromRgb(76, 175, 120))
                    : new SolidColorBrush(Color.FromRgb(200, 80, 75)),
                VerticalAlignment = VerticalAlignment.Center
            });
            badge.Child = badgeRow;
            panel.Children.Add(badge);

            panel.Children.Add(MakeActionButton("+ Добавить",   ButtonKind.Add,     onAdd));
            panel.Children.Add(MakeActionButton("🗑 Удалить",    ButtonKind.Remove,  onRemove));
            panel.Children.Add(MakeActionButton("🔍 Диагностика", ButtonKind.Neutral, onDiag));

            ContentPanel.Children.Add(panel);
        }

        // Плитка быстрого запуска
        private Border MakeQuickTile(string emoji, string label, Action action)
        {
            var tile = new Border
            {
                Width           = 118,
                Height          = 52,
                Background      = new SolidColorBrush(Color.FromRgb(24, 32, 25)),
                BorderBrush     = new SolidColorBrush(Color.FromRgb(36, 51, 40)),
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(6),
                Margin          = new Thickness(0, 0, 5, 5),
                Cursor          = System.Windows.Input.Cursors.Hand
            };

            var inner = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            };
            inner.Children.Add(new TextBlock
            {
                Text                = emoji,
                FontSize            = 16,
                Foreground          = new SolidColorBrush(Color.FromRgb(232, 245, 236)),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin              = new Thickness(0, 0, 0, 2)
            });
            inner.Children.Add(new TextBlock
            {
                Text                = label,
                FontSize            = 9,
                Foreground          = new SolidColorBrush(Color.FromRgb(232, 245, 236)),
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment       = TextAlignment.Center,
                TextWrapping        = TextWrapping.Wrap,
                MaxWidth            = 106
            });
            tile.Child = inner;

            tile.MouseEnter += (s, e) =>
            {
                tile.Background  = new SolidColorBrush(Color.FromRgb(30, 48, 33));
                tile.BorderBrush = new SolidColorBrush(Color.FromRgb(76, 175, 120));
            };
            tile.MouseLeave += (s, e) =>
            {
                tile.Background  = new SolidColorBrush(Color.FromRgb(24, 32, 25));
                tile.BorderBrush = new SolidColorBrush(Color.FromRgb(36, 51, 40));
            };
            tile.MouseLeftButtonDown += (s, e) => action?.Invoke();

            return tile;
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
            
            AddUtilityButton("💿", "Резервное копирование драйверов","Создать резервную копию на Рабочем столе",   BackupDrivers);

            StatusText.Text = "Утилиты: выберите действие";
        }

        // ═══════════════════════════════════════════════════════
        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ UI
        // ═══════════════════════════════════════════════════════

    }
}
