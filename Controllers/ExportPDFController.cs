using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlataApp.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;

namespace PlataApp.Controllers;

public class ExportPDFController : Controller {

    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _clientFactory;

    static int _brojKolona = 9;
    Document _doc;
    Font _fontStyle;
    PdfPTable _pdfTable = new PdfPTable(_brojKolona);
    PdfPCell _pdfPCell;
    MemoryStream _memoryStream = new MemoryStream();

    decimal _rateEUR = 0;
    decimal _rateUSD = 0;
    
    public ExportPDFController(ApplicationDbContext context, IHttpClientFactory clientFactory)
    {
        _context = context;
        _clientFactory = clientFactory;
    }

    public async Task<ActionResult> Export() {

        var radnici = await _context.Radnici.ToListAsync();
        byte[] abytes = PreparePDF(radnici);
        return File(abytes, "application/PDF");
    }

    public byte[] PreparePDF(List<Radnik> radnici) {

        _doc = new Document(PageSize.A4.Rotate(), 0f, 0f, 0f, 0f);
        _doc.SetPageSize(PageSize.A4.Rotate());
        _doc.SetMargins(20f, 20f, 20f, 20f);
        _pdfTable.WidthPercentage = 100;
        _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
        _fontStyle = FontFactory.GetFont("Tahoma", 8f,  Font.BOLD);
        PdfWriter.GetInstance(_doc, _memoryStream);
        _doc.Open();
        _pdfTable.SetWidths(new float[] {15f, 50f, 50f, 80f, 80f, 25f, 25f, 25f, 25f});

        this.ReportHeader();
        this.ReportBody(radnici);
        _pdfTable.HeaderRows = 2;
        _doc.Add(_pdfTable);
        _doc.Close();

        return _memoryStream.ToArray();
    }

    private void ReportHeader() {
        AddCell("Gemini Software", BaseColor.WHITE, 11f, Element.ALIGN_CENTER, true, 9, 0);
        _pdfTable.CompleteRow();
        AddCell("spisak radnika", BaseColor.WHITE, 11f, Element.ALIGN_CENTER, true, 9, 0);
        _pdfTable.CompleteRow();
        AddCell(" ", BaseColor.WHITE, 11f, Element.ALIGN_CENTER, true, 9, 0);
        _pdfTable.CompleteRow();
    }

    private void ReportBody(List<Radnik> radnici) {

        // sacekaj dok se pokupe exchange rates iz APIja
        var getRates = GetExchangeRatesAsync();
        getRates.Wait();

        AddCell("ID", BaseColor.DARK_GRAY, 8f, Element.ALIGN_CENTER, true, 1, 0, true);
        AddCell("Ime", BaseColor.DARK_GRAY, 8f, Element.ALIGN_CENTER, true, 1, 0, true);
        AddCell("Prezime", BaseColor.DARK_GRAY, 8f, Element.ALIGN_CENTER, true, 1, 0, true);
        AddCell("Adresa", BaseColor.DARK_GRAY, 8f, Element.ALIGN_CENTER, true, 1, 0, true);
        AddCell("Radna pozicija", BaseColor.DARK_GRAY, 8f, Element.ALIGN_CENTER, true, 1, 0, true);
        AddCell("Neto plata", BaseColor.DARK_GRAY, 8f, Element.ALIGN_CENTER, true, 1, 0, true);
        AddCell("Bruto RSD", BaseColor.DARK_GRAY, 8f, Element.ALIGN_CENTER, true, 1, 0, true);
        AddCell("Bruto EUR", BaseColor.DARK_GRAY, 8f, Element.ALIGN_CENTER, true, 1, 0, true);
        AddCell("Bruto USD", BaseColor.DARK_GRAY, 8f, Element.ALIGN_CENTER, true, 1, 0, true);
        _pdfTable.CompleteRow();

        _fontStyle = FontFactory.GetFont("Tahoma", 8f, Font.NORMAL);
        int row = 1;
        foreach(Radnik radnik in radnici) {

            BaseColor cellColor = row % 2 == 0 ? BaseColor.LIGHT_GRAY : BaseColor.WHITE;

            AddCell(radnik.Id.ToString(),cellColor, 8f, Element.ALIGN_CENTER);
            AddCell(radnik.Ime, cellColor);
            AddCell(radnik.Prezime, cellColor);
            AddCell(radnik.Adresa, cellColor);
            AddCell(radnik.RadnaPozicija, cellColor);

            // izracunaj bruto platu u RSD, EUR i USD
            decimal netoPlata = Math.Round(radnik.NetoPlata, 2);
            decimal brutoPlataRSD = Math.Round(netoPlata * (decimal)1.7, 2);
            decimal brutoPlataEUR = Math.Round(brutoPlataRSD * _rateEUR, 2);
            decimal brutoPlataUSD = Math.Round(brutoPlataRSD * _rateUSD, 2);

            AddCell(netoPlata.ToString(), cellColor, 8f, Element.ALIGN_RIGHT);
            AddCell(brutoPlataRSD.ToString(), cellColor, 8f, Element.ALIGN_RIGHT);
            AddCell(brutoPlataEUR.ToString(), cellColor, 8f, Element.ALIGN_RIGHT);
            AddCell(brutoPlataUSD.ToString(), cellColor, 8f, Element.ALIGN_RIGHT);

            _pdfTable.CompleteRow();
            row++;
        }

    }

    private void AddCell(string text, BaseColor backgroundColor = null, float fontsize = 8f, int hAlignment = Element.ALIGN_LEFT, bool bold = false, int colspan = 1, int border = 1, bool whiteText = false) {
        _fontStyle = FontFactory.GetFont("Tahoma", fontsize, bold ? Font.BOLD : Font.NORMAL, BaseColor.BLACK);
        if (whiteText) _fontStyle.SetColor(255,255,255);
        _pdfPCell = new PdfPCell(new Phrase(text, _fontStyle));
        _pdfPCell.Colspan = colspan;
        _pdfPCell.HorizontalAlignment = hAlignment;
        _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
        _pdfPCell.Border = border;
        if (backgroundColor != null) {
            _pdfPCell.BackgroundColor = backgroundColor;
        }
        _pdfPCell.ExtraParagraphSpace = 0;
        _pdfTable.AddCell(_pdfPCell);
    }

    private async Task GetExchangeRatesAsync() {
        // Pokrenite asinhroni poziv API-ja i sačekajte odgovor
        var apiUri = "https://api.fastforex.io/fetch-multi?from=RSD&to=EUR,USD&api_key=demo";
        var httpClient = _clientFactory.CreateClient();

        HttpResponseMessage apiResponse = await httpClient.GetAsync(new Uri(apiUri));

        if (apiResponse.IsSuccessStatusCode) {
            var content = await apiResponse.Content.ReadAsStringAsync();
            var konverzija = JsonConvert.DeserializeObject<Konverzija>(content);

            if (konverzija != null && konverzija.Results.ContainsKey("EUR") && konverzija.Results.ContainsKey("USD")) {
                // Ako je API odgovor uspešan, možete postaviti vrednosti konverzije u polja klase
                _rateEUR = konverzija.Results["EUR"];
                _rateUSD = konverzija.Results["USD"];
            }
        }
    }

}