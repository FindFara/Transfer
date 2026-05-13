using MediatR;
using WalletTransfer.Application;
using WalletTransfer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("Default") ?? "Data Source=wallet.db");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<WalletTransferDbContext>().Database.EnsureCreated();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapGet("/health", () => Results.Ok("OK"));
app.MapPost("/transfers", async (CreateTransferRequest req, IMediator mediator, CancellationToken ct) =>
{
    var id = await mediator.Send(new CreateTransferCommand(req.SourceWalletId, req.DestinationWalletId, req.SourceProvider, req.DestinationProvider, req.Amount, req.Currency, req.ClientIdempotencyKey), ct);
    return Results.Ok(new { transferId = id });
});
app.MapGet("/transfers/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
{
    var t = await mediator.Send(new GetTransferByIdQuery(id), ct);
    return t is null ? Results.NotFound() : Results.Ok(t);
});
app.Run();

public sealed record CreateTransferRequest(string SourceWalletId,string DestinationWalletId,string SourceProvider,string DestinationProvider,decimal Amount,string Currency,string ClientIdempotencyKey);
