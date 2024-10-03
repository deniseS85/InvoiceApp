namespace RechnungsApp.Models
{
    public class CustomerService
    {
        private readonly List<Customer> customers =
        [
            new()
            {
                Name = "INMEDIA STUDIOS IBIZA, S.L.",
                Address = "Calle San Lorenzo 11",
                PostalCode = "07840",
                City = "Sta. Eulalia del Rio",
                CIF = "B16611782"
            },
            new()
            {
                Name = "Notario Fernando Ramos Gil",
                Address = "Carrer Mariano Riquer Wallis 9",
                PostalCode = "07840",
                City = "Sta. Eulalia del Rio",
                CIF = "44368656T"
            }
        ];

        // Methode zum Abrufen der Kundenliste
        public List<Customer> GetCustomers()
        {
            return customers;
        }
    }
}
