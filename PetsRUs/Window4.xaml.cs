using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace PetsRUs
{
    public partial class Window4 : Window
    {
        private petsrusDataContext _lsDC;
        private string _orderID;

        public Window4(string orderID)
        {
            InitializeComponent();
            _orderID = orderID;
            _lsDC = new petsrusDataContext(Properties.Settings.Default.petsrusConnectionString);

            // Generate Payment_ID
            string paymentID = GeneratePaymentID();
            txtPaymentID.Text = paymentID;

            // Set Total Amount
            txtTotalAmount.Text = "5000"; // Fixed price for adopting a pet
        }

        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            string paymentMethod = cmbPaymentMethod.SelectedItem?.ToString();
            decimal totalAmount;
            decimal paymentAmount;

            if (decimal.TryParse(txtTotalAmount.Text, out totalAmount) && 
                decimal.TryParse(txtPaymentAmount.Text, out paymentAmount))
            {
                // Insert into Payment Table
                Payment newPayment = new Payment
                {
                    Payment_ID = txtPaymentID.Text,
                    Order_ID = _orderID,
                    Total_Amount = totalAmount,
                    Payment_Amount = paymentAmount,
                    Payment_Change = paymentMethod == "Cash" ? paymentAmount - totalAmount : 0,
                    Payment_Date = DateTime.Now,
                    Payment_Method = paymentMethod
                };

                _lsDC.Payments.InsertOnSubmit(newPayment);
                _lsDC.SubmitChanges();

                MessageBox.Show("Payment Successful!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter valid payment amount.");
            }
        }

        private string GeneratePaymentID()
        {
            // Generate a unique Payment_ID
            int count = _lsDC.Payments.Count() + 1;
            return "PID" + count.ToString("D3"); // Format count to have leading zeros if necessary
        }

        private void cmbPaymentMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPaymentMethod.SelectedItem?.ToString() == "Cash")
            {
                // Show Payment Amount input for Cash payment
                txtPaymentAmount.Visibility = Visibility.Visible;
                lblPaymentAmount.Visibility = Visibility.Visible;
            }
            else
            {
                // Hide Payment Amount input for Card payment
                txtPaymentAmount.Visibility = Visibility.Collapsed;
                lblPaymentAmount.Visibility = Visibility.Collapsed;
            }
        }
    }
}
