using System.Globalization;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iTextCell = iText.Layout.Element.Cell;
using iText.Layout.Borders;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Properties;

namespace RechnungsApp.Views
{
    public partial class InvoicePopup : ContentPage
    {
        public InvoicePopup(string address, string total)
        {
            InitializeComponent();
            PreviewAddress.Text = address;

            var formattedDateString = new FormattedString();
            formattedDateString.Spans.Add(new Span 
            {
                Text = DateTime.Now.ToString("dd/MM/yyyy"),
                FontAttributes = FontAttributes.Bold,
                FontSize = 15,
            });
          /*   PreviewDate.FormattedText = formattedDateString; */

            /* var formattedString = new FormattedString();
            formattedString.Spans.Add(new Span 
            {
                Text = total,
                FontAttributes = FontAttributes.Bold,
                FontSize = 15
            });

            PreviewTotal.FormattedText = formattedString; */
            PreviewTotal.Text = total;
            CalculateValues(total);
        }

        private void CalculateValues(string total)
        {
            string cleanedTotal = total.Replace(" €", "").Replace(",", ".");
                                                   
            if (decimal.TryParse(cleanedTotal, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal totalValue))  
            // CultureInfo.InvariantCulture: Komma wird als Dezimaltrennzeichen richtig interpretiert
            // NumberStyles.Any: akzeptiert verschiedene Zahlenformate
            {
                decimal baseImponible = totalValue / 1.10m; // m: Kennzeichen für Dezimal
                decimal iva = baseImponible * 0.10m;

                BaseImponibleLabel.Text = $"{baseImponible:F2} €"; // F2: 2 Dezimalstellen
                IvaLabel.Text = $"{iva:F2} €";
            } else
            {
                BaseImponibleLabel.Text = string.Empty;
                IvaLabel.Text = string.Empty;
            }
        }

        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void OnPrintClicked(object sender, EventArgs e)
        {
            // Hier die Logik zum Drucken der Rechnung einfügen
            await DisplayAlert("Info", "Rechnung wird gedruckt!", "OK");
        }

        private async void OnGeneratePdfClicked(object sender, EventArgs e)
        {           
            using MemoryStream memoryStream = new();
            try
            {
                using PdfWriter writer = new(memoryStream);
                using (var pdf = new PdfDocument(writer))
                {
                    Document document = new(pdf);

                    // Bild hinzufügen
                    var imgStream = await ConvertImageSourceToStreamAsync("logo.png");
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(ImageDataFactory
                        .Create(imgStream))
                        .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetWidth(120)
                        .SetMargins(0, 0, 30, 0);
                    document.Add(image);
                        
                    // Absenderadresse
                    Paragraph senderInfo = new Paragraph("Calle Mariano Riquer Wallis 9\nLocal 6\n07840 Santa Eulalia del Rio\nCIF E16618001")
                        .SetFontSize(12)
                        .SetMargins(17, 0, 42, 0);
                    document.Add(senderInfo);

                    // Betreff und Datum
                    float[] columnWidths = [1, 1]; 
                    Table addressDateRow = new Table(columnWidths)
                        .SetWidth(UnitValue.CreatePercentValue(100));

                    var subjectLine = new iTextCell()
                        .Add(new Paragraph("Dirección de facturación:")
                        .SetBold()
                        .SetPaddingLeft(-3)
                        .SetPaddingBottom(-3)
                        .SetFontSize(14)
                        .SetFontColor(new DeviceRgb(17, 119, 176)));

                    subjectLine.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    addressDateRow.AddCell(subjectLine);

                    string formattedDate = DateTime.Now.ToString("dd/MM/yyyy");
                    var dateCell = new iTextCell()
                        .Add(new Paragraph(formattedDate)
                        .SetBold()
                        
                        .SetPaddingBottom(-3)
                        .SetFontSize(14)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
                        .SetFontColor(new DeviceRgb(17, 119, 176)));

                    dateCell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    addressDateRow.AddCell(dateCell);

                    document.Add(addressDateRow);

                    // Trennlinie
                    SolidLine line = new (2);
                        line.SetColor(new DeviceRgb(17, 119, 176));
                    LineSeparator ls = new (line);
                        ls.SetWidth(155)
                        .SetMarginTop(2);
                    document.Add(ls);

                    // Empfänger Adresse
                    document.Add(new Paragraph(PreviewAddress.Text).SetFontSize(13));

                    // Rechnungsnummer
                    document.Add(new Paragraph("Número de factura: 23/2024")
                        .SetBold().SetFontSize(14)
                        .SetFontColor(new DeviceRgb(17, 119, 176))
                        .SetMarginTop(15)
                        .SetMarginBottom(13));

                    // Rechnungs Tabelle
                    float[] pointColumnWidths = [1, 1];
                    Table table = new Table(pointColumnWidths)
                        .SetWidth(UnitValue.CreatePercentValue(100));

                    // Header
                    var descriptionCell = new iTextCell().Add(new Paragraph("Descripción").SetBold().SetFontSize(14)).SetPaddingLeft(-1).SetBorderBottom(new SolidBorder(new DeviceRgb(17, 119, 176), 2));
                    descriptionCell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    table.AddCell(descriptionCell);

                    var totalCell = new iTextCell().Add(new Paragraph("Total").SetBold().SetFontSize(14)).SetBorderBottom(new SolidBorder(new DeviceRgb(17, 119, 176), 2)).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                    totalCell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    table.AddCell(totalCell);

                    // 1. Zeile
                    var baseImponibleCell = new iTextCell().Add(new Paragraph("Base Imponible").SetFontSize(12).SetPaddingLeft(-2).SetPaddingTop(5).SetPaddingBottom(5));
                    baseImponibleCell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    table.AddCell(baseImponibleCell);

                    var baseImponibleText = new iTextCell().Add(new Paragraph(BaseImponibleLabel.Text).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT).SetPaddingTop(5).SetPaddingBottom(5));
                    baseImponibleText.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    table.AddCell(baseImponibleText);

                    // 2. Zeile
                    var ivaCell = new iTextCell().Add(new Paragraph("IVA 10 %").SetFontSize(12).SetPaddingLeft(-2).SetPaddingBottom(5)).SetBorderBottom(new SolidBorder(new DeviceRgb(17, 119, 176), 2));
                    ivaCell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    table.AddCell(ivaCell);

                    var ivaText = new iTextCell().Add(new Paragraph(IvaLabel.Text).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT).SetPaddingBottom(5)).SetBorderBottom(new SolidBorder(new DeviceRgb(17, 119, 176), 2));
                    ivaText.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    table.AddCell(ivaText);

                    // 3. Zeile
                    var totalBillCell = new iTextCell().Add(new Paragraph("Factura Total").SetBold().SetFontSize(14).SetPaddingLeft(-2).SetPaddingTop(3).SetPaddingBottom(3)).SetBorderBottom(new DoubleBorder(new DeviceRgb(17, 119, 176), 4));
                    totalBillCell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    table.AddCell(totalBillCell);

                    var totalBillText = new iTextCell().Add(new Paragraph(PreviewTotal.Text).SetBold().SetFontSize(14).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT).SetPaddingTop(3).SetPaddingBottom(3)).SetBorderBottom(new DoubleBorder(new DeviceRgb(17, 119, 176), 4));
                    totalBillText.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    table.AddCell(totalBillText);

                    table.SetMarginTop(10);
                    document.Add(table);

                    // Schlusssatz
                    document.Add(new Paragraph("La factura esta pagada.").SetBold().SetFontSize(13).SetMarginTop(40));
                    document.Add(new Paragraph("Muchas gracias por su visita!").SetFontSize(13));

                    // Stempel und Unterschrift
                    var stamp_sign = await ConvertImageSourceToStreamAsync("stamp_sign.png");
                    document.Add(new iText.Layout.Element.Image(ImageDataFactory.Create(stamp_sign)).SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT).SetWidth(180).SetMarginTop(8));
                }
                
                // PDF-Daten im MemoryStream abrufen
                byte[] pdfData = memoryStream.ToArray();

                DisplayPdfPreview(pdfData);
            }
            catch (Exception ex)
            {
               await DisplayAlert("Fehler", "Fehler beim Erstellen des PDFs: " + ex.Message, "OK");
            }
        }

        private async Task<byte[]> ConvertImageSourceToStreamAsync(string imageName)
        {
            using var ms = new MemoryStream();
            using (var stream = await FileSystem.OpenAppPackageFileAsync(imageName))
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }

        private void DisplayPdfPreview(byte[] pdfData)
        {
            string fileName = "generated.pdf";
            string localFilePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            // PDF-Daten in eine lokale Datei schreiben
            File.WriteAllBytes(localFilePath, pdfData);

            // PDF im WebView anzeigen
            PdfWebView.Source = new UrlWebViewSource { Url = localFilePath };
        }
    }

}
