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
        private List<dynamic> cartItems = new List<dynamic>();
        private Dictionary<string, dynamic> _cartItems = new Dictionary<string, dynamic>();
        public Window6(Dictionary<string, dynamic> items)
        {
            InitializeComponent();
            _cartItems = items;
            LoadCartItems();
        }

        private void LoadCartItems()
        {
            cartListView.ItemsSource = _cartItems.Values.ToList();
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement checkout logic here
            MessageBox.Show("Checkout successful!");
            // Clear the cart after checkout
            cartItems.Clear();
            LoadCartItems(); // Refresh the cart view after checkout
        }
    }
}