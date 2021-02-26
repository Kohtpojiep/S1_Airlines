using S1_Вариант3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using S1_Вариант3.Pages.UserRole;

namespace S1_Вариант3.Pages.UserRole
{
    /// <summary>
    /// Логика взаимодействия для UserMenu.xaml
    /// </summary>
    public partial class UserMenu : Page
    {
        readonly MainWindow _mainWindow;
        readonly List<Loginings> _userLogs; // список сессий пользователя
        readonly Users _currentUser;
        public UserMenu(MainWindow main, Users user)
        {
            InitializeComponent();
            _mainWindow = main;
            main.Title = "User menu";
            _currentUser = user;

            _userLogs = Session1_3.GetContext().Loginings.Where(x => x.User_id == user.id).ToList();
            _userLogs.RemoveAt(_userLogs.Count - 1); // удаление последней сессии из списка

            HelloLabel.Content = $"Hi {user.Firstname}, Welcome to AMONIC Airlines.";
            string timeSpentOnSystem = (TimeSpan.FromSeconds(_userLogs.Where(x => x.Date >= DateTime.Now.AddDays(-30).Date).Sum(x => x.Time_spent_on).GetValueOrDefault())).ToString("hh':'mm':'ss");  // вывод времени в системе за последние 30 дней
            TimespentLabel.Content = $"Time spent on system : {timeSpentOnSystem}";
            NumberOfCrashesLabel.Content = $"Number of crashes: {_userLogs.Count(x => x.Error_reason != null)}";

            LogsOfUserDG.ItemsSource = _userLogs;
        }

        // Выход пользователя из системы
        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msg = MessageBox.Show("Are you sure?", "Attention", MessageBoxButton.YesNo);
            if (msg == MessageBoxResult.Yes)
            {
                _mainWindow.Logout_User();
                this.NavigationService.GoBack();
            }
        }

        private void GoToFlightScheludesBT_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ManageFlightScheludes(_mainWindow, _currentUser));
        }

        private void GoToSearchFlightsBT_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SearchForFlights(user: _currentUser, main: _mainWindow));
        }

        private void UserMenu_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow.Title = "User menu";
        }
    }
}
