using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PlataApp.Models;
using PlataApp.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;

namespace PlataApp.Controllers;

public class RadniciController : Controller
{
    private readonly ILogger<RadniciController> _logger;
    private readonly ApplicationDbContext _context;

    private readonly IHttpClientFactory _clientFactory;

    public RadniciController(ILogger<RadniciController> logger, ApplicationDbContext context, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _context = context;
        _clientFactory = clientFactory;
    }

    // Akcija za spisak radnika
    public async Task<IActionResult> Index()
    {
        // pokupi listu radnika iz baze
        var radnici = await _context.Radnici.ToListAsync();

        // ako nije prazna prosledi je u view
        if (radnici != null) {
            return View(radnici);
        } else {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetRadnik(int id)
    {
        // nadji radnika po id
        var radnik = _context.Radnici.FirstOrDefault(r => r.Id == id);
        
        if (radnik == null) {
            return NotFound(); 
        }

        // izracunaj bruto platu u RSD
        radnik.BrutoPlataRSD = Math.Round(radnik.NetoPlata * (decimal)1.7, 2);

        // Poziv API-ja za konverziju
        var apiUri = "https://api.fastforex.io/fetch-multi?from=RSD&to=EUR,USD&api_key=demo";
        var httpClient = _clientFactory.CreateClient();

        try {
            var response = await httpClient.GetAsync(new Uri(apiUri));
            if (response.IsSuccessStatusCode) {
                var content = await response.Content.ReadAsStringAsync();

                // Obrada JSON odgovora
                var konverzija = JsonConvert.DeserializeObject<Konverzija>(content);

                // ako postoje podaci od api-ja, preracunaj bruto u EUR i USD
                if (konverzija != null && konverzija.Results.ContainsKey("EUR") && konverzija.Results.ContainsKey("USD")) {
                    // Postavite vrednosti konverzije u Radnik objekat
                    radnik.BrutoPlataEUR = Math.Round(radnik.BrutoPlataRSD * konverzija.Results["EUR"], 2);
                    radnik.BrutoPlataUSD = Math.Round(radnik.BrutoPlataRSD * konverzija.Results["USD"], 2);
                } else {
                    radnik.BrutoPlataEUR = 0;
                    radnik.BrutoPlataUSD = 0;
                }
            }
        } catch {
            radnik.BrutoPlataEUR = 0;
            radnik.BrutoPlataUSD = 0;
        }

        // Vrati podatke o radniku u JSON formatu
        return Json(radnik); 
    }

    
    public IActionResult Create() {
         // Inicijalizuj novi objekat Radnik i prosledi ga u View
        Radnik noviRadnik = new Radnik();
        return View(noviRadnik); 
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Ime,Prezime,Adresa,RadnaPozicija,NetoPlata")] Radnik radnik)
    {
        // Ako je moguće, sačuvaj podatke iz forme u bazu
        if (ModelState.IsValid) {
            _context.Add(radnik);
            await _context.SaveChangesAsync();
            // vrati se na index stranu
            return RedirectToAction(nameof(Index));
        }

        // Ukoliko došlo do neke greške, vrati se na create stranu
        return View(radnik);
    }

    public IActionResult Edit(int? id) {
        if (id == null) {
            return NotFound();
        }

        // nađi radnika po id
        var radnik = _context.Radnici.Find(id);

        if (radnik == null) {
            return NotFound();
        }

        // prosledi podatke u edit view
        return View(radnik);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("Id,Ime,Prezime,Adresa,RadnaPozicija,NetoPlata")] Radnik radnik) {
        if (id != radnik.Id) {
            return NotFound();
        }

        // ako je moguće, ažuriraj podatke u bazi
        if (ModelState.IsValid) {
            try {
                _context.Update(radnik);
                _context.SaveChanges();
            } catch (DbUpdateConcurrencyException) {
                var postojiRadnik = _context.Radnici.Find(id);
                if (postojiRadnik == null) {
                    return NotFound();
                } else {
                    throw;
                }
            }
            // vrati se na index stranu
            return RedirectToAction(nameof(Index));
        }

        // u suprotnom, vrati se na edit stranu
        return View(radnik);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id) {
        // nađi radnika po id
        var radnik = await _context.Radnici.FindAsync(id);

        if (radnik == null) {
            return NotFound();
        }

        try {
            // obriši radnika iz baze
            _context.Radnici.Remove(radnik);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index"); 
        } catch {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
        
        return RedirectToAction("Index"); 
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
