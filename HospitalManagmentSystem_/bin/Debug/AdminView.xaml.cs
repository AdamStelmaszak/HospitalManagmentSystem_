using System;
using System.Collections.Generic;
using System.IO;
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

    public partial class AdminView : Window
    {

        readonly HospitalManagmentSystemDBEntities db = new HospitalManagmentSystemDBEntities();
        public AdminView()
        {
            InitializeComponent();
            Load();
        }


        private void Default_Click(object sender, RoutedEventArgs e)
        {
            ClearText();
        }

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcelAndCsv();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!(string.IsNullOrEmpty(txtFirstname.Text)) && !(string.IsNullOrEmpty(txtLastname.Text)) && !(string.IsNullOrEmpty(txtPesel.Text)) &&
                !(string.IsNullOrEmpty(UserTypeComboBox.Text)) && !(string.IsNullOrEmpty(txtLogin.Text)) && !(string.IsNullOrEmpty(txtPassword.Text)) &&
                !(string.IsNullOrEmpty(StatusAdminComboBox.Text)) && !(string.IsNullOrEmpty(StatusUserComboBox.Text)))
            {
                using (var transaction = db.Database.BeginTransaction())
                {

                   
                    var pesel = Int64.Parse(txtPesel.Text);
                    var pwz = txtPWZ.Text;
                    var query1 = from w in db.workers
                                where w.login == txtLogin.Text && w.password == txtPassword.Text
                                select w;

                    var query2 = from w in db.workers
                                 where w.pesel == pesel
                                 select w;

                    var query3 = from w in db.workers
                                 where w.pwz == Int32.Parse(txtPWZ.Text)
                                 select w;


                    if (query1.Count() >= 1)
                    {
                        MessageBox.Show("The given login and password are taken. Make changes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    

                    else if(query2.Count() >= 1)
                    {
                        MessageBox.Show("User with the given pesel already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error); 
                    }
                    else
                    {
                        if ((UserTypeComboBox.Text is "admin" && StatusAdminComboBox.Text == "True" && StatusUserComboBox.Text == "False") ||
                    (UserTypeComboBox.Text is "nurse" && StatusAdminComboBox.Text == "False" && StatusUserComboBox.Text == "True"))
                        {
                            if (string.IsNullOrEmpty(SpecializationComboBox.Text) && string.IsNullOrEmpty(txtPWZ.Text))
                            {
                                worker workerObject = new worker()
                                {
                                    firstname = txtFirstname.Text,
                                    lastname = txtLastname.Text,
                                    pesel = Int64.Parse(txtPesel.Text),
                                    user_type = UserTypeComboBox.Text,
                                    login = txtLogin.Text,
                                    password = txtPassword.Text,
                                    status_admin = Boolean.Parse(StatusAdminComboBox.Text),
                                    status_user = Boolean.Parse(StatusUserComboBox.Text)
                                };

                                db.workers.Add(workerObject);
                                db.SaveChanges();
                                transaction.Commit();
                                Load();
                            }

                            else
                            {
                                MessageBox.Show("You can't enter the values ​​of Specialization, PWZ, if the user is not a doctor.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }

                        else if (UserTypeComboBox.Text is "doctor" && StatusAdminComboBox.Text == "False" && StatusUserComboBox.Text == "True" &&
                                !(string.IsNullOrEmpty(SpecializationComboBox.Text)) && !(string.IsNullOrEmpty(txtPWZ.Text)))
                        {
                            if(query3.Count() >= 1)
                            {
                                MessageBox.Show("Doctor with the given PWZ already exists. Make changes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                            else
                            {
                                worker workerObject = new worker()
                                {
                                    firstname = txtFirstname.Text,
                                    lastname = txtLastname.Text,
                                    pesel = Int64.Parse(txtPesel.Text),
                                    user_type = UserTypeComboBox.Text,
                                    login = txtLogin.Text,
                                    password = txtPassword.Text,
                                    status_admin = Boolean.Parse(StatusAdminComboBox.Text),
                                    status_user = Boolean.Parse(StatusUserComboBox.Text),
                                    specialization = SpecializationComboBox.Text,
                                    pwz = Int32.Parse(txtPWZ.Text)
                                };

                                db.workers.Add(workerObject);
                                db.SaveChanges();
                                transaction.Commit();
                                Load();
                            }
                        }

                        else
                        {
                            if (SpecializationComboBox.Text != "doctor")
                            {
                                MessageBox.Show("User status selected incorrectly.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                            else
                            {
                                MessageBox.Show("Incorrectly completed doctor's data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }

                }  
            }

            else
            {
                MessageBox.Show("Not all data has been entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private int updatingWorker = 0;
        private void dtGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtGrid.SelectedItem != null)
            {
                dynamic row = dtGrid.SelectedItem;
                this.updatingWorker = row.IdWorker;
                txtFirstname.Text = row.Firstname.ToString();
                txtLastname.Text = row.Lastname.ToString();
                txtPesel.Text = row.PESEL.ToString();
                UserTypeComboBox.Text = row.UserType.ToString();
                txtLogin.Text = row.Login;
                txtPassword.Text = row.Password;
                StatusAdminComboBox.Text = row.StatusAdmin;
                StatusUserComboBox.Text = row.StatusUser;
                SpecializationComboBox.Text = row.Specialization;
                txtPWZ.Text = row.PWZ;
            }
        }

        private void btnUpdateWorkers_Click(object sender, RoutedEventArgs e)
        {
            var r = from d in db.workers
                    where d.id_worker == this.updatingWorker
                    select d;

            worker w = r.SingleOrDefault();

            if (!(string.IsNullOrEmpty(txtFirstname.Text)) && !(string.IsNullOrEmpty(txtLastname.Text)) && !(string.IsNullOrEmpty(txtPesel.Text)) &&
                !(string.IsNullOrEmpty(UserTypeComboBox.Text)) && !(string.IsNullOrEmpty(txtLogin.Text)) && !(string.IsNullOrEmpty(txtPassword.Text)) &&
                !(string.IsNullOrEmpty(StatusAdminComboBox.Text)) && !(string.IsNullOrEmpty(StatusUserComboBox.Text)))
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    var pesel = Int64.Parse(txtPesel.Text);
                    var pwz = Int64.Parse(txtPWZ.Text);
                    
                    var query1 = from work in db.workers
                                 where work.login == txtLogin.Text && work.password == txtPassword.Text
                                 where work.id_worker != this.updatingWorker
                                 select work;

                    var query2 = from work in db.workers
                                 where work.pesel == pesel
                                 where work.id_worker != this.updatingWorker
                                 select work;

                    var query3 = from work in db.workers
                                 where work.pwz == pwz
                                 where work.id_worker != this.updatingWorker
                                 select work;


                    if (query1.Count() >= 1)
                    {
                        MessageBox.Show("The given login and password are taken. Make changes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    else if (query2.Count() >= 1)
                    {
                        MessageBox.Show("User with the given pesel already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        if ((UserTypeComboBox.Text is "admin" && StatusAdminComboBox.Text == "True" && StatusUserComboBox.Text == "False") ||
                   (UserTypeComboBox.Text is "nurse" && StatusAdminComboBox.Text == "False" && StatusUserComboBox.Text == "True"))
                        {
                            if (string.IsNullOrEmpty(SpecializationComboBox.Text) && string.IsNullOrEmpty(txtPWZ.Text))
                            {
                                w.firstname = this.txtFirstname.Text;
                                w.lastname = this.txtLastname.Text;
                                w.pesel = Int64.Parse(this.txtPesel.Text);
                                w.user_type = this.UserTypeComboBox.Text;
                                w.login = this.txtLogin.Text;
                                w.password = this.txtPassword.Text;
                                w.status_admin = Boolean.Parse(StatusAdminComboBox.Text);
                                w.status_user = Boolean.Parse(StatusUserComboBox.Text);
                                db.SaveChanges();
                                Load();
                            }

                            else
                            {
                                MessageBox.Show("You can't enter the values ​​of Specialization, PWZ and Specialization ID, if the user is not a doctor.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }

                        else if (UserTypeComboBox.Text is "doctor" && StatusAdminComboBox.Text == "False" && StatusUserComboBox.Text == "True" &&
                                !(string.IsNullOrEmpty(SpecializationComboBox.Text)) && !(string.IsNullOrEmpty(txtPWZ.Text)))
                        {
                            if(query3.Count() >= 1)
                            {
                                MessageBox.Show("Doctor with the given PWZ already exists. Make changes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else if (w == null)
                            {
                                MessageBox.Show("There is no user with the selected data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                    w.firstname = this.txtFirstname.Text;
                                    w.lastname = this.txtLastname.Text;
                                    w.pesel = Int64.Parse(this.txtPesel.Text);
                                    w.user_type = this.UserTypeComboBox.Text;
                                    w.login = this.txtLogin.Text;
                                    w.password = this.txtPassword.Text;
                                    w.status_admin = Boolean.Parse(StatusAdminComboBox.Text);
                                    w.status_user = Boolean.Parse(StatusUserComboBox.Text);
                                    w.specialization = SpecializationComboBox.Text;
                                    w.pwz = Int32.Parse(txtPWZ.Text);
                                    db.SaveChanges();
                                    transaction.Commit();
                                    Load();
                            }
                        }

                        else
                        {
                            if (SpecializationComboBox.Text != "doctor")
                            {
                                MessageBox.Show("User status selected incorrectly.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                            else
                            {
                                MessageBox.Show("Incorrectly completed doctor's data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }

            else
            {
                MessageBox.Show("Not all data has been entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgBoxResult = MessageBox.Show("Are you sure you want to Delete?",
            "Delete Worker",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

            if (msgBoxResult == MessageBoxResult.Yes)
            {
                var itemToRemove = db.workers.SingleOrDefault(x => x.id_worker == this.updatingWorker);

                if (itemToRemove != null)
                {
                    db.workers.Remove(itemToRemove);
                    db.SaveChanges();
                    Load();
                }
            }
        }

        private void Excel_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcelAndCsv();
        }

        private void Load()
        {
            var result = from w in db.workers.DefaultIfEmpty()
                         select new
                         {
                             IdWorker = w.id_worker,
                             Firstname = w.firstname,
                             Lastname = w.lastname,
                             PESEL = w.pesel.ToString(),
                             UserType = w.user_type,
                             Login = w.login.ToString(),
                             Password = w.password.ToString(),
                             StatusAdmin = w.status_admin.ToString(),
                             StatusUser = w.status_user.ToString(),
                             Specialization = w.specialization,
                             PWZ = w.pwz.ToString()
                         };

                this.dtGrid.ItemsSource = result.ToList();
        }

        private void NumericOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);
        }

        private static bool IsTextNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^0-9]");
            return reg.IsMatch(str);
        }

        private void ClearText()
        {
            this.txtFirstname.Text = "";
            this.txtLastname.Text = "";
            this.txtPesel.Text = "";
            this.UserTypeComboBox.Text = "";
            this.txtLogin.Text = "";
            this.txtPassword.Text = "";
            this.StatusAdminComboBox.Text = "";
            this.StatusUserComboBox.Text = "";
            this.SpecializationComboBox.Text = "";
            this.txtPWZ.Text = "";
        }

        private void ExportToExcelAndCsv()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dtGrid.SelectAllCells();
            dtGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dtGrid);
            String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            String result = (string)Clipboard.GetData(DataFormats.Text);
            dtGrid.UnselectAllCells();
            System.IO.StreamWriter file = new System.IO.StreamWriter(desktopPath + "\\WorkersList.xls", true);
            file.WriteLine(result.Replace(',', ' '));
            file.Close();

            MessageBox.Show("Exporting DataGrid data to Excel file WorkersList.xls");
        }

        private void Shifts_Click(object sender, RoutedEventArgs e)
        {
            new AddShiftsView().Show();
            this.Close();
        }

        private void btnLoadWorkers_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }
    }
}
