﻿using System;
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
    public partial class Window3 : Window
    {
        private petsrusDataContext _lsDC;
        private string _customerID;
        private string _orderID;
        private string _petID;
        private string _username;
        private string _staffID;// New variable to store the username

        // Constructor with three arguments: customerID, orderID, and petID
        public Window3(string customerID, string orderID, string petID)
        {
            InitializeComponent();
            _lsDC = new petsrusDataContext(Properties.Settings.Default.petsrusConnectionString);
            _customerID = customerID;
            _orderID = orderID;
            _petID = petID;
        }

        // Constructor with four arguments: customerID, orderID, petID, and username
        public Window3(string customerID, string orderID, string petID, string staffID)
        {
            InitializeComponent();
            _lsDC = new petsrusDataContext(Properties.Settings.Default.petsrusConnectionString);
            _customerID = customerID;
            _orderID = orderID;
            _petID = petID;
            _staffID = staffID; // Use the value of staffID passed from Window2
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string customerName = txtCustomerName.Text;
            string contactNumber = txtContactNumber.Text;

            // Validate input
            if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(contactNumber))
            {
                MessageBox.Show("Please fill all fields correctly.");
                return;
            }

            try
            {
                // Insert into Customers table
                Customer newCustomer = new Customer
                {
                    Customer_ID = _customerID,
                    Customer_Name = customerName,
                    Contact_Number = contactNumber,
                };
                _lsDC.Customers.InsertOnSubmit(newCustomer);

                // Insert into Orders table
                Order newOrder = new Order
                {
                    Order_ID = _orderID,
                    Customer_ID = _customerID,
                    Staff_ID = _username,
                    Order_Desc = "Adopting",
                    Order_Date = DateTime.Now,
                    Order_Status = "Pending",
                    Pet_ID = _petID // Assuming you have stored the selected pet ID
                };
                _lsDC.Orders.InsertOnSubmit(newOrder);

                // Update Pet table to set adoption status to "AS2"
                var adoptedPets = from pet in _lsDC.Pets
                                  where pet.Pet_ID == _petID
                                  select pet;

                foreach (var pet in adoptedPets)
                {
                    pet.AdoptionStatus_ID = "AS2";
                }

                // Submit changes to the database
                _lsDC.SubmitChanges();

                MessageBox.Show("Order confirmed successfully.");
                this.Close();

                // Open Window4 for payment processing
                Window4 window4 = new Window4(_orderID);
                window4.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        //private string GeneratePaymentID()
        //{
        //    // Generate a unique Payment_ID
        //    int count = _lsDC.Payments.Count() + 1;
        //    return "PID" + count.ToString("D3"); // Format count to have leading zeros if necessary
        //}
    }
}
