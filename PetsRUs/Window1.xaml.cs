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
    public partial class Window1 : Window
    {
        private string username;
        private string _staffID;
        private petsrusDataContext _lsDC;

        public Window1(string username, string staffID, petsrusDataContext lsDC)
        {
            InitializeComponent();
            txtStaffName.Text = $"Staff: {username}";
            this.username = username;
            _staffID = staffID; // Assign _staffID
            _lsDC = lsDC;
        }


        private void DogButton_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow2WithPetType("Dog");
        }

        private void CatButton_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow2WithPetType("Cat");
        }

        private void FoodButton_Click(object sender, RoutedEventArgs e)
        {
            LoadSupplies("Food");
        }

        private void HealthButton_Click(object sender, RoutedEventArgs e)
        {
            LoadSupplies("Health");
        }

        private void AccessoriesButton_Click(object sender, RoutedEventArgs e)
        {
            LoadSupplies("Accessories");
        }

        private void LoadSupplies(string category)
        {
            var supplies = (from s in _lsDC.PetSupplies
                            where s.Supply_Category == category
                            select new
                            {
                                Supplies_ID = s.Supplies_ID,
                                Supplies_Name = s.Supplies_Name,
                                Supply_Category = s.Supply_Category,
                                Price = s.Price,
                                Stock_ID = s.Stock_ID
                            }).ToList();

            Window5 window5 = new Window5(supplies.Cast<dynamic>().ToList());
            window5.Show();
        }


        private void OpenWindow2WithPetType(string petType)
        {
            Window2 window2 = new Window2(petType, username, _staffID); // Pass _staffID here
            window2.Show();
        }   

        private void OpenWindow5WithSupplies(dynamic supplies)
        {
            Window5 window5 = new Window5(supplies.Cast<dynamic>().ToList());
            window5.Show();
        }
    }
}