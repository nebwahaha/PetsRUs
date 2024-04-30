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

namespace PetsRUs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        petsrusDataContext _lsDC = null;
        string username = "";
        bool loginlog = false;

        public MainWindow()
        {
            InitializeComponent();

            _lsDC = new petsrusDataContext(
                Properties.Settings.Default.petsrusConnectionString);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            loginlog = false;

            if (txtbusername.Text.Length > 0 && txtbpass.Text.Length > 0)
            {
                var petsrus = from s in _lsDC.Staffs
                              where s.Staff_Username == txtbusername.Text
                              select s;
                if (petsrus.Count() == 1)
                {
                    foreach (var login in petsrus)
                    {
                        if (login.Staff_Password == txtbpass.Text)
                            loginlog = true;
                        username = login.Staff_Name;

                    }
                }

            }
            if (loginlog)
            {
                MessageBox.Show($"Success! Welcome pare {username}");
                MainWindow MainWindow = new MainWindow();
                MainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("username and password is incorrect");
            }
        }

    }
}
