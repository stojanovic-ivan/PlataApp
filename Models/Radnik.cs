using System.ComponentModel.DataAnnotations.Schema;

public class Radnik {

    public Radnik() {
        Ime = "";
        Prezime = "";
        Adresa = "";
        NetoPlata = 0;
        RadnaPozicija = "";
    }

    public int Id { get; set; }
    
    public string Ime { get; set; }

    public string Prezime { get; set; }

    public string Adresa { get; set; }

    public decimal NetoPlata { get; set; }
    
    public string RadnaPozicija { get; set; }

    [NotMapped]
    public decimal BrutoPlataRSD { get; set; }
    [NotMapped]
    public decimal BrutoPlataEUR { get; set; }
    [NotMapped]
    public decimal BrutoPlataUSD { get; set; }

}
