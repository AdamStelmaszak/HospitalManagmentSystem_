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
using System.Data.SqlClient;

namespace HospitalManagmentSystem_
{
    /// <summary>
    /// Logika interakcji dla klasy UserView.xaml
    /// </summary>
    public partial class UserView : Window
    {
        public UserView()
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


        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void Load()
        {
            HospitalManagmentSystemDBEntities db = new HospitalManagmentSystemDBEntities();

            var query = (from d in db.workers.DefaultIfEmpty()
                       join s in db.shifts on d.id_worker equals s.id_worker
                       where d.user_type != "admin"
                       
                       select new
                       {
                           IdWorker = d.id_worker,
                           IdShift = s.id_shift,
                           Firstname = d.firstname,
                           Lastname = d.lastname,
                           UserType = d.user_type,
                           Specialization = d.specialization,
                           ShiftDate = s.date_shift
                       });

            var newResults = from result in query.ToList()

                             select new
                             {
                                 IdWorker = result.IdWorker,
                                 IdShift = result.IdShift,
                                 Firstname = result.Firstname,
                                 Lastname = result.Lastname,
                                 UserType = result.UserType,
                                 Specialization = result.Specialization,
                                 ShiftDate = result.ShiftDate.Value.GetDateTimeFormats()[3]
                             };

            dtGrid.Visibility = Visibility.Visible;
            this.dtGrid.ItemsSource = newResults.ToList();
        }

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcelAndCsv();
        }

        private void ExportToExcelAndCsv()
        {
            dtGrid.SelectAllCells();
            dtGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dtGrid);
            String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            String result = (string)Clipboard.GetData(DataFormats.Text);
            dtGrid.UnselectAllCells();
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\stelm\source\repos\HospitalManagmentSystem_\excel\ShiftsOfDoctorsAndNurses.xls");
            file.WriteLine(result.Replace(',', ' '));
            file.Close();

            MessageBox.Show("Exporting DataGrid data to Excel file ShiftsOfDoctorsAndNurses.xls");
        }
    }
}
