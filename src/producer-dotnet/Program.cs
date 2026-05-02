using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;
using ProducerDotnet;

var builder = WebApplication.CreateBuilder(args);

// Configuração do cliente do SQS apontando para o LocalStack (Docker)
builder.Services.AddSingleton<IAmazonSQS>(sp =>
{
    var config = new AmazonSQSConfig
    {
        ServiceURL = "http://localhost:4566", // Porta do LocalStack
        AuthenticationRegion = "us-east-1"
    };
    // Usamos credenciais fakes já que o LocalStack roda localmente
    return new AmazonSQSClient("fake-access-key", "fake-secret-key", config);
});

var app = builder.Build();

app.UseHttpsRedirection();

// Endpoint para receber transações e publicar no SQS
app.MapPost("/api/transactions", async (Transaction transaction, IAmazonSQS sqsClient) =>
{
    if (string.IsNullOrWhiteSpace(transaction.AssetCode) || transaction.Price <= 0)
    {
        return Results.BadRequest("Dados da transação inválidos.");
    }

    // Serializa o objeto Transaction para JSON
    string messageBody = JsonSerializer.Serialize(transaction);

    // Envia a mensagem para a fila SQS do LocalStack
    var sendMessageRequest = new SendMessageRequest
    {
        QueueUrl = "http://localhost:4566/000000000000/financial-operations-queue",
        MessageBody = messageBody
    };

    try
    {
        await sqsClient.SendMessageAsync(sendMessageRequest);
        Console.WriteLine($"[API .NET] Mensagem enviada com sucesso para o SQS: {transaction.AssetCode}");
        return Results.Ok(new { message = "Transação enviada para processamento!", id = transaction.Id });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Erro] Falha ao enviar para o SQS: {ex.Message}");
        return Results.Problem("Erro interno ao enviar para a fila de mensageria.");
    }
});

app.Run();