# WalletTransfer

Event-driven wallet transfer pipeline using Clean Architecture, MediatR, Minimal API, EF Core, Outbox, and background workers.

## Demo behavior
- wallet id contains `retry-block` => first block retryable fail, second success
- wallet id contains `invalid` => wallet validation permanent fail
- wallet id contains `low-balance` => balance check permanent fail
- wallet id contains `transfer-timeout` => first transfer retryable fail, second success
- wallet id contains `transfer-permanent-fail` => transfer permanent fail + compensation path

## Run
`dotnet run --project src/WalletTransfer.Api`

Swagger available in development.
