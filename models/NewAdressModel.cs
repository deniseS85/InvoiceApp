using System.Windows.Input;
using RechnungsApp.Models;
using Microsoft.Maui.Controls;

namespace RechnungsApp.Models
{
    public class NewAddressModel : BindableObject
    {
        private Customer? customer;

        public Customer? Customer
        {
            get => customer;
            set
            {
                customer = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCustomerCommand { get; }

        public NewAddressModel()
        {
            Customer = new Customer();
            AddCustomerCommand = new Command(OnAddCustomer);
        }

        private /* async */ void OnAddCustomer()
        {
           // Beispiel für die Anzeige der Kundendaten in einem DisplayAlert
            /* await Application.Current.MainPage.DisplayAlert("Kundendaten",
                $"Name: {Customer.Name}\nAdresse: {Customer.Address}\nPostalCode: {Customer.PostalCode}\nCity: {Customer.City}\nCIF: {Customer.CIF}",
                "OK"); */
        }
    }
}
