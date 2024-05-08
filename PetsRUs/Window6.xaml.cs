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
    public partial class Window6 : Window
    {
        //private Dictionary<string, dynamic> _cartItems = new Dictionary<string, dynamic>();
        private petsrusDataContext _lsDC;
        private string _staffID;
        private List<dynamic> _supplies; // Add _supplies as a class member



        public Window6(Dictionary<string, dynamic> cartItems, petsrusDataContext lsDC, string staffID, List<dynamic> supplies)
        {
            InitializeComponent();
            _lsDC = lsDC; // Initialize _lsDC
            _staffID = staffID;
            _supplies = supplies; // Initialize _supplies
            LoadCartItems();
            CalculateTotalAmount();
        }

        public static Dictionary<string, dynamic> CartItems { get; } = new Dictionary<string, dynamic>();

        private void LoadCartItems()
        {
            cartListView.ItemsSource = CartItems.Values.ToList();
        }

        private void CalculateTotalAmount()
        {
            decimal totalAmount = 0;
            foreach (var item in CartItems.Values)
            {
                totalAmount += item.Price * item.Quantity;
            }
            txtTotalAmount.Text = totalAmount.ToString();
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            string customerName = txtCustomerName.Text;
            string paymentMethod = cmbPaymentMethod.SelectedItem?.ToString();
            decimal totalAmount;
            decimal paymentAmount;
            // Declare a counter outside the loop
            int orderCounter = _lsDC.Orders.Count() + 1;


            if (decimal.TryParse(txtTotalAmount.Text, out totalAmount) &&
                decimal.TryParse(txtPaymentAmount.Text, out paymentAmount)) // Parse payment amount
            {
                // Validate input
                if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(paymentMethod))
                {
                    MessageBox.Show("Please fill all fields correctly.");
                    return;
                }

                // Calculate remaining balance
                decimal remainingBalance = paymentAmount - totalAmount;

                // Create customer and order records
                string orderID = GenerateOrderID();
                string customerID = GenerateCustomerID();

                if (!string.IsNullOrEmpty(orderID) && !string.IsNullOrEmpty(customerID))
                {
                    try
                    {
                        // Insert into Customers table
                        Customer newCustomer = new Customer
                        {
                            Customer_ID = customerID,
                            Customer_Name = customerName,
                            // Add other customer details as needed
                        };
                        _lsDC.Customers.InsertOnSubmit(newCustomer);

                        // Insert orders for each item in the cart

                        foreach (var item in Window6.CartItems.Values)
                        {
                            // Generate a unique order ID based on the counter
                            orderID = "OID" + orderCounter.ToString("D3");
                            orderCounter++; // Increment the counter for the next order

                            Order newOrder = new Order
                            {
                                Order_ID = orderID,
                                Customer_ID = customerID,
                                Staff_ID = _staffID,
                                Order_Desc = "Pet Supply", // Provide a description for the order
                                Order_Date = DateTime.Now,
                                Order_Status = "Complete",
                                Supplies_ID = item.Supplies_ID // Assign Supplies_ID
                            };
                            _lsDC.Orders.InsertOnSubmit(newOrder);

                            string stockID = item.Stock_ID.ToString();

                            var stockItem = _lsDC.StockSupplies.FirstOrDefault(stock => stock.Stock_ID == stockID);
                            if (stockItem != null)
                            {
                                // Deduct the ordered quantity
                                stockItem.Stock_Quantity -= item.Quantity;

                                // Submit changes to the database
                                _lsDC.SubmitChanges();
                                Console.WriteLine("Stock quantity deducted successfully.");
                            }
                            else
                            {
                                // Log an error if stock item not found
                                Console.WriteLine("Error: Stock item not found.");
                            }
                        }

                        // Insert payment record
                        Payment newPayment = new Payment
                        {
                            Payment_ID = GeneratePaymentID(),
                            Customer_ID = customerID, // Assign the customer ID
                            Total_Amount = totalAmount,
                            Payment_Amount = paymentAmount,
                            Payment_Change = remainingBalance,
                            Payment_Method = paymentMethod,
                            Payment_Date = DateTime.Now
                        };

                        _lsDC.Payments.InsertOnSubmit(newPayment);

                        // Submit changes to the database
                        _lsDC.SubmitChanges();

                        MessageBox.Show("Checkout successful!");
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to generate Customer_ID or Order_ID.");
                }
            }
            else
            {
                MessageBox.Show("Please enter valid total amount and payment amount.");
            }
            Window6.CartItems.Clear();
        }


        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedCartItem = cartListView.SelectedItem;
            if (selectedCartItem != null)
            {
                string itemId = $"{selectedCartItem.Supplies_Name}_{selectedCartItem.Supply_Category}_{selectedCartItem.Price}";
                if (CartItems.ContainsKey(itemId))
                {
                    // Create a new instance of the anonymous type with the updated quantity
                    var updatedItem = new
                    {
                        Supplies_Name = selectedCartItem.Supplies_Name,
                        Supply_Category = selectedCartItem.Supply_Category,
                        Price = selectedCartItem.Price,
                        Quantity = selectedCartItem.Quantity + 1,
                        Supplies_ID = selectedCartItem.Supplies_ID
                    };

                    // Update the item in the cart
                    CartItems[itemId] = updatedItem;
                    LoadCartItems();
                    CalculateTotalAmount();
                }
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedCartItem = cartListView.SelectedItem;
            if (selectedCartItem != null)
            {
                string itemId = $"{selectedCartItem.Supplies_Name}_{selectedCartItem.Supply_Category}_{selectedCartItem.Price}";
                if (CartItems.ContainsKey(itemId) && CartItems[itemId].Quantity > 1)
                {
                    // Create a new instance of the anonymous type with the updated quantity
                    var updatedItem = new
                    {
                        Supplies_Name = selectedCartItem.Supplies_Name,
                        Supply_Category = selectedCartItem.Supply_Category,
                        Price = selectedCartItem.Price,
                        Quantity = selectedCartItem.Quantity - 1,
                        Supplies_ID = selectedCartItem.Supplies_ID
                    };

                    // Update the item in the cart
                    CartItems[itemId] = updatedItem;
                    LoadCartItems();
                    CalculateTotalAmount();
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Window5 window5 = new Window5(_supplies, _staffID);
            window5.Show();
            this.Close();
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

        private string GeneratePaymentID()
        {
            int count = _lsDC.Payments.Count() + 1;
            return "PID" + count.ToString("D3"); // Format count to have leading zeros if necessary
        }
    }
}
