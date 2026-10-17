using CartorioTwin.Api.Seguranca;
using CartorioTwin.Infrastructure.Persistencia;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("CartorioTwin")
    ?? throw new InvalidOperationException("A conexão CartorioTwin é obrigatória.");

builder.Services.AddPersistenciaCartorioTwin(connectionString);
builder.Services.AddSegurancaCartorioTwin(builder.Configuration, builder.Environment);

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();

public partial class Program;
