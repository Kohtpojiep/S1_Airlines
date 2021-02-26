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

namespace S1_Вариант3.Windows
{
    /// <summary>
    /// Логика взаимодействия для ConfirmBookingWindow.xaml
    /// </summary>
    public partial class ConfirmBookingWindow : Window
    {
        List<Bookings> _currentBookings;
        double[] _prices;
        private int counts;
        public ConfirmBookingWindow(List<Bookings> bookings, string cabinType)
        {
            InitializeComponent();
            _currentBookings = bookings;
            _prices = new double[bookings.Count];

            int bookingId = bookings[0].id;
            counts = Session1_3.GetContext().Booking_Persons
                .Where(x => x.Booking_id == bookingId).ToList().Sum(x => x.Ticket_count);

            switch (cabinType)
            {
                case "Economy":
                    for (int i = 0; i < bookings.Count; i++)
                        _prices[i] = Convert.ToDouble(bookings[i].Flight_Scheludes.EconomyPrice);
                    break;
                case "Business":
                    for (int i = 0; i < bookings.Count; i++)
                        _prices[i] = Convert.ToDouble(bookings[i].Flight_Scheludes.BusinessPrice);
                    break;
                case "First Class":
                    for (int i = 0; i < bookings.Count; i++)
                        _prices[i] = Convert.ToDouble(bookings[i].Flight_Scheludes.FirstClassPrice);
                    break;
            }
            
            TotalAmount_Label.Content += $"[ {_prices.Sum() * counts}$ ]";
        }

        private string GetCorrectId(int id)
        {
            int length = 3 - id.ToString().Length;
            string result = id.ToString();
            for (int i = 1; i <= length; i++)
                result = "X" + result;
            return result;
        }

        private void IssueTickets_Button_Click(object sender, RoutedEventArgs e)
        {
            Billings currentBillings = Session1_3.GetContext().Billings.ToList().LastOrDefault();
            int billingsId = currentBillings == null ? 1 : currentBillings.id + 1;
            string payment = CreditCard_RadioButton.IsChecked.Value ? "Credit Card" : Cash_RadioButton.IsChecked.Value ? "Cash" : "Voucher";

            TicketNumbers_Label.Content = "Ticket numbers:\n";
            for (int i = 0; i < _currentBookings.Count; i++)
            {
                TicketNumbers_Label.Content += $"{GetCorrectId(billingsId)}{GetCorrectId(_currentBookings[i].id)}\n";
                Session1_3.GetContext().Billings.Add(new Billings
                {
                    id = (int) billingsId,
                    Booking_id = _currentBookings[i].id,
                    //Paid_using = payment, 
                    Amount = Convert.ToInt32(_prices[i] * counts)
                });
            }

            Session1_3.GetContext().SaveChanges();

            MessageBox.Show("Успешно.");
            this.Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msg = MessageBox.Show(caption: "Attention", button: MessageBoxButton.YesNo, messageBoxText: "Are you sure?");
            if (msg == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
    }
}
