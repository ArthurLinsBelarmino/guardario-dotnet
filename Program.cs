using GuardaRio.Abrigos.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Oracle");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'Oracle' não configurada. Veja a seção 'Como executar' do README.");
}

builder.Services.AddDbContext<GuardaRioContext>(options => options.UseOracle(connectionString));
builder.Services.AddControllers();

var app = builder.Build();

app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsJsonAsync(new { mensagem = "Erro interno ao processar a requisição." });
}));

app.MapControllers();

app.Run();
