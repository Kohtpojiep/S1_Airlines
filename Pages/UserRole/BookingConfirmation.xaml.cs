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
    /// Логика взаимодействия для BookingConfirmation.xaml
    /// </summary>
    public partial class BookingConfirmation : Page
    {
        private int _peoplesCount;
        private string _cabinType = "";
        private List<Persons> _passengersList = new List<Persons>();
        private List<Bookings> _currentBooking = new List<Bookings>();

        public BookingConfirmation(int peopleCount, string cabinType, FlightDetail outboundFlight, FlightDetail returnFlight)
        {
            InitializeComponent();
            Country_ComboBox.ItemsSource = Session1_3.GetContext().Countries.ToList();
            _peoplesCount = peopleCount;
            _cabinType = cabinType;

            OutboundCabinType_Label.Content += $" {cabinType}";
            OutboundFromCountry_Label.Content += $" {outboundFlight.From}";
            OutboundToCountry_Label.Content += $" {outboundFlight.To}";
            OutboundDate_Label.Content += $" {outboundFlight.Date}";
            OutboundFlightNumber_Label.Content += $" {outboundFlight.FlightNumbers}";
            for (int i = 0; i < outboundFlight.UsingScheludes.Count; i++)
                _currentBooking.Add(new Bookings { Flight_Schelude_id = outboundFlight.UsingScheludes[i].id });

            if (returnFlight != null)
            {
                ReturnCabinType_Label.Content += $" {cabinType}";
                ReturnFromCountry_Label.Content += $" {returnFlight.From}";
                ReturnToCountry_Label.Content += $" {returnFlight.To}";
                ReturnDate_Label.Content += $" {returnFlight.Date}";
                ReturnFlightNumber_Label.Content += $" {returnFlight.FlightNumbers}";
                for (int i = 0; i < returnFlight.UsingScheludes.Count; i++)
                    _currentBooking.Add(new Bookings { Flight_Schelude_id = returnFlight.UsingScheludes[i].id });
            }
        }

        private void BackToSearchFlights_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msg = MessageBox.Show(caption: "Attention", button: MessageBoxButton.YesNo, messageBoxText: "Are you sure?");
            if (msg == MessageBoxResult.Yes)
            {
                this.NavigationService.GoBack();
            }
        }

        private void ConfirmBooking_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_peoplesCount != Passengers_DataGrid.Items.Count)
            {
                MessageBox.Show($"Кол-во пассажиров не соответствует заданному ранее количеству. А именно {_peoplesCount}.");
                return;
            }
            Session1_3.GetContext().Bookings.AddRange(_currentBooking);
            Session1_3.GetContext().SaveChanges();

            new Windows.ConfirmBookingWindow(_currentBooking, _cabinType).ShowDialog();
        }

        private void RemovePassenger_Button_Click(object sender, RoutedEventArgs e)
        {
            Persons person = Passengers_DataGrid.SelectedItems.Cast<Persons>().SingleOrDefault();
            if (person == null)
            {
                MessageBox.Show("Выберите пассажира для удаления");
                return;
            }
            _passengersList.Remove(person);
            if(_passengersList.Count(x => x.id == person.id) > 0)
            {
                for (int i = 0; i < _currentBooking.Count; i++)
                {
                    _currentBooking[i].Booking_Persons.First(x => x.Person_id == person.id).Ticket_count--;
                }
            }
            else
            {
                for (int i = 0; i < _currentBooking.Count; i++)
                {
                    _currentBooking[i].Booking_Persons.Remove(
                        _currentBooking[i].Booking_Persons.First(x => x.Person_id == person.id));
                }
            }
            
            Session1_3.GetContext().Persons.Remove(person);
            Session1_3.GetContext().SaveChanges();

            Passengers_DataGrid.ItemsSource = null;
            Passengers_DataGrid.ItemsSource = _passengersList;
        }

        private void AddPassenger_Button_Click(object sender, RoutedEventArgs e)
        {
            bool isCorrect = true;
            foreach (var item in PersonInputs_Grid.Children)
            {
                if (item is TextBox textBox)
                    if (string.IsNullOrWhiteSpace(textBox.Text))
                    { 
                        isCorrect = false;
                        break;
                    }
                if (item is DatePicker datePicker)
                    if (datePicker.SelectedDate == null)
                    {
                        isCorrect = false;
                        break;
                    }
            }
            if (isCorrect == false)
            {
                MessageBox.Show("Все поля должны быть заполнены");
                return;
            }
            string firstname = Firstname_TextBox.Text, lastname = Lastname_TextBox.Text;
            int passportNumber = -1, countryId = ((Countries)Country_ComboBox.SelectedItem).id;
            long phone = -1;
            DateTime birthdate = Birthdate_DatePicker.SelectedDate.Value;

            if (!int.TryParse(PassportNumber_TextBox.Text, out passportNumber))
            {
                MessageBox.Show("Неправильный формат паспорта");
                return;
            }
            if (!long.TryParse(Phone_TextBox.Text, out phone))
            {
                MessageBox.Show("Неправильный формат номера телефона");
                return;
            }

            Persons person = Session1_3.GetContext().Persons.SingleOrDefault(
                x => x.Firstname == firstname && x.Lastname == lastname && x.Passport_number == passportNumber 
                     && x.Passport_Country == countryId && x.Phone == phone && x.Birthdate == birthdate);

            if (person == null)
            {
                person = new Persons
                {
                    Firstname = firstname,
                    Lastname = lastname,
                    Passport_number = passportNumber,
                    Passport_Country = countryId,
                    Phone = phone,
                    Birthdate = birthdate
                };
                Session1_3.GetContext().Persons.Add(person);
                Session1_3.GetContext().SaveChanges();
            }
            _passengersList.Add(person);
            if(_passengersList.Count(x => x.id == person.id) > 1)
            {
                for (int i = 0; i < _currentBooking.Count; i++)
                {
                    _currentBooking[i].Booking_Persons.First(x => x.Person_id == person.id).Ticket_count++;
                }
            }
            else
            {
                for (int i = 0; i < _currentBooking.Count; i++)
                {
                    _currentBooking[i].Booking_Persons.Add(new Booking_Persons { Booking_id = _currentBooking[i].id, Person_id = person.id, Ticket_count = 1});
                }
            }

            Passengers_DataGrid.ItemsSource = null;
            Passengers_DataGrid.ItemsSource = _passengersList;
        }
    }
}
