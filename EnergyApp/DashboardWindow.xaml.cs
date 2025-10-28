using EnergyApp.Data;
using EnergyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EnergyApp
{
    public partial class DashboardWindow : Window
    {
        private List<EnergyRecord> allRecords = new();

        public DashboardWindow()
        {
            InitializeComponent();
            DatabaseHelper.InitializeDatabase();
            LoadData();
        }

        private void LoadData()
        {
            allRecords = DatabaseHelper.GetRecords();
            EnergyDataGrid.ItemsSource = allRecords;
            UpdateSummary(allRecords);
        }

        // Применить фильтр по диапазону дат
        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите обе даты для фильтрации.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime start = StartDatePicker.SelectedDate.Value;
            DateTime end = EndDatePicker.SelectedDate.Value;

            if (start > end)
            {
                MessageBox.Show("Начальная дата не может быть позже конечной.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var filtered = allRecords
                .Where(r => r.Date >= start && r.Date <= end)
                .ToList();

            EnergyDataGrid.ItemsSource = filtered;
            UpdateSummary(filtered);
        }

        // Сброс фильтра
        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            EnergyDataGrid.ItemsSource = allRecords;
            UpdateSummary(allRecords);
        }

        // Пересчёт итогов
        private void UpdateSummary(List<EnergyRecord> records)
        {
            double totalConsumption = records.Sum(r => r.Consumption);
            double totalCost = records.Sum(r => r.Cost);

            SummaryText.Text = $"Итого: {totalConsumption} кВт⋅ч | {totalCost:F2} ₽";
        }

        private void AddRecordButton_Click(object sender, RoutedEventArgs e)
        {
            AddRecordWindow addWindow = new AddRecordWindow();
            addWindow.ShowDialog();
            LoadData();
        }

        private void DeleteRecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnergyDataGrid.SelectedItem is EnergyRecord selected)
            {
                DatabaseHelper.DeleteRecord(selected.Id);
                LoadData();
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления");
            }
        }
    }
}