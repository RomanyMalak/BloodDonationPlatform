# Database

This project contains entity classes and AppDbContext only.
No business logic, repositories, services, or controllers have been implemented yet.

Run migrations later using:

dotnet ef migrations add InitialCreate --project BloodDonation.Infrastructure --startup-project BloodDonation.API

dotnet ef database update --project BloodDonation.Infrastructure --startup-project BloodDonation.API
