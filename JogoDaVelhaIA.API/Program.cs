using JogoDaVelhaIA.Data;
using JogoDaVelhaIA.Interfaces;
using JogoDaVelhaIA.Models;
using JogoDaVelhaIA.Repositories;
using JogoDaVelhaIA.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inje��o de depend�ncias
builder.Services.AddScoped<IJogadaRepository, JogadaRepository>();
builder.Services.AddScoped<IJogadaService, JogadaService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ?? CORS liberado para tudo (ajuste se necess�rio)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

// Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Aplicar migra��es automaticamente (apenas para desenvolvimento)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Inicializar dados base se necess�rio
    await InicializarDadosBase(db);
}

app.Run();

async Task InicializarDadosBase(AppDbContext db)
{
    if (!db.JogadasBase.Any())
    {
        // Estrat�gias b�sicas para o jogo da velha
        var jogadasBase = new List<JogadaBase>
        {
            // Jogadas ofensivas (alto peso)
            new() { EstadoTabuleiro = "X,X, , , , , , , ", PosicaoEscolhida = 2, Peso = 100 },
            new() { EstadoTabuleiro = " , , ,X,X, , , , ", PosicaoEscolhida = 5, Peso = 100 },
            new() { EstadoTabuleiro = " , , , , , ,X,X, ", PosicaoEscolhida = 8, Peso = 100 },
            
            // Jogadas defensivas (peso m�dio)
            new() { EstadoTabuleiro = "O,O, , , , , , , ", PosicaoEscolhida = 2, Peso = 80 },
            new() { EstadoTabuleiro = " , ,O, , ,O, , , ", PosicaoEscolhida = 7, Peso = 80 }
        };

        await db.JogadasBase.AddRangeAsync(jogadasBase);
        await db.SaveChangesAsync();
    }
}