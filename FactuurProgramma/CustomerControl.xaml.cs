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
using Factory;
using LogicLayer;
using Models;

namespace FactuurProgramma
{
    /// <summary>
    /// Interaction logic for CustomerControl.xaml
    /// </summary>
    public partial class CustomerControl : UserControl
    {
        private Customer customer;
        ICustomerLogic customerLogic = CustomerFactory.ManageCustomers();

        public List<Customer> Customers { get; private set; }

        public CustomerControl()
        {
            InitializeComponent();
        }

        public static readonly RoutedEvent SettingConfirmedEvent =
            EventManager.RegisterRoutedEvent("SettingConfirmedEvent", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(CustomerControl));

        public event RoutedEventHandler SettingConfirmed
        {
            add { AddHandler(SettingConfirmedEvent, value); }
            remove { RemoveHandler(SettingConfirmedEvent, value); }
        }



        public void Change(Customer customer)
        {
            tbFirstname.Text = customer.FirstName;
            tbPreposition.Text = customer.Preposition;
            tbLastname.Text = customer.LastName;
            tbEmail.Text = customer.Email;
            tbPhoneNumber.Text = customer.PhoneNumber;
            tbPlace.Text = customer.Address.Place;
            tbStreet.Text = customer.Address.StreetName;
            tbZipCode.Text = customer.Address.ZipCode;
        }

        private void tbLastname_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Customers = customerLogic.GetCustomersByLastName(tbLastname.Text);

            RaiseEvent(new RoutedEventArgs(CustomerControl.SettingConfirmedEvent));
        }
    }
}
