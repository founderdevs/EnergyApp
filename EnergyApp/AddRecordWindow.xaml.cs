using EnergyApp.Data;
using EnergyApp.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace EnergyApp
{
    public partial class AddRecordWindow : Window
    {
        public AddRecordWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(ConsumptionTextBox.Text, out double consumption))
            {
                MessageBox.Show("Введите корректное значение потребления!");
                return;
            }

            double price = 5.0; // Можно позже брать из поля PricePerKwTextBox в Dashboard
            double cost = consumption * price;

            var record = new EnergyRecord
            {
                Date = DatePicker.SelectedDate ?? DateTime.Today,
                Consumption = consumption,
                Cost = cost,
                Comment = CommentTextBox.Text
            };

            DatabaseHelper.AddRecord(record);
            this.Close();
        }
    }
}