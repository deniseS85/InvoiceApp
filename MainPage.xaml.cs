namespace RechnungsApp;
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
        // Überprüfen, welcher Button geklickt wurde
        Button clickedButton = (Button)sender;
        string customerName = clickedButton.Text;

        if (selectedCustomerName == customerName)
            {
                clickedButton.BackgroundColor = Color.FromArgb("#ac99ea");
                selectedCustomerName = null;
                return;
            }

        // Suche den Kunden in der Service-Klasse
        var selectedCustomer = customerService.GetCustomers()
        .FirstOrDefault(c => c.Name != null && c.Name.StartsWith(customerName));

        if (selectedCustomer != null)
        {
            if (selectedCustomerName != null)
                {
                    var previousButton = GetButtonByCustomerName(selectedCustomerName);
                    if (previousButton != null)
                    {
                        previousButton.BackgroundColor = Color.FromArgb("#ac99ea");
                    }
                }
                selectedCustomerName = customerName;
                clickedButton.BackgroundColor = Color.FromArgb("#52ceff");
        } else
        {
            DisplayAlert("Fehler", "Kunde nicht gefunden.", "OK");
        }
    }

     private Button? GetButtonByCustomerName(string customerName)
    {
        // Suche den Button anhand des Kundennamens
        return customerName switch
        {
            "INMEDIA STUDIOS" => customer1Button,
            "Notario" => customer2Button,
            _ => null,
        };
    }

    private void OnAddNewAddress(object sender, EventArgs e)
    {
        // Logik zum Hinzufügen einer neuen Adresse hier implementieren
    }


	private void OnCreateInvoice(object sender, EventArgs e)
    {
        var selectedCustomer = customerService.GetCustomers()
            .FirstOrDefault(c => c.Name != null && selectedCustomerName != null && c.Name.StartsWith(selectedCustomerName, StringComparison.OrdinalIgnoreCase));

        if (selectedCustomer != null)
        {
            string address = $"{selectedCustomer.Name}\n{selectedCustomer.Address}\n{selectedCustomer.PostalCode} {selectedCustomer.City}\nCIF {selectedCustomer.CIF}";

            string wholeNumberText = WholeNumberEntry.Text ?? "0";
            string decimalText = DecimalEntry.Text ?? "00";

            if (decimal.TryParse(wholeNumberText, out decimal wholeNumber) && decimal.TryParse(decimalText, out decimal decimalPart))
            {
                decimal totalValue = wholeNumber + (decimalPart / 100);

                if (totalValue == 0)
                {
                    DisplayAlert("Fehler", "Gebe eine Rechnungssumme ein.", "OK");
                    return;
                }

                string total = $"{totalValue:C}";
                OnShowInvoicePopup(address, total);
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

    private async void OnShowInvoicePopup(string address, string total)
    {
        var invoicePopup = new InvoicePopup(address, total);
        await Navigation.PushModalAsync(invoicePopup);
    }

    private void ResetForm()
    {
        WholeNumberEntry.Text = string.Empty;
        DecimalEntry.Text = string.Empty;
        WholeNumberEntry.Unfocus();
        DecimalEntry.Unfocus();

        if (selectedCustomerName != null)
        {
            var previousButton = GetButtonByCustomerName(selectedCustomerName);
            if (previousButton != null)
            {
                previousButton.BackgroundColor = Color.FromArgb("#ac99ea");
            }
        }
        selectedCustomerName = null;
    }
}


