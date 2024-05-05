using PetsRUs.Properties;
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

namespace PetsRUs
{
    public partial class Window2 : Window
    {
        private petsrusDataContext _lsDC;
        private string _username;
        private string _staffID;// Declare _username variable

        public Window2(string petType, string username)
        {
            InitializeComponent();
            _lsDC = new petsrusDataContext(Properties.Settings.Default.petsrusConnectionString);
            LoadPets(petType);
            _username = username;
            _staffID = username; // Assign _staffID the value of username
        }

        private void LoadPets(string petType)
        {
            // Retrieve data from the Pet table filtered by pet type
            var pets = (from p in _lsDC.Pets
                        where p.Pet_Type == petType
                        select new
                        {
                            p.Pet_ID,
                            p.Pet_Name,
                            p.Pet_Type,
                            p.Pet_Breed,
                            p.Pet_BirthDate,
                            p.Pet_Gender,
                            p.Vaccination_Status
                        }).ToList();

            // Bind the data to the ListView
            petListView.ItemsSource = pets;
        }
        private string GenerateCustomerID()
        {
            int count = _lsDC.Customers.Count() + 1;
            return "CS" + count.ToString("D3"); // Format count to have leading zeros if necessary
        }

        private string GenerateOrderID()
        {
            int count = _lsDC.Orders.Count() + 1;
            return "OID" + count.ToString("D3"); // Format count to have leading zeros if necessary
        }

        private void AdoptButton_Click(object sender, RoutedEventArgs e)
        {
            string customerID = GenerateCustomerID();
            string orderID = GenerateOrderID();

            if (!string.IsNullOrEmpty(customerID) && !string.IsNullOrEmpty(orderID))
            {
                // Check if a pet is selected
                if (petListView.SelectedItem != null)
                {
                    // Get the selected pet
                    dynamic selectedPet = petListView.SelectedItem;
                    string petID = selectedPet.Pet_ID;

                    Window3 window3 = new Window3(customerID, orderID, petID, _staffID);
                    window3.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please select a pet to adopt.");
                }
            }
            else
            {
                MessageBox.Show("Failed to generate Customer_ID or Order_ID.");
            }
        }
    }
}