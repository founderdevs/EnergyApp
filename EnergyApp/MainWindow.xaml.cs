using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections.ObjectModel;
using EnergyApp.Models;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using EnergyApp.Models;

namespace EnergyApp
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<EnergyRecord> EnergyRecords { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            EnergyRecords = new ObservableCollection<EnergyRecord>();
            EnergyDataGrid.ItemsSource = EnergyRecords;

            // Опционально — начальные даты для фильтра (последние 30 дней)
            StartDatePicker.SelectedDate = DateTime.Now.Date.AddDays(-30);
            EndDatePicker.SelectedDate = DateTime.Now.Date;
        }

        // Добавление записи
        private void AddRecord_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(KilowattInput.Text, out double kwh) &&
                double.TryParse(CostInput.Text, out double price))
            {
                EnergyRecords.Add(new EnergyRecord
                {
                    Date = DateTime.Now,
                    KilowattHours = kwh,
                    CostPerKwh = price
                });

                KilowattInput.Clear();
                CostInput.Clear();
            }
            else
            {
                MessageBox.Show("Введите корректные значения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Удаление выбранной записи
        private void DeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            if (EnergyDataGrid.SelectedItem is EnergyRecord record)
            {
                EnergyRecords.Remove(record);
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Подсчёт за период
        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.SelectedDate is DateTime start && EndDatePicker.SelectedDate is DateTime end)
            {
                // Сравниваем по дате без времени: берем Date.Date
                var total = EnergyRecords
                    .Where(r => r.Date.Date >= start.Date && r.Date.Date <= end.Date)
                    .Sum(r => r.TotalCost);

                ResultText.Text = $"Сумма за период: {total:F2} ₽";
            }
            else
            {
                MessageBox.Show("Выберите даты начала и конца периода!");
            }
        }

        // Пока-заглушка для сохранения чека — потом заменим на реальную генерацию PDF/CSV
        private void SaveCheck_Click(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.SelectedDate is DateTime start && EndDatePicker.SelectedDate is DateTime end)
            {
                var records = EnergyRecords
                    .Where(r => r.Date.Date >= start.Date && r.Date.Date <= end.Date)
                    .ToList();

                if (!records.Any())
                {
                    MessageBox.Show("Нет записей за выбранный период.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Пока просто показываем сумму — заменим на экспорт в PDF/CSV
                var total = records.Sum(r => r.TotalCost);
                MessageBox.Show($"Чек за период {start:d} — {end:d}\nИтого: {total:F2} ₽", "Чек", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Выберите даты начала и конца периода!");
            }
        }
    }
}