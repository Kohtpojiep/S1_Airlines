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
using System.Windows.Shapes;
using S1_Вариант3.Model;
using S1_Вариант3.Pages.UserRole;

namespace S1_Вариант3.Windows
{
    /// <summary>
    /// Логика взаимодействия для ScheludeEditWindow.xaml
    /// </summary>
    public partial class ScheludeEditWindow : Window
    {
        Flight_Scheludes _flightSchelude;
        ManageFlightScheludes _manageFlight;
        public ScheludeEditWindow(Flight_Scheludes currentSchelude, ManageFlightScheludes flightScheludes)
        {
            InitializeComponent();
            _flightSchelude = currentSchelude;
            _manageFlight = flightScheludes;

            FromCountry_Label.Content += currentSchelude.Countries.CountryShort;
            ToCountry_Label.Content += currentSchelude.Countries1.CountryShort;
            Aircraft_Label.Content += currentSchelude.Aircraft.Model;

            Date_DatePicker.SelectedDate = currentSchelude.Date;
            Time_TextBox.Text = currentSchelude.Time.ToString();
            EconomyPrice_TextBox.Text = currentSchelude.EconomyPrice.ToString();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msg = MessageBox.Show("Are you sure?", "Attention", MessageBoxButton.YesNo);
            if (msg == MessageBoxResult.Yes)
            {
                try
                {
                    _flightSchelude.Date = Date_DatePicker.SelectedDate.Value;
                    _flightSchelude.Time = Convert.ToDateTime(Time_TextBox.Text).TimeOfDay;
                    _flightSchelude.EconomyPrice = Convert.ToInt16(EconomyPrice_TextBox.Text);

                    Session1_3.GetContext().SaveChanges();
                }
                catch
                {
                    MessageBox.Show("Неккоректно записаны данные...");
                    return;
                }
                _manageFlight.LoadFlightScheludes();
                this.Close();
            }   
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msg = MessageBox.Show("Are you sure?", "Attention", MessageBoxButton.YesNo);
            if (msg == MessageBoxResult.Yes)
                this.Close();
        }
    }
}
