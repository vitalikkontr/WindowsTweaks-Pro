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

            // Загружаем контент после инициализации UI
            LoadPerformanceContent();
        }

        private void NavigationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contentLoaders == null || NavigationList.SelectedIndex < 0)
                return;

            if (contentLoaders.ContainsKey(NavigationList.SelectedIndex))
            {
                contentLoaders[NavigationList.SelectedIndex]();
            }
        }

        private void LoadPerformanceContent()
        {
            if (ContentPanel == null) return;

            ContentPanel.Children.Clear();

            var title = CreateTitle("⚡ Оптимизация производительности");
            ContentPanel.Children.Add(title);

            AddTweakCheckbox("Отключить визуальные эффекты Windows", "DisableVisualEffects");
            AddTweakCheckbox("Отключить индексирование поиска", "DisableSearchIndexing");
            AddTweakCheckbox("Отключить SuperFetch/Prefetch", "DisableSuperfetch");
            AddTweakCheckbox("Оптимизировать файл подкачки", "OptimizePageFile");
            AddTweakCheckbox("Отключить спящий режим (hiberfil.sys)", "DisableHibernation");
            AddTweakCheckbox("Отключить дефрагментацию по расписанию", "DisableScheduledDefrag");
            AddTweakCheckbox("Увеличить кэш DNS", "IncreaseDNSCache");
            AddTweakCheckbox("Отключить Windows Defender (требует осторожности!)", "DisableDefender");

            StatusText.Text = "Производительность: готов к настройке";
        }

        private void LoadPrivacyContent()
        {
            if (ContentPanel == null) return;

            ContentPanel.Children.Clear();

            var title = CreateTitle("🔒 Конфиденциальность и телеметрия");
            ContentPanel.Children.Add(title);

            AddTweakCheckbox("Отключить телеметрию Windows", "DisableTelemetry");
            AddTweakCheckbox("Отключить рекламу в меню Пуск", "DisableStartMenuAds");
            AddTweakCheckbox("Отключить Cortana", "DisableCortana");
            AddTweakCheckbox("Отключить отслеживание местоположения", "DisableLocationTracking");
            AddTweakCheckbox("Отключить советы Windows", "DisableWindowsTips");
            AddTweakCheckbox("Отключить рекламный ID", "DisableAdvertisingID");
            AddTweakCheckbox("Блокировать сбор диагностических данных", "BlockDiagnosticData");
            AddTweakCheckbox("Отключить облачную синхронизацию", "DisableCloudSync");

            StatusText.Text = "Конфиденциальность: готов к настройке";
        }

        private void LoadNetworkContent()
        {
            if (ContentPanel == null) return;

            ContentPanel.Children.Clear();

            var title = CreateTitle("🌐 Сетевые настройки");
            ContentPanel.Children.Add(title);

            AddTweakCheckbox("Отключить IPv6", "DisableIPv6");
            AddTweakCheckbox("Оптимизировать TCP/IP", "OptimizeTCPIP");
            AddTweakCheckbox("Очистить кэш DNS", "FlushDNSCache");
            AddTweakCheckbox("Сбросить сетевые адаптеры", "ResetNetworkAdapters");
            AddTweakCheckbox("Отключить лимитированное подключение", "DisableMeteredConnection");
            AddTweakCheckbox("Оптимизировать настройки QoS", "OptimizeQoS");

            StatusText.Text = "Сеть: готов к настройке";
        }

        private void LoadAppearanceContent()
        {
            if (ContentPanel == null) return;

            ContentPanel.Children.Clear();

            var title = CreateTitle("🎨 Внешний вид и персонализация");
            ContentPanel.Children.Add(title);

            AddTweakCheckbox("Включить темную тему", "EnableDarkTheme");
            AddTweakCheckbox("Показывать расширения файлов", "ShowFileExtensions");
            AddTweakCheckbox("Показывать скрытые файлы", "ShowHiddenFiles");
            AddTweakCheckbox("Классический контекстное меню (Win11)", "ClassicContextMenu");
            AddTweakCheckbox("Отключить группировку на панели задач", "DisableTaskbarGrouping");
            AddTweakCheckbox("Мелкие значки на панели задач", "SmallTaskbarIcons");
            AddTweakCheckbox("Убрать виджеты с панели задач (Win11)", "RemoveTaskbarWidgets");

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

            AddTweakCheckbox("Отключить Windows Update (осторожно!)", "DisableWindowsUpdate");
            AddTweakCheckbox("Отключить Windows Search", "DisableWindowsSearch");
            AddTweakCheckbox("Отключить печать (Print Spooler)", "DisablePrintSpooler");
            AddTweakCheckbox("Отключить факс", "DisableFax");
            AddTweakCheckbox("Отключить Bluetooth", "DisableBluetooth");
            AddTweakCheckbox("Отключить диагностику", "DisableDiagnostic");

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
                Margin = new Thickness(0, 0, 0, 20),
                TextWrapping = TextWrapping.Wrap
            };
            ContentPanel.Children.Add(description);

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

            // Показываем статус установки для "Этот компьютер"
            var statusPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 15)
            };

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
                Foreground = ComputerContextMenu.AreToolsInstalled()
                    ? new SolidColorBrush(Color.FromRgb(76, 175, 80))
                    : new SolidColorBrush(Color.FromRgb(244, 67, 54)),
                FontWeight = FontWeights.Bold
            };

            statusPanel.Children.Add(statusIcon);
            statusPanel.Children.Add(statusText);
            ContentPanel.Children.Add(statusPanel);

            // Панель кнопок для контекстного меню "Этот компьютер"
            var menuButtonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 15)
            };

            var addMenuButton = new Button
            {
                Content = "➕ Добавить системные инструменты",
                Width = 280,
                Height = 40,
                Margin = new Thickness(0, 0, 15, 0),
                Background = new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                Foreground = Brushes.White,
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Cursor = System.Windows.Input.Cursors.Hand
            };
            addMenuButton.Click += AddContextMenuItems_Click;

            var removeMenuButton = new Button
            {
                Content = "🗑️ Удалить системные инструменты",
                Width = 280,
                Height = 40,
                Margin = new Thickness(0, 0, 15, 0),
                Background = new SolidColorBrush(Color.FromRgb(244, 67, 54)),
                Foreground = Brushes.White,
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Cursor = System.Windows.Input.Cursors.Hand
            };
            removeMenuButton.Click += RemoveContextMenuItems_Click;

            var diagnosticButton = new Button
            {
                Content = "🔍 Диагностика меню Этот компьютер",
                Width = 280,
                Height = 40,
                Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                Foreground = Brushes.White,
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Cursor = System.Windows.Input.Cursors.Hand
            };
            diagnosticButton.Click += DiagnosticContextMenu_Click;

            menuButtonsPanel.Children.Add(addMenuButton);
            menuButtonsPanel.Children.Add(removeMenuButton);
            menuButtonsPanel.Children.Add(diagnosticButton);
            ContentPanel.Children.Add(menuButtonsPanel);

            // Разделитель
            var separator = new System.Windows.Controls.Separator
            {
                Margin = new Thickness(0, 10, 0, 20),
                Background = new SolidColorBrush(Color.FromRgb(60, 60, 60))
            };
            ContentPanel.Children.Add(separator);

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

            // Статус установки для меню рабочего стола
            var desktopStatusPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 15)
            };

            var desktopStatusIcon = new TextBlock
            {
                FontSize = 16,
                Margin = new Thickness(0, 0, 10, 0)
            };

            var desktopStatusText = new TextBlock
            {
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center
            };

            bool desktopToolsInstalled = DesktopContextMenu.AreDesktopToolsInstalled();
            if (desktopToolsInstalled)
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

            // Кнопки управления для рабочего стола
            var desktopButtonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 10, 0, 20)
            };

            var addDesktopButton = new Button
            {
                Content = "➕ Добавить в меню Рабочего Стола",
                Width = 280,
                Height = 40,
                Margin = new Thickness(0, 0, 15, 0),
                Background = new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                FontSize = 13,
                FontWeight = FontWeights.SemiBold
            };
            addDesktopButton.Click += AddDesktopContextMenuItems_Click;

            var removeDesktopButton = new Button
            {
                Content = "🗑️ Удалить из меню Рабочего Стола",
                Width = 280,
                Height = 40,
                Margin = new Thickness(0, 0, 15, 0),
                Background = new SolidColorBrush(Color.FromRgb(244, 67, 54)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                FontSize = 13,
                FontWeight = FontWeights.SemiBold
            };
            removeDesktopButton.Click += RemoveDesktopContextMenuItems_Click;

            var diagnosticDesktopButton = new Button
            {
                Content = "🔍 Диагностика меню Рабочего Стола",
                Width = 280,
                Height = 40,
                Margin = new Thickness(0, 0, 0, 0),
                Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                FontSize = 13,
                FontWeight = FontWeights.SemiBold
            };
            diagnosticDesktopButton.Click += DiagnosticDesktopContextMenu_Click;

            desktopButtonsPanel.Children.Add(addDesktopButton);
            desktopButtonsPanel.Children.Add(removeDesktopButton);
            desktopButtonsPanel.Children.Add(diagnosticDesktopButton);
            ContentPanel.Children.Add(desktopButtonsPanel);

            // Разделитель
            var separator2 = new System.Windows.Controls.Separator
            {
                Margin = new Thickness(0, 20, 0, 20),
                Background = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                Height = 2
            };
            ContentPanel.Children.Add(separator2);

            // Заголовок для быстрого запуска
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

        private void AddTweakCheckbox(string label, string tweakKey)
        {
            // Проверяем, применен ли твик
            bool isApplied = tweakEngine.IsTweakApplied(tweakKey);
            
            // Создаем горизонтальную панель для размещения индикатора и чекбокса
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 8, 0, 8)
            };
            
            // Индикатор статуса (зеленая галочка если применено)
            var statusIcon = new TextBlock
            {
                Text = isApplied ? "✅" : "⬜",
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 8, 0),
                ToolTip = isApplied ? "Твик применен" : "Твик не применен"
            };
            
            // Чекбокс
            var checkbox = new CheckBox
            {
                Content = label,
                FontSize = 14,
                Foreground = Brushes.White,
                Tag = tweakKey,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            // Если твик применен, делаем текст зеленым
            if (isApplied)
            {
                checkbox.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Зеленый
            }

            // Флаг для отслеживания программного изменения
            bool isUpdating = false;

            // ✨ Применяем/отменяем СРАЗУ при клике пользователя
            checkbox.Checked += async (s, e) =>
            {
                // Игнорируем программные изменения
                if (isUpdating) return;
                
                StatusText.Text = $"⏳ Применяется: {label}...";
                
                try
                {
                    // Применяем твик СРАЗУ
                    tweakEngine.EnableTweak(tweakKey);
                    await tweakEngine.ApplySelectedTweakAsync(tweakKey);
                    
                    // Обновляем индикатор
                    statusIcon.Text = "✅";
                    statusIcon.ToolTip = "Твик применен";
                    checkbox.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                    
                    StatusText.Text = $"✅ Применено: {label}";
                }
                catch (UnauthorizedAccessException)
                {
                    isUpdating = true;
                    checkbox.IsChecked = false;
                    isUpdating = false;
                    
                    StatusText.Text = "❌ Требуются права администратора!";
                    MessageBox.Show(
                        $"Для применения твика \"{label}\" требуются права администратора.\n\n" +
                        "Запустите программу от имени администратора.",
                        "Недостаточно прав",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    isUpdating = true;
                    checkbox.IsChecked = false;
                    isUpdating = false;
                    
                    StatusText.Text = $"❌ Ошибка: {ex.Message}";
                    MessageBox.Show(
                        $"Ошибка применения твика:\n{ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            };

            checkbox.Unchecked += async (s, e) =>
            {
                // Игнорируем программные изменения
                if (isUpdating) return;
                
                StatusText.Text = $"⏳ Отменяется: {label}...";
                
                try
                {
                    // Отменяем твик СРАЗУ
                    tweakEngine.DisableTweak(tweakKey);
                    await tweakEngine.RevertSelectedTweakAsync(tweakKey);
                    
                    // Обновляем индикатор
                    statusIcon.Text = "⬜";
                    statusIcon.ToolTip = "Твик не применен";
                    checkbox.Foreground = Brushes.White;
                    
                    StatusText.Text = $"↩️ Отменено: {label}";
                }
                catch (Exception ex)
                {
                    isUpdating = true;
                    checkbox.IsChecked = true;
                    isUpdating = false;
                    
                    StatusText.Text = $"❌ Ошибка отмены: {ex.Message}";
                    MessageBox.Show(
                        $"Ошибка отмены твика:\n{ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            };

            // ВАЖНО: Устанавливаем галочку ПОСЛЕ добавления обработчиков
            // Используем флаг, чтобы не вызывать применение
            isUpdating = true;
            checkbox.IsChecked = isApplied;
            isUpdating = false;

            // Собираем элементы
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
                Background = new SolidColorBrush(Color.FromRgb(66, 165, 245)), // Светло-голубой (немного темнее чем на скрине)
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Padding = new Thickness(15, 8, 15, 8)
            };

            // Лёгкая подсветка при наведении (чуть светлее)
            button.MouseEnter += (s, e) =>
            {
                button.Background = new SolidColorBrush(Color.FromRgb(100, 181, 246)); // Немного светлее
            };

            button.MouseLeave += (s, e) =>
            {
                button.Background = new SolidColorBrush(Color.FromRgb(66, 165, 245)); // Возврат к базовому
            };

            button.Click += (s, e) => action?.Invoke();

            ContentPanel.Children.Add(button);
        }

        private async void ApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы уверены, что хотите применить выбранные изменения?\n\n" +
                "Рекомендуется создать точку восстановления перед применением.",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                StatusText.Text = "Применение изменений...";

                try
                {
                    await tweakEngine.ApplyAllTweaksAsync();
                    StatusText.Text = "Изменения успешно применены!";

                    MessageBox.Show(
                        "Изменения успешно применены!\n\n" +
                        "Некоторые изменения могут потребовать перезагрузки системы.",
                        "Успешно",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    
                    // Обновляем индикаторы после применения
                    RefreshAllCheckboxes();
                }
                catch (Exception ex)
                {
                    StatusText.Text = "Ошибка при применении изменений";
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
                "2️⃣ Отметьте нужные твики галочками\n\n" +
                "3️⃣ Нажмите кнопку 'Применить изменения'\n\n" +
                "4️⃣ Дождитесь завершения операции\n\n" +
                "⚠️ ВАЖНЫЕ РЕКОМЕНДАЦИИ:\n\n" +
                "• Создавайте точку восстановления системы\n" +
                "  перед применением изменений!\n\n" +
                "• Некоторые изменения требуют перезагрузки\n\n" +
                "🎯 ДОБАВЛЕНИЕ ПУНКТОВ В МЕНЮ:\n\n" +
                "Раздел 'Инструменты администрирования' позволяет\n" +
                "добавить системные утилиты в контекстные меню:\n" +
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
                "║   WindowsTweaks Pro Edition v2.4            ║\n" +
                "╚═════════════════════════════════════════════╝\n\n" +
                "🎯 Профессиональный инструмент для оптимизации\n" +
                "   и настройки операционной системы Windows\n\n" +
                "✨ ОСНОВНЫЕ ВОЗМОЖНОСТИ:\n" +
                "   • Оптимизация производительности\n" +
                "   • Настройка конфиденциальности\n" +
                "   • Управление службами Windows\n" +
                "   • Контекстное меню \"Этот компьютер\"\n" +
                "     (8 инструментов + Безопасный режим)\n" +
                "   • Контекстное меню рабочего стола\n" +
                "     (11 инструментов + 2 подменю)\n\n" +
                "👤 Разработчик:\n" +
                "   Виталий Николаевич (vitalikkontr)\n\n" +
                "📅 Дата сборки: 16.02.2026\n\n" +
                "© 2026 WindowsTweaks Pro Edition\n" +
                "Все права защищены.",
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
                // Путь к папке на рабочем столе
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string backupFolder = System.IO.Path.Combine(desktopPath, "DriverBackup");
                
                // Создаём папку если её нет
                if (!System.IO.Directory.Exists(backupFolder))
                {
                    System.IO.Directory.CreateDirectory(backupFolder);
                }

                // Создаём bat файл для установки драйверов
                string installBatPath = System.IO.Path.Combine(backupFolder, "Install-all-drivers.bat");
                string installBatContent = @"@echo off
pnputil /add-driver *.inf /install /subdirs

echo.
echo Finished.
echo.
echo Reboot after pressing button.
echo.

shutdown /r /t 3
";
                System.IO.File.WriteAllText(installBatPath, installBatContent);

                // Запускаем экспорт драйверов через DISM с правами администратора
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c dism /online /export-driver /destination:\"{backupFolder}\"",
                    UseShellExecute = true,
                    Verb = "runas", // Запрос прав администратора
                    CreateNoWindow = false,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
                };

                var process = System.Diagnostics.Process.Start(psi);
                
                if (process != null)
                {
                    StatusText.Text = "⏳ Резервное копирование драйверов... Дождитесь завершения.";
                    
                    // Запускаем асинхронное ожидание завершения
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        process.WaitForExit();
                        
                        // Обновляем UI в главном потоке
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
                                
                                // Открываем папку с бэкапом
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
                // Пользователь отклонил UAC
                StatusText.Text = "❌ Требуются права администратора для резервного копирования драйверов";
                MessageBox.Show(
                    "Для резервного копирования драйверов требуются права администратора.\n\n" +
                    "Подтвердите запрос UAC для продолжения.",
                    "Требуются права администратора",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                StatusText.Text = "❌ Ошибка при резервном копировании драйверов";
                MessageBox.Show(
                    $"Произошла ошибка:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
            try
            {
                StartMmc("gpedit.msc");
            }
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
                "• Администрирование\n" +
                "• Панель управления\n" +
                "• Диспетчер устройств\n" +
                "• Управление дисками\n" +
                "• Редактор групповой политики\n" +
                "• Программы и компоненты\n" +
                "• Редактор реестра\n" +
                "• Безопасный режим (с подменю)\n" +
                "• Службы\n\n" +
                "Для доступа к пунктам нажмите ПКМ на \"Этот компьютер\"",
                "Добавление системных инструментов",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    StatusText.Text = "Добавление пунктов в контекстное меню...";
                    string addResult = ComputerContextMenu.AddSystemTools();

                    MessageBox.Show(
                        addResult,
                        "Результат добавления",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    LoadAdministrationContent();
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка добавления пунктов меню";
                    MessageBox.Show(
                        $"Не удалось добавить пункты в контекстное меню:\n\n{ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
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

                    MessageBox.Show(
                        removeResult,
                        "Результат удаления",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    LoadAdministrationContent();
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка удаления пунктов меню";
                    MessageBox.Show(
                        $"Не удалось удалить пункты из контекстного меню:\n\n{ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void DiagnosticContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string diagnostic = ComputerContextMenu.GetDiagnosticInfo();

                var diagnosticWindow = new Window
                {
                    Title = "Диагностика контекстного меню",
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
                    Text = diagnostic,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap
                };

                scrollViewer.Content = textBlock;
                diagnosticWindow.Content = scrollViewer;
                diagnosticWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка диагностики:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // ОБРАБОТЧИКИ КОНТЕКСТНОГО МЕНЮ РАБОЧЕГО СТОЛА
        // ═══════════════════════════════════════════════════════════════════

        private void AddDesktopContextMenuItems_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Добавить системные инструменты в контекстное меню рабочего стола?\n\n" +
                "📋 БУДУТ ДОБАВЛЕНЫ:\n\n" +
                "Основные инструменты:\n" +
                "• Администрирование\n" +
                "• Указатели мыши\n" +
                "• Свойства папки\n" +
                "• Сетевые подключения\n" +
                "• Программы и компоненты\n" +
                "• Редактор реестра\n" +
                "• Диспетчер задач\n\n" +
                "Подменю:\n" +
                "• Персонализация+ (темы, цвета, фон, шрифты и др.)\n" +
                "• Панель настроек (система, дисплей, звук и др.)\n\n" +
                "💡 Для доступа: щелкните ПКМ на пустом месте рабочего стола",
                "Добавление в контекстное меню рабочего стола",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    StatusText.Text = "Добавление пунктов в контекстное меню рабочего стола...";
                    string addResult = DesktopContextMenu.AddDesktopTools();

                    MessageBox.Show(
                        addResult,
                        "Результат добавления",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    LoadAdministrationContent();
                    StatusText.Text = "✅ Инструменты успешно добавлены в меню рабочего стола";
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка добавления пунктов в меню";
                    MessageBox.Show(
                        $"Не удалось добавить пункты в контекстное меню рабочего стола:\n\n{ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
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

                    MessageBox.Show(
                        removeResult,
                        "Результат удаления",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    LoadAdministrationContent();
                    StatusText.Text = "✅ Инструменты успешно удалены из меню рабочего стола";
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка удаления пунктов из меню";
                    MessageBox.Show(
                        $"Не удалось удалить пункты из контекстного меню:\n\n{ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void DiagnosticDesktopContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string diagnostic = DesktopContextMenu.GetDiagnosticInfo();

                var diagnosticWindow = new Window
                {
                    Title = "Диагностика контекстного меню рабочего стола",
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
                    Text = diagnostic,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap
                };

                scrollViewer.Content = textBlock;
                diagnosticWindow.Content = scrollViewer;
                diagnosticWindow.ShowDialog();

                StatusText.Text = "Диагностика выполнена";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка диагностики:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        // НОВАЯ ФУНКЦИЯ: ОТМЕНА ПРИМЕНЕННЫХ ТВИКОВ (v3.0)
        // ═══════════════════════════════════════════════════════════════════════

        private async void RevertChanges_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "⚠️ ВЫ УВЕРЕНЫ, ЧТО ХОТИТЕ ОТМЕНИТЬ ВСЕ ПРИМЕНЕННЫЕ ИЗМЕНЕНИЯ?\n\n" +
                "Это действие восстановит систему в исходное состояние:\n\n" +
                "✓ Все примененные твики будут отменены\n" +
                "✓ Службы Windows будут восстановлены\n" +
                "✓ Настройки реестра вернутся к значениям по умолчанию\n\n" +
                "⚠️ ВНИМАНИЕ: Некоторые изменения могут потребовать перезагрузки!",
                "Подтверждение отмены изменений",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                StatusText.Text = "⏳ Отмена изменений...";

                try
                {
                    // Отменяем все примененные твики
                    await tweakEngine.RevertAllTweaksAsync();
                    
                    StatusText.Text = "✅ Все изменения успешно отменены!";

                    MessageBox.Show(
                        "╔═══════════════════════════════════════════════════╗\n" +
                        "║   ✅ ВСЕ ИЗМЕНЕНИЯ УСПЕШНО ОТМЕНЕНЫ!              ║\n" +
                        "╚═══════════════════════════════════════════════════╝\n\n" +
                        "🔄 Система восстановлена в исходное состояние\n\n" +
                        "📋 Что было сделано:\n" +
                        "   • Отменены все примененные твики\n" +
                        "   • Восстановлены службы Windows\n" +
                        "   • Возвращены настройки реестра\n\n" +
                        "⚠️ ВАЖНО:\n" +
                        "   Некоторые изменения вступят в силу после\n" +
                        "   перезагрузки системы.\n\n" +
                        "💡 Рекомендуется перезагрузить компьютер сейчас.",
                        "Отмена изменений завершена",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                        
                    // Снимаем все галочки с чекбоксов
                    RefreshAllCheckboxes();
                }
                catch (Exception ex)
                {
                    StatusText.Text = "❌ Ошибка при отмене изменений";
                    MessageBox.Show(
                        "╔═══════════════════════════════════════════════════╗\n" +
                        "║   ❌ ОШИБКА ПРИ ОТМЕНЕ ИЗМЕНЕНИЙ                  ║\n" +
                        "╚═══════════════════════════════════════════════════╝\n\n" +
                        $"Описание ошибки:\n{ex.Message}\n\n" +
                        "💡 Попробуйте:\n" +
                        "   • Запустить программу от имени администратора\n" +
                        "   • Проверить логи в папке AppData\\WindowsTweaks\\Logs\n" +
                        "   • Создать точку восстановления и откатить вручную",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        // Метод для обновления всех чекбоксов на основе примененных твиков
        private void RefreshAllCheckboxes()
        {
            try
            {
                // Просто перезагружаем текущую категорию
                // Это создаст все элементы заново с правильными индикаторами
                if (NavigationList.SelectedIndex >= 0 && contentLoaders.ContainsKey(NavigationList.SelectedIndex))
                {
                    contentLoaders[NavigationList.SelectedIndex]();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка обновления: {ex.Message}");
            }
        }
    }
}
