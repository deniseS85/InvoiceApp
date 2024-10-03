
using RechnungsApp.Models;

namespace RechnungsApp.Views
{
    public partial class NewAdressForm : ContentPage
    {
        public NewAdressForm()
        {
            InitializeComponent();

            
            BindingContext = new NewAddressModel();
        }

        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

    }
}