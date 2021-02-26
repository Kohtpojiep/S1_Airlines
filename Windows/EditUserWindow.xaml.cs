using S1_Вариант3.Model;
using S1_Вариант3.Pages.AdministratorRole;
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
using System.Windows.Shapes;

namespace S1_Вариант3.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChangeRoleWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Window
    {
        Label _currentLabel = new Label();
        readonly List<Offices> _offices;
        readonly Users _currentUser;
        readonly AdministrationMenu _menu;
        readonly bool _isCurrentAsLoggined = false;

        public EditUserWindow(Users user, AdministrationMenu currentMenu, bool isLoginnedUser)
        {
            InitializeComponent();
            _menu = currentMenu;
            _currentUser = user;
            _isCurrentAsLoggined = isLoginnedUser;
            _offices = Session1_3.GetContext().Offices.ToList();
            _offices.Insert(0, new Offices { id = 0, Name = "Not stated" });
            OfficeCB.ItemsSource = _offices;
            ImportData();
        }

        private void ImportData()
        {
            EmailTB.Text = _currentUser.Email;
            FirstnameTB.Text = _currentUser.Firstname;
            LastnameTB.Text = _currentUser.Lastname;
            OfficeCB.SelectedIndex = _currentUser.Offices != null ? _offices.IndexOf(_currentUser.Offices) : 0;

            switch (_currentUser.Role)
            {
                case "Administrator": AdministratorRB.IsChecked = true; break;
                case "User": UserRB.IsChecked = true; break;
            }
        }

        private void UpdateUser()
        {
            _currentUser.Role = GetChoisedRole();
            _currentUser.Email = EmailTB.Text;
            _currentUser.Password = _currentUser.Password;
            _currentUser.Firstname = FirstnameTB.Text;
            _currentUser.Lastname = LastnameTB.Text;
            _currentUser.Office_id = OfficeCB.SelectedIndex > 0 ? ((Offices)OfficeCB.SelectedItem).id : new Nullable<int>();
            _currentUser.Role = AdministratorRB.IsChecked == true ? "Administrator" : "User";
        }

        private string GetChoisedRole()
        {
            foreach(var i in RadioButtonGrid.Children)
                if (i is RadioButton bt)
                    if (bt.IsChecked == true)
                        return bt.Content.ToString();
            return null;
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            _currentLabel = (Label)sender;
            _currentLabel.Visibility = Visibility.Hidden;
        }

        private void TextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (((TextBox)sender).Text == "")
                _currentLabel.Visibility = Visibility.Visible;
        }
        private void ComboBox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex == -1)
                _currentLabel.Visibility = Visibility.Visible;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text == "")
                _currentLabel.Visibility = Visibility.Visible;
            else
                _currentLabel.Visibility = Visibility.Hidden;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex == -1)
                _currentLabel.Visibility = Visibility.Visible;
            else
                _currentLabel.Visibility = Visibility.Hidden;
        }

        private bool AreNullInputs()
        {
            foreach (var i in InputElementsGrid.Children)
            {
                if (i is TextBox tb)
                    if (tb.Text == "")
                        return false;

                if (i is ComboBox cb)
                    if (cb.SelectedIndex == -1)
                        return false;
            }
            return true;
        }

        private void ApplyBT_Click(object sender, RoutedEventArgs e)
        {
            if (!AreNullInputs())
            {
                MessageBox.Show("Can't be done. There are null textbox.");
                return;
            }

            MessageBoxResult msg = MessageBox.Show("Are you sure?", "Attention", MessageBoxButton.YesNo);
            if (msg == MessageBoxResult.Yes)
            {
                UpdateUser();
                Session1_3.GetContext().SaveChanges();
                _menu.UpdateDataGrid();
                MessageBox.Show("Successful done.");
                this.Close();
            }
        }

        private void CancelBT_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msg = MessageBox.Show("Are you sure?", "Attention", MessageBoxButton.YesNo);
            if (msg == MessageBoxResult.Yes)
            {
                _menu.UpdateDataGrid();
                this.Close();
            }
        }
    }
}
