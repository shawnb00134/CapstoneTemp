using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CMSDesktopClient.Data;

namespace CMSDesktopClient.View
{
    public sealed partial class LoginPage : Page
    {
        private DatabaseHelper _dbHelper;

        public LoginPage()
        {
            this.InitializeComponent();

            // Initialize database helper
            // TODO: Replace with your actual database credentials
            _dbHelper = new DatabaseHelper(
                host: "localhost",        // or your server IP
                database: "cam_cms",
                username: "postgres",
                password: "password",
                port: 5432
            );

            // Test connection on page load
            TestDatabaseConnection();
        }

        private async void TestDatabaseConnection()
        {
            try
            {
                bool isConnected = await _dbHelper.TestConnectionAsync();
                if (isConnected)
                {
                    StatusTextBlock.Text = "✅ Database connected";
                    StatusTextBlock.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(
                        Windows.UI.Colors.Green);
                }
                else
                {
                    StatusTextBlock.Text = "❌ Database connection failed";
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"❌ Error: {ex.Message}";
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                StatusTextBlock.Text = "Please enter username and password";
                return;
            }

            try
            {
                StatusTextBlock.Text = "Logging in...";

                bool isValid = await _dbHelper.ValidateLoginAsync(username, password);

                if (isValid)
                {
                    // Login successful
                    this.Frame.Navigate(typeof(DashboardPage));
                }
                else
                {
                    // Login failed
                    StatusTextBlock.Text = "Invalid username or password";
                    StatusTextBlock.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(
                        Windows.UI.Colors.Red);
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Login error: {ex}");
            }
        }
    }
}