using EnergyApp.Models;
using System;
using System.Windows;

namespace EnergyApp
{
    public partial class AddRecordWindow : Window
    {
        public EnergyRecord NewRecord { get; private set; }
        public double DefaultPrice { get; set; } = 5.0;

        public AddRecordWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!DatePicker.SelectedDate.HasValue ||
                !double.TryParse(KilowattsTextBox.Text, out double kwh) || kwh <= 0 ||
                !double.TryParse(PricePerKwTextBox.Text, out double price) || price <= 0)
            {
                MessageBox.Show("Заполните все поля корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double cost = kwh * price;
            NewRecord = new EnergyRecord
            {
                Date = DatePicker.SelectedDate.Value.Date,
                Consumption = kwh,
                PricePerKwh = price,
                Cost = cost
            };

            DialogResult = true;
            Close();
        }
    }
}