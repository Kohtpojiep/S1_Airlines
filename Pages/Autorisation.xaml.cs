using S1_Вариант3.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using S1_Вариант3.Pages.UserRole;

namespace S1_Вариант3.Pages
{
    /// <summary>
    /// Логика взаимодействия для Autorisation.xaml
    /// </summary>
    public partial class Autorisation : Page
    {
        readonly MainWindow _mainWindow;
        readonly DispatcherTimer _afkTimer = new DispatcherTimer(); // таймер для отсчета до выхода из приложения
        int _seconds = 60; // секунды до выхода

        public Autorisation(MainWindow main)
        {
            InitializeComponent();
            _mainWindow = main;
            main.Title = "login";

            // конфигурация таймера
            _afkTimer.Tick += new EventHandler(CheckAfk);
            _afkTimer.Interval = new TimeSpan(0, 0, 1);
            _afkTimer.Start();
        }

        // событие при тике
        private void CheckAfk(object sender, EventArgs e)
        {
            _seconds--;
            TimerLabel.Content = _seconds;
            if (_seconds <= 0)
            {
                _mainWindow.Visibility = Visibility.Hidden;
                MessageBox.Show("Вы доафкашились...");
                _mainWindow.Close();
            }   
        }

        // авторизация пользователя
        private void LoginBT_Click(object sender, RoutedEventArgs e)
        {
            string login = UsernameTB.Text != "" ? UsernameTB.Text : null;
            string pass = PasswordPB.Password != "" ? PasswordPB.Password : null;

            if (login == null || pass == null)
            {
                MessageBox.Show("Поля должны быть заполнены", "Внимание");
                return;
            }

            // получение данных о пользователе. Если такогго нет - получаем null
            Users user = Session1_3.GetContext().Users.Where(x => x.Email == login && x.Password == pass).SingleOrDefault();

            if (user != null)
            {
                // проверка на блокировку пользователя
                if (user.Active != true)
                {
                    MessageBox.Show("This user is not active. Contact to the Administrator");
                    return;
                }

                // в случае, если процесс входа успешен
                _afkTimer.Stop();

                _mainWindow.Log_Autorisation(user); // инициализируем новую сессию для пользователя

                // переводим пользователя в соответствующее для него окно
                if (user.Role == "Administrator")
                    this.NavigationService.Navigate(new AdministratorRole.AdministrationMenu(_mainWindow, user));
                else
                    if (user.Role == "User")
                        this.NavigationService.Navigate(new UserMenu(_mainWindow, user));
            }
            else
                MessageBox.Show("Неверная почта или пароль");

            //System.Data.SqlClient.SqlParameter paramLogin = new System.Data.SqlClient.SqlParameter("@login", login);
            //System.Data.SqlClient.SqlParameter paramPass = new System.Data.SqlClient.SqlParameter("@password", pass);
            //bool isCorrect = session1.Database.SqlQuery<bool>("SELECT dbo.IsCorrectUser(@login, @password)", paramLogin, paramPass).First();
        }

        // Процессы, при котором счётчик возращается в исходное состояние
        private void Page_MouseMove(object sender, MouseEventArgs e) => _seconds = 60;

        private void Page_MouseDown(object sender, MouseButtonEventArgs e) => _seconds = 60;

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => _seconds = 60;
        private void PasswordPB_PasswordChanged(object sender, RoutedEventArgs e) => _seconds = 60;

        // ---

        // Выход из приложения
        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msg = MessageBox.Show("Are you sure", "Attention", MessageBoxButton.YesNo);
            if (msg == MessageBoxResult.Yes)
            {
                _mainWindow.Close();
            }
        }

        // Изменение размера текста в соответствии с размерами окна
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UsernameLB.FontSize = UsernameLB.ActualHeight * 0.5;
            PasswordLB.FontSize = PasswordLB.ActualHeight * 0.5;
            UsernameTB.FontSize = UsernameTB.ActualHeight * 0.55;
            PasswordPB.FontSize = PasswordPB.ActualHeight * 0.55;

            LoginBT.FontSize = LoginBT.ActualHeight * 0.5;
            ExitBT.FontSize = ExitBT.ActualHeight * 0.5;
        }

        // При возращении в данное окно из других окон
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow.Title = "login";
            _afkTimer.Start();
        }
    }
}
