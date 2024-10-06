
using RechnungsApp.Models;
using RechnungsApp.Views;
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
            CustomPopup.PopupClosed += OnPopupClosed;
        }

        private async void OnAddCustomer(object sender, EventArgs e)
        {
            var newCustomer = (NewAddressModel)BindingContext;

            if (newCustomer.Customer == null || string.IsNullOrWhiteSpace(newCustomer.Customer.Name))
            {
                await Overlay.FadeTo(0.6, 100);
                Overlay.IsVisible = true;
                await CustomPopup.ShowPopup("Fehler", "Name ist erforderlich");
                return;
            }
            
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

        private async void OnPopupClosed(object? sender, EventArgs e) {
            await Overlay.FadeTo(0, 100);
            Overlay.IsVisible = false;
        }

    }
}