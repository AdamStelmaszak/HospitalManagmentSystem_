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
using System.Linq.Expressions;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Data.SqlClient;
using System.Globalization;


namespace HospitalManagmentSystem_
{
    public partial class AddShiftsView : Window
    {
        public AddShiftsView()
        {
            InitializeComponent();
            Load();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {

            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            CheckBeforeAdd();
        }

        private void Excel_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcelAndCsv();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void Load()
        {
            HospitalManagmentSystemDBEntities db = new HospitalManagmentSystemDBEntities();
            
            var queryWorkers = from d in db.workers.DefaultIfEmpty()
                       where d.user_type != "admin"
                       select new
                       {
                           IdWorker = d.id_worker,
                           Firstname = d.firstname,
                           Lastname = d.lastname,
                           UserType = d.user_type,
                           Specialization = d.specialization
                       };

            this.dtGridWorkers.ItemsSource = queryWorkers.ToList();

            var queryShifts = (from w in db.workers.DefaultIfEmpty()
                        join s in db.shifts.DefaultIfEmpty() on w.id_worker equals s.id_worker
                        where w.user_type != "admin"

                        select new
                        {
                            IdShift = s.id_shift,
                            Firstname = w.firstname,
                            Lastname = w.lastname,
                            Specialization = s.specialization_shift,
                            ShiftDate = s.date_shift
                        });

            var newResults = from result in queryShifts.ToList()

                             select new
                            {
                                  IdShift = result.IdShift,
                                  Firstname = result.Firstname,
                                  Lastname = result.Lastname,
                                  Specialization = result.Specialization,
                                  ShiftDate = result.ShiftDate.Value.GetDateTimeFormats()[3]
                             };

            this.dtGridShifts.ItemsSource = newResults.ToList();
        }



        private void Arrow_Click(object sender, RoutedEventArgs e)
        {
            new AdminView().Show();
            this.Close();
        }

        

        private void dtGrid_SelectionChanged_Worker(object sender, SelectionChangedEventArgs e)
        {
            if (dtGridWorkers.SelectedItem != null)
            {
                dynamic row = dtGridWorkers.SelectedItem;
                txtId_Worker.Text = row.IdWorker.ToString();
                txtFirstnameShift.Text = row.Firstname.ToString();
                txtLastnameShift.Text = row.Lastname.ToString();
                txtSpecializationShifts.Text = row.Specialization;            
            }
        }

        private int countDelete = 0;
        private void dtGrid_SelectionChanged_Shift(object sender, SelectionChangedEventArgs e)
        {
            if(dtGridShifts.SelectedItem != null)
            {
                dynamic row = dtGridShifts.SelectedItem;
                this.countDelete = row.IdShift;
            }
        }

        private void ExportToExcelAndCsv()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dtGridShifts.SelectAllCells();
            dtGridShifts.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dtGridShifts);
            String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            String result = (string)Clipboard.GetData(DataFormats.Text);
            dtGridShifts.UnselectAllCells();
            System.IO.StreamWriter file = new System.IO.StreamWriter(desktopPath + "\\WorkersList.xls", true);
            file.WriteLine(result.Replace(',', ' '));
            file.Close();

            MessageBox.Show("Exporting DataGrid data to Excel file ShiftsList.xls");
        }

    private void CheckBeforeAdd()
        {
            HospitalManagmentSystemDBEntities entities = new HospitalManagmentSystemDBEntities();
            using (var transaction = entities.Database.BeginTransaction())
            {
                if (!(string.IsNullOrEmpty(txtFirstnameShift.Text)) && !(string.IsNullOrEmpty(txtLastnameShift.Text))
                    && !(string.IsNullOrEmpty(txtId_Worker.Text)) && !(string.IsNullOrEmpty(dataPicker.Text)))
                {
                    DateTime beginDate = new DateTime(dataPicker.SelectedDate.Value.Year, dataPicker.SelectedDate.Value.Month,
                                        1);
                    DateTime endDate = new DateTime(dataPicker.SelectedDate.Value.Year, dataPicker.SelectedDate.Value.Month, 1).AddMonths(1);

                    int id = Int32.Parse(txtId_Worker.Text);

                    var maxDays = from w in entities.workers
                                  join s in entities.shifts on w.id_worker equals s.id_worker
                                  where s.id_worker == id
                                  where s.date_shift >= beginDate && s.date_shift < endDate
                                  select s;

                    var CannotWork_ID = from w in entities.workers
                                join s in entities.shifts on w.id_worker equals s.id_worker
                                where s.id_worker == id
                                where s.specialization_shift == txtSpecializationShifts.Text
                                where s.date_shift == dataPicker.SelectedDate
                                select s;

                    var CannotWorkWSpecialization = from w in entities.workers
                                 join s in entities.shifts on w.id_worker equals s.id_worker
                                 where s.specialization_shift == txtSpecializationShifts.Text 
                                 && s.specialization_shift != null && s.specialization_shift != "" && s.specialization_shift != "nurse"
                                 where s.date_shift == dataPicker.SelectedDate
                                 select s;

                    var dayBefoe = dataPicker.SelectedDate.Value.AddDays(-1);
                    var dayOff = from w in entities.workers
                                 join s in entities.shifts on w.id_worker equals s.id_worker
                                 where s.id_worker == id
                                 where s.date_shift == dayBefoe
                                 select s;

                    if (maxDays.Count() < 10)
                    {
                        if (CannotWork_ID.Count() >= 1)
                        {
                            MessageBox.Show("On this day, a given employee has a scheduled shift.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        else if (CannotWorkWSpecialization.Count() >= 1)
                        {
                            MessageBox.Show("An employee with a specialization " + txtSpecializationShifts.Text + " is already working on that day.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }


                        else if (dayOff.Count() >= 1)
                        {
                            MessageBox.Show("It takes 1 day off to add another shift.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                       

                        else
                        {
                            shift shiftObject = new shift()
                            {
                                id_worker = Int32.Parse(txtId_Worker.Text),
                                date_shift = dataPicker.SelectedDate,
                                specialization_shift = txtSpecializationShifts.Text
                            };

                            entities.shifts.Add(shiftObject);
                            entities.SaveChanges();
                            transaction.Commit();
                            Load();
                        }
                    }
                    else
                    {
                        MessageBox.Show("10 shifts a month have already been planned.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                else
                {
                    MessageBox.Show("Not all data has been entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dtGridWorkers.SelectedItem != null)
            {
                if (dtGridShifts.SelectedItem != null)
                {
                    Delete_Sift();
                }
                else
                {
                    MessageBox.Show("Nie robię nic.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                Delete_Sift();
            }
           
        }

        private void Delete_Sift()
        {
            HospitalManagmentSystemDBEntities db = new HospitalManagmentSystemDBEntities();
            MessageBoxResult msgBoxResult = MessageBox.Show("Are you sure you want to Delete?",
            "Delete Shift",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

            if (msgBoxResult == MessageBoxResult.Yes)
            {
                var r = from d in db.shifts
                        where d.id_shift == this.countDelete
                        select d;

                shift s = r.SingleOrDefault();

                if (s != null)
                {
                    db.shifts.Remove(s);
                    db.SaveChanges();
                    Load();
                }
            }
        }
    }
}

