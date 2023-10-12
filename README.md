# PlataApp - Web Aplikacija za Konverziju Neto Plate u Bruto Iznos

Gemini Software - test projekat

Ova web aplikacija omogućava unos i praćenje informacija o radnicima, kao i konverziju neto plate u bruto iznos. Takođe, omogućava izvoz podataka u različite formate i druge korisne funkcionalnosti.

<br>

## Uputstvo za Pokretanje Projekta

Sledite ova uputstva kako biste pokrenuli ovaj projekat na svom računaru.

<br>

### Koraci za Postavljanje Baze Podataka

1. Instalirajte SQL Server Management Studio (SSMS) ako ga nemate instaliranog.

2. Otvorite SSMS i kreirajte novu bazu podataka za ovu aplikaciju.

<br>

### Koraci za Pokretanje Web Aplikacije

1. Instalirajte [.NET Core SDK](https://dotnet.microsoft.com/download/dotnet) ako ga nemate instaliranog.

2. Otvorite Visual Studio Code (VS Code) i otvorite ovaj projekat.

3. U VS Code terminalu, idite do foldera projekta i pokrenite sledeće komande:

   ```bash
   dotnet restore
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   dotnet run
   ```

Aplikacija će se pokrenuti na http://localhost:5000 (ili drugoj odabranoj adresi).

<br>

## Korišćenje Aplikacije

Otvorite web browser i posetite http://localhost:5000. (ili drugu odabranu adresu).

- Klikom na dugme "Dodaj novog radnika" možete unositi podatke o novim radnicima

  ![Dodaj novog radnika](https://webdevelopersi.com/plate_ss/unos.jpg)

- Klikom na žuto dugme sa ikonom oka možete videti sve podatke o izabranom radniku, kao i preračunate bruto iznose u RSD, EUR i USD

  ![Pregled radnika](https://webdevelopersi.com/plate_ss/pregled.jpg)

- Klikom na plavo dugme sa ikonom olovke možete izmeniti sve unete podatke o izabranom radniku

  ![Izmena radnika](https://webdevelopersi.com/plate_ss/izmena.jpg)

- Klikom na crveno dugme sa ikonom kante možete obrisati sve unete podatke o izabranom radniku

- Klikom na žuto dugme sa natpisom PDF možete da izvezete sve unete podatke o radnicima, kao i preračunate bruto iznose u RSD, EUR i USD, u PDF dokument

  ![Export PDF](https://webdevelopersi.com/plate_ss/pdf.jpg)

  - Klikom na žuto dugme sa natpisom XLSX možete da izvezete sve unete podatke o radnicima, kao i preračunate bruto iznose u RSD, EUR i USD, u XLSX dokument

  ![Export XLSX](https://webdevelopersi.com/plate_ss/xlsx.jpg)

<br>

## Autor

- [Ivan Stojanović](https://www.webdevelopersi.com)
