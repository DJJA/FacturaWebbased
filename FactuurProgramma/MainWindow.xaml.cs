using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
using Factory;
using LogicLayer;
using Models;

namespace FactuurProgramma
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ICustomerLogic customerLogic = CustomerFactory.ManageCustomers();
        public MainWindow()
        {
            InitializeComponent();
            dgOverview.ItemsSource = customerLogic.GetAllCustomers();

            AddHandler(CustomerControl.SettingConfirmedEvent,
                new RoutedEventHandler(CustomerControl_OnSettingConfirmed));
        }

        private void dgOverview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           var selectedCustomer = (Customer)dgOverview.SelectedItem;
            if (selectedCustomer != null)
            {
                customerControl.Change(selectedCustomer);
            }
        }

        private void CustomerControl_OnSettingConfirmed(object sender, RoutedEventArgs e)
        {
            dgOverview.ItemsSource = null;
            dgOverview.ItemsSource = customerControl.Customers;
        }
    }
}
