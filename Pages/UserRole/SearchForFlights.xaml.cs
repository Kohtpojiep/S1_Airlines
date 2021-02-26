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
using S1_Вариант3.Model;
using S1_Вариант3.Classes;

namespace S1_Вариант3.Pages.UserRole
{
    /// <summary>
    /// Логика взаимодействия для SearchForFlights.xaml
    /// </summary>
    
    public partial class SearchForFlights : Page
    {
        readonly MainWindow _mainWindow;
        readonly Users _currentUser;
        public SearchForFlights(MainWindow main, Users user)
        {
            InitializeComponent();
            _mainWindow = main;
            main.Title = "Search for flights";
            _currentUser = user;

            Return_RadioButton.IsChecked = true;
            List<Countries> countries = Session1_3.GetContext().Countries.ToList();

            AirportListFrom_ComboBox.ItemsSource = countries;
            AirportListTo_ComboBox.ItemsSource = countries;
            CabinType_ComboBox.ItemsSource = new List<string> { "Economy", "Business", "First Class" };
        }
        
        private void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            
            if (!Outbound_DatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату отправления");
                return;
            }
            if (!Return_DatePicker.SelectedDate.HasValue && Return_RadioButton.IsChecked.Value == true)
            {
                MessageBox.Show("Выберите дату вылета в обратном направлении");
                return;
            }

            string from = ((Countries)AirportListFrom_ComboBox.SelectedItem).CountryShort;
            string to = ((Countries)AirportListTo_ComboBox.SelectedItem).CountryShort;
            if (from == to)
            {
                MessageBox.Show("Выбраны эквивалетные аэропорты отправления и прилета");
                return;
            }

            string cabinType = (string)CabinType_ComboBox.SelectedItem;

            DateTime outboundDate = Outbound_DatePicker.SelectedDate.Value;
            Nullable<DateTime> returnDate = Return_DatePicker.SelectedDate;
            if (returnDate < outboundDate)
            {
                MessageBox.Show("Дата вылета в обратном направлении не может быть раньше времени");
                return;
            }

            OutboundScheludes_DataGrid.ItemsSource = 
                GetFlightScheludesInfo(from: from, to: to, cabinType: cabinType, date: outboundDate, isPressedFeaturedCheckBox: OutboundDisplayOption_CheckBox.IsChecked.Value);
            if (returnDate != null)
                ReturnScheludes_DataGrid.ItemsSource = 
                    GetFlightScheludesInfo(from: to, to: from, cabinType: cabinType, date: returnDate.Value, isPressedFeaturedCheckBox: ReturnDisplayOption_CheckBox.IsChecked.Value);

            string errorMessage = null;
            if (OutboundScheludes_DataGrid.Columns.Count < 1)
                errorMessage += "Не удалось найти ни одного вылета";
            if (Return_RadioButton.IsChecked.Value == true && ReturnScheludes_DataGrid.Columns.Count < 1)
                errorMessage += errorMessage == null ? "Не удалось найти ни одного прилета" : " и прилета";

            if (errorMessage != null)
                MessageBox.Show(errorMessage);
        }

        private List<FlightDetail> GetFlightScheludesInfo(string from, string to, string cabinType, DateTime date, bool isPressedFeaturedCheckBox)
        {
            List<FlightDetail> flightDetails = new List<FlightDetail>();
            int countryFromId = Session1_3.GetContext().Countries.Where(x => x.CountryShort == from).FirstOrDefault().id;
            int countryToId = Session1_3.GetContext().Countries.Where(x => x.CountryShort == to).FirstOrDefault().id;

            List<Flight_Scheludes> fromFlightScheludes = Session1_3.GetContext().Flight_Scheludes.Where(x => x.CountryFromId == countryFromId).ToList();
            List<Flight_Scheludes> toFlightScheludes = Session1_3.GetContext().Flight_Scheludes.Where(x => x.CountryToId == countryToId ).ToList();

            if (isPressedFeaturedCheckBox == true)
            {
                fromFlightScheludes = fromFlightScheludes.Where(x => date.AddDays(-3) <= x.Date && x.Date <= date.AddDays(3)).ToList();
                toFlightScheludes = toFlightScheludes.Where(x => date.AddDays(-3) <= x.Date && x.Date <= date.AddDays(3)).ToList();
            }
            else
            {
                fromFlightScheludes = fromFlightScheludes.Where(x => date == x.Date).ToList();
                toFlightScheludes = toFlightScheludes.Where(x => date == x.Date).ToList();
            }

            for (int i = 0; i < fromFlightScheludes.Count; i++)
            {
                if (fromFlightScheludes[i].CountryToId == countryToId)
                {
                    flightDetails.Add(
                        new FlightDetail
                        {
                            From = from,
                            To = to,
                            Date = fromFlightScheludes[i].Date.ToShortDateString(),
                            Time = fromFlightScheludes[i].Time.ToString(),
                            FlightNumbers = $"[{fromFlightScheludes[i].FlightNumber}]",
                            Price = (cabinType == "Economy" ? fromFlightScheludes[i].EconomyPrice :
                                (cabinType == "Business" ? fromFlightScheludes[i].BusinessPrice : fromFlightScheludes[i].FirstClassPrice)).ToString(),
                            NumberOfStops = 0,
                            UsingScheludes = new List<Flight_Scheludes> { fromFlightScheludes[i] }
                        });
                }
                else
                {
                    for (int j = 0; j < toFlightScheludes.Count; j++)
                    {
                        if (fromFlightScheludes[i].CountryToId == toFlightScheludes[j].CountryFromId)
                        {
                            if ((fromFlightScheludes[i].Date < toFlightScheludes[j].Date) || 
                                    (fromFlightScheludes[i].Date == toFlightScheludes[j].Date && fromFlightScheludes[i].Time < toFlightScheludes[j].Time))
                                flightDetails.Add(
                                new FlightDetail
                                {
                                    From = from,
                                    To = to,
                                    Date = fromFlightScheludes[i].Date.ToShortDateString(),
                                    Time = fromFlightScheludes[i].Time.ToString(),
                                    FlightNumbers = $"[{fromFlightScheludes[i].FlightNumber}] - [{toFlightScheludes[j].FlightNumber}]",
                                    Price = "$ " + (cabinType == "Economy" ? fromFlightScheludes[i].EconomyPrice + toFlightScheludes[j].EconomyPrice :
                                        cabinType == "Business" ? fromFlightScheludes[i].BusinessPrice + toFlightScheludes[j].BusinessPrice :
                                            fromFlightScheludes[i].FirstClassPrice + toFlightScheludes[j].FirstClassPrice),
                                    NumberOfStops = 1,
                                    UsingScheludes = new List<Flight_Scheludes> { fromFlightScheludes[i], toFlightScheludes[j] }
                                });
                        }
                    }
                }
            }
            return flightDetails;
        }

        private void BookingFlight_Button_Click(object sender, RoutedEventArgs e)
        {
            int passengerCount = -1;
            if (!int.TryParse(PassengersCount_TextBox.Text, out passengerCount))
            {
                MessageBox.Show("Неправильный формат количества пассажиров");
                return;
            }
            if (passengerCount < 1 || passengerCount > 20)
            {
                MessageBox.Show("Ошибочное количество пассажиров");
                return;
            }
            FlightDetail outboundSchelude = OutboundScheludes_DataGrid.SelectedItems.Cast<FlightDetail>().SingleOrDefault();
            FlightDetail returnSchelude = ReturnScheludes_DataGrid.SelectedItems.Cast<FlightDetail>().SingleOrDefault();
            if (outboundSchelude == null)
            {
                MessageBox.Show("Выберите рейс для вылета");
                return;
            }
            if (returnSchelude == null && ReturnDisplayOption_CheckBox.IsChecked.Value)
            {
                MessageBox.Show("Выберите рейс для прилета");
                return;
            }
            if (returnSchelude != null)
                if (Convert.ToDateTime(returnSchelude.Date) < Convert.ToDateTime(outboundSchelude.Date))
                {
                    MessageBox.Show("Дата возвращения раньше чем дата вылета");
                    return;
                }    

            this.NavigationService.Navigate(new BookingConfirmation(passengerCount, CabinType_ComboBox.SelectedItem.ToString(), outboundSchelude, returnSchelude));
        }

        private void OneWay_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ReturnScheludes_DataGrid.IsEnabled = false;
            Return_DatePicker.IsEnabled = false;
        }

        private void Return_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ReturnScheludes_DataGrid.IsEnabled = true;
            Return_DatePicker.IsEnabled = true;
        }
    }
}
