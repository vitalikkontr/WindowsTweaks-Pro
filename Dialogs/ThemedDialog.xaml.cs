using System.Windows;
using System.Windows.Media;

namespace WindowsTweaks
{
    /// <summary>
    /// Кастомный диалог в теме программы — замена стандартного MessageBox.
    /// </summary>
    public partial class ThemedDialog : Window
    {
        public bool Result { get; private set; } = false;

        public ThemedDialog()
        {
            InitializeComponent();
        }

        // ─────────────────────────────────────────────────
        // Фабричные методы (аналог MessageBox.Show)
        // ─────────────────────────────────────────────────

        /// <summary>Информационный диалог с кнопкой OK.</summary>
        public static void Show(string message, string title = "WindowsTweaks Pro",
                                DialogIcon icon = DialogIcon.Info, Window owner = null)
        {
            var dlg = Build(message, title, icon, yesNo: false, owner: owner);
            dlg.ShowDialog();
        }

        /// <summary>Диалог подтверждения Да/Нет. Возвращает true если нажато Да.</summary>
        public static bool Confirm(string message, string title = "Подтверждение",
                                   DialogIcon icon = DialogIcon.Question, Window owner = null)
        {
            var dlg = Build(message, title, icon, yesNo: true, owner: owner);
            dlg.ShowDialog();
            return dlg.Result;
        }

        // ─────────────────────────────────────────────────
        // Внутренняя сборка окна
        // ─────────────────────────────────────────────────

        private static ThemedDialog Build(string message, string title,
                                          DialogIcon icon, bool yesNo, Window owner)
        {
            var dlg = new ThemedDialog
            {
                Owner = owner ?? Application.Current?.MainWindow
            };

            dlg.TitleText.Text    = title;
            dlg.MessageText.Text  = message;

            if (yesNo)
            {
                dlg.BtnYes.Content    = "Да";
                dlg.BtnNo.Visibility  = Visibility.Visible;
            }

            switch (icon)
            {
                case DialogIcon.Warning:
                    dlg.IconText.Text       = "⚠";
                    dlg.IconText.Foreground = new SolidColorBrush(Color.FromRgb(230, 126, 34));
                    dlg.IconBorder.Background = new SolidColorBrush(Color.FromRgb(45, 32, 10));
                    break;
                case DialogIcon.Error:
                    dlg.IconText.Text       = "✕";
                    dlg.IconText.Foreground = new SolidColorBrush(Color.FromRgb(192, 57, 43));
                    dlg.IconBorder.Background = new SolidColorBrush(Color.FromRgb(45, 15, 15));
                    break;
                case DialogIcon.Question:
                    dlg.IconText.Text       = "?";
                    dlg.IconText.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 120));
                    dlg.IconBorder.Background = new SolidColorBrush(Color.FromRgb(20, 45, 28));
                    break;
                case DialogIcon.Success:
                    dlg.IconText.Text       = "✓";
                    dlg.IconText.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 120));
                    dlg.IconBorder.Background = new SolidColorBrush(Color.FromRgb(20, 45, 28));
                    break;
                default: // Info
                    dlg.IconText.Text       = "ℹ";
                    dlg.IconText.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 120));
                    dlg.IconBorder.Background = new SolidColorBrush(Color.FromRgb(20, 45, 28));
                    break;
            }

            return dlg;
        }

        // ─────────────────────────────────────────────────
        // Обработчики
        // ─────────────────────────────────────────────────

        private void Header_MouseDown(object sender,
            System.Windows.Input.MouseButtonEventArgs e) => DragMove();

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }
    }

    public enum DialogIcon
    {
        Info,
        Warning,
        Error,
        Question,
        Success
    }
}
