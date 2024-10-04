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

        public NewAddressModel()
        {
            Customer = new Customer();
        }
    }
}
