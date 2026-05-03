using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.EntityFrameworkCore;
using ProducerDotNet;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 1. Configura o Entity Framework Core com o PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Configura o cliente do Amazon SQS apontando para o LocalStack
var sqsConfig = new AmazonSQSConfig
{
    ServiceURL = "http://localhost:4566",
    AuthenticationRegion = "us-east-1"
};

builder.Services.AddSingleton<IAmazonSQS>(new AmazonSQSClient("fake-access-key", "fake-secret-key", sqsConfig));


var app = builder.Build();

// Endpoint para receber transações via HTTP (POST)
app.MapPost("/api/transactions", async (Transaction transaction, AppDbContext db, IAmazonSQS sqsClient) =>
{
    // Preenche os metadados da transação
    transaction.Id = Guid.NewGuid();
    transaction.CreatedAt = DateTime.UtcNow;

    // Ação 1: Salva no PostgreSQL
    db.Transactions.Add(transaction);
    await db.SaveChangesAsync();

    // Ação 2: Envia para o SQS local
    var queueUrl = "http://sqs.us-east-1.localhost.localstack.cloud:4566/000000000000/financial-operations-queue";
    var messageBody = JsonSerializer.Serialize(transaction);

    var sendMessageRequest = new SendMessageRequest
    {
        QueueUrl = queueUrl,
        MessageBody = messageBody
    };

    await sqsClient.SendMessageAsync(sendMessageRequest);

    // Retorna a resposta confirmando o sucesso de ambas as operações
    return Results.Ok(new 
    { 
        message = "Transação salva no banco e enviada para processamento!", 
        id = transaction.Id 
    });
});


// NOVO ENDPOINT: Listar transações salvas no PostgreSQL
app.MapGet("/api/transactions", async (AppDbContext db) =>
{
    // Busca todas as transações no banco de dados ordenando pelas mais recentes
    var transactions = await db.Transactions
        .OrderByDescending(t => t.CreatedAt)
        .ToListAsync();

    return Results.Ok(transactions);
});


// Cria automaticamente a tabela no PostgreSQL caso ela não exista
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();