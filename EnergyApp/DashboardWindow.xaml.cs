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
        private List<EnergyRecord> _records = new();

        public DashboardWindow()
        {
            InitializeComponent();
            DatabaseHelper.InitializeDatabase();
            LoadData();
        }

        private void LoadData()
        {
            _records = DatabaseHelper.GetRecords();
            EnergyDataGrid.ItemsSource = _records;
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            double totalKw = _records.Sum(r => r.Consumption);
            double totalCost = _records.Sum(r => r.Cost);
            SummaryText.Text = $"Итого: {totalKw} кВт⋅ч | {totalCost:F2} ₽";
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