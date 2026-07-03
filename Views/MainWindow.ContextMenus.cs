using System;
using System.Windows;

namespace WindowsTweaks
{
    public partial class MainWindow
    {
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

    }
}
