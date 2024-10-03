﻿namespace RechnungsApp;
using RechnungsApp.Models;
using RechnungsApp.Views;
using System.Linq;
using System.Text.RegularExpressions;

public partial class MainPage : ContentPage
{
    [GeneratedRegex(@"^\d*$")]
    private static partial Regex MyRegex();
	private readonly CustomerService customerService;
    private string? selectedCustomerName;

	public MainPage()
	{
		InitializeComponent();
		customerService = new CustomerService();
	}

	private void OnCustomerButtonClicked(object sender, EventArgs e)
    {

        Button clickedButton = (Button)sender;
        string customerName = clickedButton.Text;

        if (selectedCustomerName == customerName)
        {
            foreach (var button in ButtonContainer.Children.OfType<Button>())
            {
                button.BackgroundColor = Color.FromArgb("#ac99ea");
            }

            selectedCustomerName = null;
            return;
        }

        foreach (var button in ButtonContainer.Children.OfType<Button>())
        {
            if (button != clickedButton)
            {
                button.BackgroundColor = Color.FromArgb("#50FFFFFF");
            }
        }

        clickedButton.BackgroundColor = Color.FromArgb("#52ceff");

        var selectedCustomer = customerService.GetCustomers()
            .FirstOrDefault(c => c.Name != null && c.Name.StartsWith(customerName));

        if (selectedCustomer != null)
        {
            if (selectedCustomerName != null && selectedCustomerName != customerName)
            {
                var previousButton = GetButtonByCustomerName(selectedCustomerName);
                if (previousButton != null)
                {
                    previousButton.BackgroundColor = Color.FromArgb("#50FFFFFF");
                }
            }
            selectedCustomerName = customerName;
        }
        else
        {
            OnAddNewAddress(sender, e);
        }
    }

    private Button? GetButtonByCustomerName(string customerName)
    {
        return customerName switch
        {
            "INMEDIA STUDIOS" => customer1Button,
            "Notario" => customer2Button,
            "Neue Adresse hinzufügen" => newCustomerButton,
            _ => null,
        };
    }

   private async void OnAddNewAddress(object sender, EventArgs e)
    {
        var addNewAdress = new NewAdressForm();
        await Navigation.PushModalAsync(addNewAdress);
    }


	private void OnCreateInvoice(object sender, EventArgs e)
    {
        var selectedCustomer = customerService.GetCustomers()
            .FirstOrDefault(c => c.Name != null && selectedCustomerName != null && c.Name.StartsWith(selectedCustomerName, StringComparison.OrdinalIgnoreCase));

        if (selectedCustomer != null)
        {

            string address = $"{selectedCustomer.Name}\n{selectedCustomer.Address}\n{selectedCustomer.PostalCode} {selectedCustomer.City}\nCIF {selectedCustomer.CIF}";
            string wholeNumberText = WholeNumberEntry.Text ?? "0";
            string decimalText = string.IsNullOrEmpty(DecimalEntry.Text) ? "00" : DecimalEntry.Text;

            if (decimal.TryParse(wholeNumberText, out decimal wholeNumber) && decimal.TryParse(decimalText, out decimal decimalPart))
            {
              
                decimal totalValue = wholeNumber + (decimalPart / 100);

                if (totalValue == 0)
                {
                    DisplayAlert("Fehler", "Gebe eine Rechnungssumme ein.", "OK");
                    return;
                }

                string total = $"{totalValue:C}";
 
                ShowInvoicePDF(address, total);
                ResetForm();
            }
        }
        else
        {
            DisplayAlert("Fehler", "Bitte wähle eine Adresse aus.", "OK");
        }
    }
    

    private void OnlyNumberValid(object sender, TextChangedEventArgs e)
    {
        if (sender is Entry entry)
        {
            if (!string.IsNullOrWhiteSpace(e.NewTextValue) && !MyRegex().IsMatch(e.NewTextValue))
            {
                entry.Text = e.OldTextValue;
            }
        }
    }

    private async void ShowInvoicePDF(string address, string total)
    {
        var invoicePDF = new InvoicePDF(address, total);
        invoicePDF.OnGeneratePdfClicked(this, EventArgs.Empty);
        await Navigation.PushModalAsync(invoicePDF);
    }

    public void ResetForm()
    {
        WholeNumberEntry.Text = string.Empty;
        DecimalEntry.Text = string.Empty;
        WholeNumberEntry.Unfocus();
        DecimalEntry.Unfocus();

        foreach (var button in ButtonContainer.Children.OfType<Button>())
        {
            button.BackgroundColor = Color.FromArgb("#ac99ea");
        }
        selectedCustomerName = null;
    }
}


