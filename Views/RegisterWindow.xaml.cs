using System.Windows;
using System.Windows.Media;
using ParkingApp.Services;
using ParkingApp.Utils;

namespace ParkingApp.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly DatabaseService _dbService;
        private readonly AuthenticationService _authService;

        private static readonly SolidColorBrush RedBrush = new SolidColorBrush(Color.FromRgb(255, 107, 107));
        private static readonly SolidColorBrush GreenBrush = new SolidColorBrush(Color.FromRgb(80, 200, 80));
        private static readonly SolidColorBrush NormalBorder = new SolidColorBrush(Color.FromRgb(68, 68, 68));
        private static readonly SolidColorBrush ErrorBorder = new SolidColorBrush(Color.FromRgb(255, 80, 80));

        public RegisterWindow(DatabaseService dbService, AuthenticationService authService)
        {
            InitializeComponent();
            _dbService = dbService;
            _authService = authService;

            // Валідація email в реальному часі при виході з поля
            EmailTextBox.LostFocus += (s, e) => ValidateEmailField();
        }

        private bool ValidateEmailField()
        {
            string email = EmailTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                SetFieldError(EmailTextBox, null);
                return false;
            }

            if (!ValidationHelper.IsValidEmail(email))
            {
                SetFieldError(EmailTextBox, "❌ Невірний формат пошти. Приклад: user@gmail.com");
                return false;
            }

            // Все добре — знімаємо підсвітку
            EmailTextBox.BorderBrush = NormalBorder;
            Message.Text = "";
            return true;
        }

        private void SetFieldError(System.Windows.Controls.TextBox field, string msg)
        {
            field.BorderBrush = ErrorBorder;
            if (msg != null)
            {
                Message.Text = msg;
                Message.Foreground = RedBrush;
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            // Перевірка порожніх полів
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password))
            {
                Message.Text = "❌ Будь ласка, заповніть усі поля";
                Message.Foreground = RedBrush;
                return;
            }

            // Валідація email
            if (!ValidationHelper.IsValidEmail(email))
            {
                SetFieldError(EmailTextBox, "❌ Невірний формат пошти. Приклад: user@gmail.com");
                return;
            }
            else
            {
                EmailTextBox.BorderBrush = NormalBorder;
            }

            // Валідація логіна
            if (!ValidationHelper.IsValidLogin(login))
            {
                SetFieldError(LoginTextBox,
                    "❌ Логін: 3–20 символів, лише латиниця, цифри та _");
                LoginTextBox.BorderBrush = ErrorBorder;
                return;
            }
            else
            {
                LoginTextBox.BorderBrush = NormalBorder;
            }

            // Валідація пароля
            if (!ValidationHelper.IsValidPassword(password))
            {
                Message.Text = "❌ Пароль має бути не менше 6 символів";
                Message.Foreground = RedBrush;
                return;
            }

            // Реєстрація
            bool success = _authService.Register(email, login, password);

            if (success)
            {
                Message.Text = "✅ Реєстрація успішна! Повертаємось на вхід...";
                Message.Foreground = GreenBrush;

                // Невеликий таймер щоб юзер побачив повідомлення
                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = System.TimeSpan.FromSeconds(1.2);
                timer.Tick += (ts, te) =>
                {
                    timer.Stop();
                    var loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                };
                timer.Start();
            }
            else
            {
                Message.Text = "❌ Логін вже існує. Спробуйте інший.";
                Message.Foreground = RedBrush;
                LoginTextBox.BorderBrush = ErrorBorder;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}