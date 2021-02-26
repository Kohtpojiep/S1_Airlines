using S1_Вариант3.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
using System.Windows.Threading;

namespace S1_Вариант3.Pages.AdministratorRole
{
    /// <summary>
    /// Логика взаимодействия для AdministrationMenu.xaml
    /// </summary>

    public partial class AdministrationMenu : Page
    {
        readonly MainWindow _mainWindow;
        readonly Users _loginnedUser; // пользователь под которого авторизовался пользователь

        public AdministrationMenu(MainWindow main, Users userLogged)
        {
            InitializeComponent();
            _mainWindow = main;
            _loginnedUser = userLogged;

            main.Title = "AMONIC Airlines Automation System";   
            UsersDG.ItemsSource = Session1_3.GetContext().Users.ToList(); // список пользователей системы
            
            List<Offices> offices = Session1_3.GetContext().Offices.ToList();
            offices.Insert(0, new Offices
            {
                id = 0,
                Name = "All Offices"
            });
            OfficeCB.ItemsSource = offices; // список офисов
        }

        // обновление списка пользователей
        public void UpdateDataGrid()
        {
            UsersDG.ItemsSource = Session1_3.GetContext().Users.ToList();
        }

        // переход для изменения параметров пользователя
        private void ChangeRoleBT_Click(object sender, RoutedEventArgs e)
        {
            List<Users> users = UsersDG.SelectedItems.Cast<Users>().ToList();
            if(users.Count > 1)
            {
                MessageBox.Show("Выберите только одного пользователя");
                return;
            }
            if (users.Count < 1)
            {
                MessageBox.Show("Выберите пользователя для редактирования");
                return;
            }
            Window editWindow = new Windows.EditUserWindow(users[0], this, users[0].id == _loginnedUser.id);
            editWindow.ShowDialog();
        }

        // отключение/включение пользователя
        private void EnableDisableBT_Click(object sender, RoutedEventArgs e)
        {
            List<Users> users = UsersDG.SelectedItems.Cast<Users>().ToList();
            if (users.Count < 1)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }

            MessageBoxResult msg = MessageBox.Show("Are you sure?", "Attention", MessageBoxButton.YesNo);
            if (msg == MessageBoxResult.Yes)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    users[i].Active = users[i].Active != true;
                }
                Session1_3.GetContext().SaveChanges();
                UpdateDataGrid();
                MessageBox.Show("Success done.");
            }
        }

        // фильтрация пользователей по критерию
        private void OfficeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OfficeCB.SelectedIndex > 0)
            {
                Offices office = (Offices)OfficeCB.SelectedItem;
                UsersDG.ItemsSource = Session1_3.GetContext().Users.Where(x => x.Office_id == office.id).ToList();
            }
            else
            {
                UsersDG.ItemsSource = Session1_3.GetContext().Users.ToList();
            }
        }

        // выход из системы
        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msg = MessageBox.Show("Are you sure", "Attention", MessageBoxButton.YesNo);
            if (msg == MessageBoxResult.Yes)
            {
                _mainWindow.Logout_User();
                this.NavigationService.GoBack();
            }
        }

        // добавление нового пользователя
        private void AddUserBT_Click(object sender, RoutedEventArgs e)
        {
            Window editWindow = new Windows.AddUserWindow(this);
            editWindow.ShowDialog();
        }

    }
}
