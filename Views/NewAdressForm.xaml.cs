
using RechnungsApp.Models;
using System.Diagnostics; 

namespace RechnungsApp.Views
{
    public partial class NewAdressForm : ContentPage
    {
        private readonly MainPage? _mainPage;
        public NewAdressForm(MainPage mainPage)
        {
            InitializeComponent();
            _mainPage = mainPage;
            BindingContext = new NewAddressModel();
        }

        private async void OnAddCustomer(object sender, EventArgs e)
        {
            var newCustomer = (NewAddressModel)BindingContext;
            
            _mainPage?.AddNewCustomer(newCustomer.Customer);
            ResetInputs();
            await Navigation.PopModalAsync();
        }

        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            ResetInputs();
            _mainPage?.ResetForm();
            await Task.Delay(100);
            await Navigation.PopModalAsync();
        }

        private void ResetInputs()
        {
            var newCustomer = (NewAddressModel)BindingContext;
            newCustomer.Customer = new Customer();
        }

    }
}