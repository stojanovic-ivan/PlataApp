# PlataApp - Web Aplikacija za Konverziju Neto Plate u Bruto Iznos

Gemini Software - test projekat

Ova web aplikacija omogućava unos i praćenje informacija o radnicima, kao i konverziju neto plate u bruto iznos. Takođe, omogućava izvoz podataka u različite formate i druge korisne funkcionalnosti.

## Uputstvo za Pokretanje Projekta

Sledite ova uputstva kako biste pokrenuli ovaj projekat na svom računaru.

### Koraci za Postavljanje Baze Podataka

1. Instalirajte SQL Server Management Studio (SSMS) ako ga nemate instaliranog.

2. Otvorite SSMS i kreirajte novu bazu podataka za ovu aplikaciju.

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

## Korišćenje Aplikacije

1. Otvorite web browser i posetite http://localhost:5000.

## Autor

- [Ivan Stojanović](https://github.com/stojanovic-ivan)
