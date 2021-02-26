using S1_Вариант3.Model;
using S1_Вариант3.Pages.AdministratorRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        readonly List<Offices> _offices;
        readonly AdministrationMenu _administration;
        public AddUserWindow(AdministrationMenu menu)
        {
            InitializeComponent();
            _administration = menu;

            _offices = Session1_3.GetContext().Offices.ToList();
            _offices.Insert(0, new Offices { id = 0, Name = "Not stated" });
            OfficeCB.ItemsSource = _offices;
        }

        private bool CheckInputs()
        {
            foreach (var i in InputElementsGrid.Children)
            {
                if (i is TextBox tb)
                {
                    if (tb.Text == "")
                    {
                        MessageBox.Show("Detected null TextBox. You should type value");
                        return false;
                    }
                    if (tb.Name == "EmailTB")
                    {
                        Regex emailFilter = new Regex(@"^((\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)\s*[;]{0,1}\s*)+$");
                        if (!(emailFilter.IsMatch(tb.Text)))
                        {
                            MessageBox.Show("Incorrect email...");
                            return false;
                        }    
                    }
                }
                if (i is PasswordBox pb)
                    if (pb.Password == "")
                    {
                        MessageBox.Show("Detected null Password. You should type value");
                        return false;
                    }
                if (i is DatePicker dp)
                    if (dp.SelectedDate == null)
                    {
                        MessageBox.Show("You have not selected Birthdate. Select the date.");
                        return false;
                    }
                if (i is ComboBox cb)
                    if (cb.SelectedIndex == -1)
                    {
                        MessageBox.Show("You have not selected value in ComboBox.");
                        return false;
                    }
            }
            return true;
        }

        private void SaveBT_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInputs() != true)
                return;
            Users user;
            try
            {
                user = new Users
                {
                    Role = "User",
                    Email = EmailTB.Text,
                    Password = PasswordPB.Password,
                    Firstname = FirstnameTB.Text,
                    Lastname = LastnameTB.Text,
                    Title = "",
                    Birthdate = Convert.ToDateTime(BirthdateDP.Text),
                    Office_id = OfficeCB.SelectedIndex > 0 ? ((Offices)OfficeCB.SelectedItem).id : new Nullable<int>(),
                    Active = true
                };
            }
            catch
            {
                MessageBox.Show("Incorrect typed data. Try again.");
                return;
            }
            try 
            {
                Session1_3.GetContext().Users.Add(user);
                Session1_3.GetContext().SaveChanges();
                _administration.UpdateDataGrid();
                MessageBox.Show("Successful added.");
                this.Close();
            }
            catch (Exception ex)
            {
                Session1_3.GetContext().Users.Remove(user);
                MessageBox.Show($"Can't be added:\n{ex.Message}");
            }
        }

        private void CancelBT_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msg = MessageBox.Show("Are you sure?", "Attention", MessageBoxButton.YesNo);
            if (msg == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
    }
}
