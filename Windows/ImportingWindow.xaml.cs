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
using Microsoft.Win32;
using System.IO;
using S1_Вариант3.Model;
using System.Security.Cryptography;
using System.Data.Entity.Migrations;

namespace S1_Вариант3.Windows
{
    /// <summary>
    /// Логика взаимодействия для ImportingWindow.xaml
    /// </summary>
    public partial class ImportingWindow : Window
    {
        public ImportingWindow()
        {
            InitializeComponent();
        }

        private string GetFilePath()
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                CheckFileExists = false,
                CheckPathExists = true,
                Multiselect = false,
                Title = "Выберите файл",
                DefaultExt = "csv"
            };

            if (dialog.ShowDialog() != null)
            {
                if (dialog.FileName.EndsWith(".csv"))
                    return dialog.FileName;
                else
                    return null;
            }
            return null;
        }

        private void Import_Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> lines = new List<string>();
            List<Flight_Scheludes> importList = new List<Flight_Scheludes>();
            List<Flight_Scheludes> updateList = new List<Flight_Scheludes>();
            int successfulLineCount = 0, dublicateLineCount = 0, missingLinesCount = 0;
            string filePath = GetFilePath();
            if (filePath != null)
            {
                if (!filePath.EndsWith(".csv"))
                {
                    MessageBox.Show("Выбран файл неправильного формата");
                    return;
                }

                Filename_TextBox.Text = filePath.Split('\\')[filePath.Split('\\').Length - 1];

                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (lines.Contains(line))
                        {
                            dublicateLineCount++;
                            continue;
                        }
                        lines.Add(line);
                        string[] data = line.Contains(';') ? line.Split(';') : line.Split(',');
                        Flight_Scheludes currentSchelude = null;
                        
                        string nameCountry = data[4];
                        string nameCountry2 = data[5];
                        
                        Countries countryFrom = Session1_3.GetContext().Countries.FirstOrDefault(x => x.CountryShort == nameCountry);
                        if (countryFrom == null)
                        {
                            Session1_3.GetContext().Countries.Add(new Countries { Country = nameCountry, CountryShort = nameCountry });
                            Session1_3.GetContext().SaveChanges();
                            countryFrom = Session1_3.GetContext().Countries.FirstOrDefault(x => x.CountryShort == nameCountry);
                        }
                        Countries countryTo = Session1_3.GetContext().Countries.FirstOrDefault(x => x.CountryShort == nameCountry2);
                        if (countryTo == null)
                        {
                            Session1_3.GetContext().Countries.Add(new Countries { Country = nameCountry2, CountryShort = nameCountry2 });
                            Session1_3.GetContext().SaveChanges();
                            countryTo = Session1_3.GetContext().Countries.FirstOrDefault(x => x.CountryShort == nameCountry2);
                        }
                        try
                        {
                            DateTime dt = data[1].Contains('.') ? DateTime.ParseExact(data[1], "dd.MM.yyyy", null) : DateTime.ParseExact(data[1], "yyyy-MM-dd", null);
                            short smth = Convert.ToInt16(data[3]);
                            int aircraftId = Convert.ToInt32(data[6]);
                            Aircraft aircraft = Session1_3.GetContext().Aircraft.FirstOrDefault(x => x.id == aircraftId);
                            if (aircraft == null)
                            {
                                Session1_3.GetContext().Aircraft.Add(new Aircraft { id = aircraftId, Model = aircraftId.ToString() });
                                Session1_3.GetContext().SaveChanges();
                                aircraft = aircraft = Session1_3.GetContext().Aircraft.FirstOrDefault(x => x.id == aircraftId);
                            }

                            if (data[0].ToLower() == "edit")
                            {
                                currentSchelude = Session1_3.GetContext().Flight_Scheludes.FirstOrDefault(x => x.Date.ToString() == dt.ToString() && x.FlightNumber == smth);
                                if (currentSchelude != null)
                                {
                                    currentSchelude.Date = dt;
                                    currentSchelude.Time = TimeSpan.Parse(data[2]);
                                    currentSchelude.FlightNumber = Convert.ToInt16(data[3]);
                                    currentSchelude.CountryFromId = countryFrom.id;
                                    currentSchelude.CountryToId = countryTo.id;
                                    currentSchelude.AircraftId = aircraftId;
                                    currentSchelude.EconomyPrice = Convert.ToInt16(Convert.ToDouble(data[7].Replace('.', ',')));
                                    currentSchelude.IsAccepted = data[8].ToLower() == "ok";

                                    successfulLineCount++;
                                    updateList.Add(currentSchelude);
                                }
                            }

                            if (currentSchelude == null)
                            {
                                currentSchelude = new Flight_Scheludes
                                {
                                    Date = dt,
                                    Time = TimeSpan.Parse(data[2]),
                                    FlightNumber = Convert.ToInt16(data[3]),
                                    CountryFromId = countryFrom.id,
                                    CountryToId = countryTo.id,
                                    AircraftId = Convert.ToInt32(data[6]),
                                    EconomyPrice = Convert.ToDecimal(data[7].Replace('.', ',')),
                                    IsAccepted = data[8].ToLower() == "ok"
                                };

                                successfulLineCount++;
                                importList.Add(currentSchelude);
                            }
                        }
                        catch
                        {
                           missingLinesCount++;
                        }
                    }
                }

                for (int i = 0; i < importList.Count; i++)
                    Session1_3.GetContext().Flight_Scheludes.Add(importList[i]);
                for (int i = 0; i < updateList.Count; i++)
                    Session1_3.GetContext().Flight_Scheludes.AddOrUpdate(updateList[i]);

                Session1_3.GetContext().SaveChanges();

                SuccessfulCount_Label.Content = successfulLineCount;
                DublicateCount_Label.Content = dublicateLineCount;
                MissingCount_Label.Content = missingLinesCount;
            }
            else
            {
                MessageBox.Show("Не выбран файл");
                return;
            }
        }
    }
}
