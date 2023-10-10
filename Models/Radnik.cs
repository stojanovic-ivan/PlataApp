using System;
using System.ComponentModel.DataAnnotations;

public class Radnik
{
    public int Id { get; set; }
    
    [Required]
    public string Ime { get; set; }

    [Required]
    public string Prezime { get; set; }

    [Required]
    public string Adresa { get; set; }

    [Required]
    public decimal NetoPlata { get; set; }
    
    [Required]
    public string RadnaPozicija { get; set; }
}
