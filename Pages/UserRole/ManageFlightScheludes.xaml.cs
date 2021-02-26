using System;
using S1_Вариант3.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using S1_Вариант3.Windows;

namespace S1_Вариант3.Pages.UserRole
{
    /// <summary>
    /// Логика взаимодействия для ManageFlightScheludes.xaml
    /// </summary>
    public partial class ManageFlightScheludes
    {
        readonly MainWindow _mainWindow;
        readonly Users _currentUser;
        public ManageFlightScheludes(MainWindow main, Users userLogged)
        {
            InitializeComponent();
            _mainWindow = main;
            _currentUser = userLogged;

            List<Countries> countries = Session1_3.GetContext().Countries.ToList();
            countries.Insert(0, new Countries { CountryShort = "Любая", Country = "Na" });

            string[] sortByArray = new string[] { "Нет", "Date-Time", "Price", "Is Accepted" };
            SortByComboBox.ItemsSource = sortByArray;

            AirportListFromComboBox.ItemsSource = countries;
            AirportListToComboBox.ItemsSource = countries;
            ScheludesDataGrid.ItemsSource = Session1_3.GetContext().Flight_Scheludes.OrderByDescending(x => x.Date).ThenByDescending(x => x.Time).ToList();
        }

        private void CancelFlights_Button_Click(object sender, RoutedEventArgs e)
        {
            List<Flight_Scheludes> scheludes = ScheludesDataGrid.SelectedItems.Cast<Flight_Scheludes>().ToList();
            if (scheludes.Count < 1)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }

            MessageBoxResult msg = MessageBox.Show("Are you sure?", "Attention", MessageBoxButton.YesNo);
            if (msg == MessageBoxResult.Yes)
            {
                for (int i = 0; i < scheludes.Count; i++)
                {
                    scheludes[i].IsAccepted = scheludes[i].IsAccepted != true;
                }
                Session1_3.GetContext().SaveChanges();
                LoadFlightScheludes();
                MessageBox.Show("Success done.");
            }
        }

        private void ExitBT_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msg = MessageBox.Show("Are you sure?", "Attention", MessageBoxButton.YesNo);
            if (msg == MessageBoxResult.Yes)
            {
                _mainWindow.Logout_User();
                var navigationService = this.NavigationService;
                if (navigationService != null) navigationService.GoBack();
            }
        }

        private void GoToFlightScheludesBT_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msg = MessageBox.Show("Are you sure?", "Attention", MessageBoxButton.YesNo);
            if (msg == MessageBoxResult.Yes)
            {
                var navigationService = this.NavigationService;
                if (navigationService != null) navigationService.GoBack();
            }
        }

        public void LoadFlightScheludes()
        {
            int flightNum = 0;

            try { if (FlightNumberTextBox.Text != "") flightNum = Convert.ToInt32(FlightNumberTextBox.Text); }
            catch { MessageBox.Show("Допущена ошибка, при наборе номера"); return; }

            List<Flight_Scheludes> flightScheludes = Session1_3.GetContext().Flight_Scheludes.ToList();
            List<Flight_Scheludes> newFlightScheludes = new List<Flight_Scheludes>();
            for (int i = 0; i < flightScheludes.Count; i++)
            {
                bool checkingParams = true;
                if (FlightNumberTextBox.Text != "")
                    if (flightScheludes[i].FlightNumber != flightNum)
                        checkingParams = false;

                if (AirportListFromComboBox.SelectedIndex > 0)
                    if (((Countries)AirportListFromComboBox.SelectedValue).id != flightScheludes[i].CountryFromId)
                        checkingParams = false;

                if (AirportListToComboBox.SelectedIndex > 0)
                    if (((Countries)AirportListToComboBox.SelectedValue).id != flightScheludes[i].CountryToId)
                        checkingParams = false;

                if (OutboundDatePicker.SelectedDate.HasValue)
                    if (OutboundDatePicker.SelectedDate.Value != flightScheludes[i].Date)
                        checkingParams = false;

                if (checkingParams == true)
                    newFlightScheludes.Add(flightScheludes[i]);
            }

            if (SortByComboBox.SelectedIndex > 0)
            {
                if ((string)SortByComboBox.SelectedItem == "Date-Time")
                    newFlightScheludes = newFlightScheludes.OrderByDescending(x => x.Date).ThenByDescending(x => x.Time).ToList();
                if ((string)SortByComboBox.SelectedItem == "Price")
                    newFlightScheludes = newFlightScheludes.OrderByDescending(x => x.EconomyPrice).ToList();
                if ((string)SortByComboBox.SelectedItem == "Is Accepted")
                    newFlightScheludes = newFlightScheludes.OrderByDescending(x => x.IsAccepted).ToList();
            }

            ScheludesDataGrid.ItemsSource = newFlightScheludes;
        }

        private void ImportChanges_Button_Click(object sender, RoutedEventArgs e)
        {
            ImportingWindow window = new ImportingWindow();
            window.ShowDialog();
        }

        private void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            LoadFlightScheludes();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFlightScheludes();
        }

        private void EditFlight_Button_Click(object sender, RoutedEventArgs e)
        {
            List<Flight_Scheludes> scheludes = ScheludesDataGrid.SelectedItems.Cast<Flight_Scheludes>().ToList();
            if (scheludes.Count != 1)
            {
                MessageBox.Show("Выберите однин рейс");
                return;
            }

            ScheludeEditWindow win = new ScheludeEditWindow(scheludes[0], this);
            win.ShowDialog();
        }
    }
}
