Migration: dotnet ef migrations add InitialCreate --project NexusProcure.Infrastructure --startup-project NexusProcure.Api
Remove Migration: dotnet ef migrations remove --project NexusProcure.Infrastructure --startup-project NexusProcure.Api

Database Update: dotnet ef database update --project NexusProcure.Infrastructure --startup-project NexusProcure.Api