using HttpPrint.Client.Models;
using System.Drawing.Printing;

namespace HttpPrint.Client.RequestHandlers
{
    public static class PrinterHelper
    {
        public static async Task<List<string>> GetAllInstalledPrinters()
        {
            var printers = PrinterSettings.InstalledPrinters.Cast<string>().ToList();
            return await Task.FromResult(printers);
        }

        public static async Task<PrintResponse> PrintPdf(string printerName, int copies, Stream stream)
        {
            try
            {
                var printerSettings = new PrinterSettings
                {
                    PrinterName = printerName,
                    Copies = (short)copies,
                };
                using var document = PdfiumViewer.PdfDocument.Load(stream);
                using var printDocument = document.CreatePrintDocument();

                printDocument.PrinterSettings = printerSettings;
                printDocument.DefaultPageSettings = new PageSettings(printerSettings)
                {
                    Margins = new Margins(0, 0, 0, 0),
                };
                printDocument.PrintController = new StandardPrintController();
                printDocument.Print();
            }
            catch (Exception ex)
            {
                return PrintResponse.Failed("Failed :-" + ex.Message);
            }
            return await Task.FromResult(PrintResponse.Success(""));
        }
    }
}
