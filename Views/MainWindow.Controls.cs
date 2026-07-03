using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindowsTweaks
{
    public partial class MainWindow
    {
        private TextBlock CreateTitle(string text)
        {
            return new TextBlock
            {
                Text = text,
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(232, 245, 236)),
                Margin = new Thickness(0, 0, 0, 8)
            };
        }

        private void AddSectionSeparator(string sectionName)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 7, 0, 5)
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
                Text       = sectionName,
                FontSize   = 11,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(168, 196, 174)),
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
                Margin       = new Thickness(0, 0, 0, 2),
                Padding      = new Thickness(6, 3, 6, 3),
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
                Margin          = new Thickness(0, 1, 0, 1),
                Padding         = new Thickness(6, 3, 6, 3),
                Cursor          = System.Windows.Input.Cursors.Hand
            };

            var outerGrid = new Grid();
            outerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            outerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            outerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Иконка
            var iconBorder = new Border
            {
                Width           = 18,
                Height          = 18,
                Background      = new SolidColorBrush(Color.FromRgb(28, 52, 38)),
                BorderBrush     = new SolidColorBrush(Color.FromRgb(46, 100, 65)),
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(3),
                Margin          = new Thickness(0, 0, 7, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            iconBorder.Child = new TextBlock
            {
                Text                = emoji,
                FontSize            = 10,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            };

            // Текст
            var textStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            textStack.Children.Add(new TextBlock
            {
                Text       = name,
                Foreground = new SolidColorBrush(Color.FromRgb(232, 245, 236)),
                FontSize   = 10,
                FontWeight = FontWeights.SemiBold
            });
            textStack.Children.Add(new TextBlock
            {
                Text       = description,
                Foreground = new SolidColorBrush(Color.FromRgb(107, 155, 117)),
                FontSize   = 9,
                Margin     = new Thickness(0, 1, 0, 0)
            });

            // Стрелка
            var arrow = new TextBlock
            {
                Text                = "›",
                Foreground          = new SolidColorBrush(Color.FromRgb(61, 120, 85)),
                FontSize            = 11,
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

    }
}
