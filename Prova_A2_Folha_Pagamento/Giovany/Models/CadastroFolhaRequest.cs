namespace Giovany.Models;

public class CadastroFolhaRequest
{
    public string? Nome { get; set; }
    public string? Cpf { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
    public decimal HorasTrabalhadas { get; set; }
    public decimal ValorHora { get; set; }
}
