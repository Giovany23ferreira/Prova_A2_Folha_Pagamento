namespace Giovany.Models;

public class FolhaPagamento
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public int Mes { get; set; }
    public int Ano { get; set; }
    public decimal HorasTrabalhadas { get; set; }
    public decimal ValorHora { get; set; }
    public decimal SalarioBruto { get; set; }
    public decimal ImpostoRenda { get; set; }
    public decimal Inss { get; set; }
    public decimal Fgts { get; set; }
    public decimal SalarioLiquido { get; set; }
}
