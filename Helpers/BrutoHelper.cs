public class BrutoHelper
{
    public decimal GetBruto(decimal netoPlata) {
        decimal neoporezivDeo = 3719.38m;
        decimal oporezivDeo = netoPlata - neoporezivDeo;

        // odredi porez na dohodak gradjana
        decimal StopaPDG = 0.1m;
        if ( oporezivDeo > 7438.75m) StopaPDG = 0.15m;

        // doprinos za PIO
        decimal StopaPIO = 0.26m;

        // doprinos za zdravstveno
        decimal StopaZO = 0.0515m;

        // izracunaj bruto
        decimal brutoPlata = Math.Round(netoPlata / (1 - StopaPDG - StopaPIO - StopaZO), 2);

        return brutoPlata;
    }
}