using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PlataApp.Models;
using PlataApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace PlataApp.Controllers;

public class RadniciController : Controller
{   private readonly ApplicationDbContext _context;
    private readonly ExchangeRateHelper _exchangeRateHelper;
    private readonly BrutoHelper _brutoHelper;
    public RadniciController(ApplicationDbContext context, ExchangeRateHelper exchangeRateHelper, BrutoHelper brutoHelper) {
        _context = context;
        _exchangeRateHelper = exchangeRateHelper;
        _brutoHelper = brutoHelper;
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
        radnik.BrutoPlataRSD = _brutoHelper.GetBruto(radnik.NetoPlata);

        // sacekaj dok se pokupe exchange rates iz APIja
        (decimal rateEUR, decimal rateUSD) = await _exchangeRateHelper.GetExchangeRatesAsync();

        // Postavite vrednosti konverzije u Radnik objekat
        radnik.BrutoPlataEUR = Math.Round(radnik.BrutoPlataRSD * rateEUR, 2);
        radnik.BrutoPlataUSD = Math.Round(radnik.BrutoPlataRSD * rateUSD, 2);

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
