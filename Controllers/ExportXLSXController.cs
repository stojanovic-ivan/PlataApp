
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using PlataApp.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;

public class ExportXLSXController : Controller {

    private readonly ApplicationDbContext _context;
    private readonly ExchangeRateHelper _exchangeRateHelper;
    
    public ExportXLSXController(ApplicationDbContext context, ExchangeRateHelper exchangeRateHelper) {
        _context = context;
        _exchangeRateHelper = exchangeRateHelper;
    }

    [HttpGet]
    public async Task<IActionResult> Export() {

        // sacekaj dok se pokupe exchange rates iz APIja
        (decimal rateEUR, decimal rateUSD) = await _exchangeRateHelper.GetExchangeRatesAsync();

        // Uzmite podatke koje želite izvezete (npr. iz baze podataka)
        var radnici = await _context.Radnici.ToListAsync();

        var package = new ExcelPackage();
        var workSheet = package.Workbook.Worksheets.Add("Radnici"); // Naziv lista u Excel fajlu

        // Definisi stil headera
        var headerStyle = workSheet.Cells["A1:I1"].Style;
        headerStyle.Font.Bold = true;
        headerStyle.Fill.PatternType = ExcelFillStyle.Solid;
        headerStyle.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
        headerStyle.Font.Color.SetColor(System.Drawing.Color.White);
        headerStyle.VerticalAlignment = ExcelVerticalAlignment.Center;
        headerStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        workSheet.Cells[1, 1].Value = "Id";
        workSheet.Cells[1, 2].Value = "Ime";
        workSheet.Cells[1, 3].Value = "Prezime";
        workSheet.Cells[1, 4].Value = "Adresa";
        workSheet.Cells[1, 5].Value = "Radna pozicija";
        workSheet.Cells[1, 6].Value = "Neto plata";
        workSheet.Cells[1, 7].Value = "Bruto RSD";
        workSheet.Cells[1, 8].Value = "Bruto EUR";
        workSheet.Cells[1, 9].Value = "Bruto USD";

        // dodeli stil headera
        //workSheet.Cells["A1:C1"].StyleName = "headerStyle"; // Postavljanje definisanog stila

        // Popuni xlsx podacima iz baze
        int row = 2;
        foreach(Radnik radnik in radnici) {

            if (row % 2 != 0) {
                // Definisi stil reda
                var rowStyle = workSheet.Cells[$"A{row}:I{row}"].Style;
                rowStyle.Fill.PatternType = ExcelFillStyle.Solid;
                rowStyle.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // izracunaj bruto platu u RSD, EUR i USD
            decimal netoPlata = Math.Round(radnik.NetoPlata, 2);
            decimal brutoPlataRSD = Math.Round(netoPlata * (decimal)1.7, 2);
            decimal brutoPlataEUR = Math.Round(brutoPlataRSD * rateEUR, 2);
            decimal brutoPlataUSD = Math.Round(brutoPlataRSD * rateUSD, 2);

            workSheet.Cells[row, 1].Value = radnik.Id;
            workSheet.Cells[row, 2].Value = radnik.Ime;
            workSheet.Cells[row, 3].Value = radnik.Prezime;
            workSheet.Cells[row, 4].Value = radnik.Adresa;
            workSheet.Cells[row, 5].Value = radnik.RadnaPozicija;
            workSheet.Cells[row, 6].Value = netoPlata;
            workSheet.Cells[row, 7].Value = brutoPlataRSD;
            workSheet.Cells[row, 8].Value = brutoPlataEUR;
            workSheet.Cells[row, 9].Value = brutoPlataUSD;
            row++;
        }

        // Automatski podesi širinu kolona prema najširem sadržaju
        workSheet.Cells["A:Z"].AutoFitColumns();

        // Postavi MIME tip za XLSX
        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        // Kreiraj memory stream za fajl
        var stream = new MemoryStream(package.GetAsByteArray());

        // HTTP zaglavlje za preuzimanje
        Response.Headers.Add("content-disposition", "attachment; filename=Report.xlsx");

        // Oslobodi resurse
        package.Dispose();

        return File(stream, contentType);
    }

    

}
