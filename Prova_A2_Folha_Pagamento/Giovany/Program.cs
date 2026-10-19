using Giovany.Data;
using Giovany.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FolhaDbContext>(options =>
    options.UseSqlite("Data Source=giovany.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FolhaDbContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => Results.Ok(new { mensagem = "API de Folha de Pagamento funcionando" }));

app.MapPost("/api/folha/cadastrar", async (CadastroFolhaRequest entrada, FolhaDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(entrada.Nome) || string.IsNullOrWhiteSpace(entrada.Cpf))
        return Results.BadRequest(new { erro = "Nome e CPF são obrigatórios." });
    if (entrada.Mes < 1 || entrada.Mes > 12)
        return Results.BadRequest(new { erro = "O mês deve estar entre 1 e 12." });
    if (entrada.Ano < 2000)
        return Results.BadRequest(new { erro = "O ano deve ser maior ou igual a 2000." });
    if (entrada.HorasTrabalhadas < 0 || entrada.ValorHora < 0)
        return Results.BadRequest(new { erro = "Horas trabalhadas e valor da hora não podem ser negativos." });

    bool jaExiste = await db.Folhas.AnyAsync(f => f.Cpf == entrada.Cpf && f.Mes == entrada.Mes && f.Ano == entrada.Ano);
    if (jaExiste)
        return Results.Conflict(new { erro = "Já existe uma folha para este CPF, mês e ano." });

    decimal bruto = Arredondar(entrada.HorasTrabalhadas * entrada.ValorHora);
    decimal ir = CalcularIr(bruto);
    decimal inss = CalcularInss(bruto);
    decimal fgts = Arredondar(bruto * 0.08m);
    decimal liquido = Arredondar(bruto - ir - inss);

    var folha = new FolhaPagamento
    {
        Nome = entrada.Nome,
        Cpf = entrada.Cpf,
        Mes = entrada.Mes,
        Ano = entrada.Ano,
        HorasTrabalhadas = entrada.HorasTrabalhadas,
        ValorHora = entrada.ValorHora,
        SalarioBruto = bruto,
        ImpostoRenda = ir,
        Inss = inss,
        Fgts = fgts,
        SalarioLiquido = liquido
    };
    db.Folhas.Add(folha);
    try
    {
        await db.SaveChangesAsync();
    }
    catch (DbUpdateException)
    {
        return Results.Conflict(new { erro = "Não foi possível cadastrar. Verifique se a folha já existe." });
    }
    return Results.Created($"/api/folha/buscar/{Uri.EscapeDataString(folha.Cpf)}/{folha.Mes}/{folha.Ano}", folha);
});

app.MapGet("/api/folha/listar", async (FolhaDbContext db) =>
{
    var folhas = await db.Folhas.AsNoTracking().OrderBy(f => f.Id).ToListAsync();
    return folhas.Count == 0 ? Results.NotFound(new { erro = "Nenhuma folha cadastrada." }) : Results.Ok(folhas);
});

app.MapGet("/api/folha/buscar/{cpf}/{mes:int}/{ano:int}", async (string cpf, int mes, int ano, FolhaDbContext db) =>
{
    var folha = await db.Folhas.AsNoTracking().FirstOrDefaultAsync(f => f.Cpf == cpf && f.Mes == mes && f.Ano == ano);
    return folha is null ? Results.NotFound(new { erro = "Folha não encontrada." }) : Results.Ok(folha);
});

app.MapDelete("/api/folha/remover/{cpf}/{mes:int}/{ano:int}", async (string cpf, int mes, int ano, FolhaDbContext db) =>
{
    var folha = await db.Folhas.FirstOrDefaultAsync(f => f.Cpf == cpf && f.Mes == mes && f.Ano == ano);
    if (folha is null) return Results.NotFound(new { erro = "Folha não encontrada." });
    db.Folhas.Remove(folha);
    await db.SaveChangesAsync();
    return Results.Ok(new { mensagem = "Folha removida com sucesso." });
});

app.MapGet("/api/folha/total-liquido", async (FolhaDbContext db) =>
{
    var salarios = await db.Folhas.AsNoTracking().Select(f => f.SalarioLiquido).ToListAsync();
    if (salarios.Count == 0) return Results.NotFound(new { erro = "Nenhuma folha cadastrada." });
    return Results.Ok(new { totalLiquido = Arredondar(salarios.Sum()) });
});

app.Run();

static decimal Arredondar(decimal valor) => Math.Round(valor, 2, MidpointRounding.AwayFromZero);

static decimal CalcularIr(decimal bruto)
{
    if (bruto <= 1903.98m) return 0m;
    if (bruto <= 2826.65m) return Arredondar(Math.Max(0m, bruto * 0.075m - 142.80m));
    if (bruto <= 3751.05m) return Arredondar(Math.Max(0m, bruto * 0.15m - 354.80m));
    if (bruto <= 4664.68m) return Arredondar(Math.Max(0m, bruto * 0.225m - 636.13m));
    return Arredondar(Math.Max(0m, bruto * 0.275m - 869.36m));
}

static decimal CalcularInss(decimal bruto)
{
    if (bruto <= 1693.72m) return Arredondar(bruto * 0.08m);
    if (bruto <= 2822.90m) return Arredondar(bruto * 0.09m);
    if (bruto <= 5645.80m) return Arredondar(bruto * 0.11m);
    return 621.03m;
}
