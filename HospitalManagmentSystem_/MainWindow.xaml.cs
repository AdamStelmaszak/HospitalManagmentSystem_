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

namespace HospitalManagmentSystem_
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            HospitalManagmentSystemDBEntities dc = new HospitalManagmentSystemDBEntities();
            if (txtUsername.Text != string.Empty && txtPassword.Password != string.Empty)
            {
                var existuser = dc.workers.FirstOrDefault(a => a.login.Equals(txtUsername.Text));
                if (existuser != null)
                {
                    if (existuser.password.Equals(txtPassword.Password) && existuser.status_admin == true)
                    {
                        AdminView adminView = new AdminView();
                        adminView.Show();
                        this.Close();
                    }

                    else if (existuser.password.Equals(txtPassword.Password) && existuser.status_user == true)
                    {
                        UserView userView = new UserView();
                        userView.Show();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Username or password is incorrect", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Insert fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}