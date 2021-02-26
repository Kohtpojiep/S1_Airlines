using S1_Вариант3.Model;
using S1_Вариант3.Pages;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows.Threading;

namespace S1_Вариант3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        // объявление параметров для хранения данных о сессии
        readonly DispatcherTimer _spendTimer = new DispatcherTimer(); // таймер для подсчёта проведенного времени
        public Loginings currentLog; // экземпляр класса текущей сессии
        int _spendOn; // хранит проведенное за сессию время

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                MainFrame.Navigate(new Autorisation(this));
            }
            catch (Exception ex) // в случае фатальной ошибки - записываем данные о сессии в логах
            {
                if (currentLog != null)
                    Logout_User(ex.Message);
            }
        }

        private void TimerTick(object sender, EventArgs e) => _spendOn++; // добавление проведенного времени

        // выход пользователя из системы
        public void Logout_User(string reason = null)
        {
            _spendTimer.Stop();
            currentLog.Logout_time = DateTime.Now.TimeOfDay;
            currentLog.Time_spent_on = _spendOn;
            if (reason != null)
            {
                currentLog.Error_reason = reason;
            }
            Session1_3.GetContext().Loginings.AddOrUpdate(currentLog);
            Session1_3.GetContext().SaveChanges();
            currentLog = null;
            _spendOn = 0;
        }

        // пользователь зашёл в систему
        public void Log_Autorisation(Users user)
        {
            _spendTimer.Tick += TimerTick;
            _spendTimer.Interval = new TimeSpan(0, 0, 1);
            _spendTimer.Start();

            currentLog = new Loginings
            {
                User_id = user.id,
                Date = DateTime.Now.Date,
                Login_time = DateTime.Now.TimeOfDay
            };

            Session1_3.GetContext().Loginings.AddOrUpdate(currentLog);
            Session1_3.GetContext().SaveChanges();
            currentLog = Session1_3.GetContext().Loginings.FirstOrDefault(x => x.Date == currentLog.Date & x.Login_time == currentLog.Login_time & x.User_id == currentLog.User_id);
        }

        // в случае, если пользователь выходит с помощью крестика
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (currentLog != null)
                Logout_User("Аварийное закрытие окна");
        }
    }
}
