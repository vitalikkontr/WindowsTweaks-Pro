using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindowsTweaks
{
    public partial class MainWindow
    {
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

    }
}
