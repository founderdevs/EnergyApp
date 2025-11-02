using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent(); // Обязательно — генерируется из XAML
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            // Простая проверка (заменишь на реальную позже)
            if (login == "admin" && password == "1234")
            {
                var dashboard = new DashboardWindow();
                dashboard.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка входа",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}