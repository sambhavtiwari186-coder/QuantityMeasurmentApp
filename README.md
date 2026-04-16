# Quantity Measurement API

This project is an ASP.NET Core Web API that handles quantity measurement conversions, comparisons, and arithmetic operations. It has been recently upgraded to include a robust Security layer with JWT Authentication, Password Hashing, Encryption, and REST API Best Practices.

## Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server LocalDB (or any SQL Server instance configured in `QuantityMeasurementWebAPI\appsettings.json`)

## Running the Application

### 1. Build the Solution
Open a terminal in the solution root directory and restore/build the projects:
```powershell
dotnet clean
dotnet build
```

### 2. Apply Database Migrations
Before running the application for the first time, you must apply the EF Core migrations to create the database schema (including the `UserEntity` tables for authentication).
Run the following command from the root directory:
```powershell
dotnet ef database update --project QuantityMeasurementApp.Repository --startup-project QuantityMeasurementWebAPI
```

### 3. Start the Web API
To run the project, start the `QuantityMeasurementWebAPI` startup project:
```powershell
dotnet run --project QuantityMeasurementWebAPI
```

Alternatively, if you are using Visual Studio, you can:
1. Set `QuantityMeasurementWebAPI` as the Startup Project.
2. Press `F5` or click "Start Debugging".

### 4. Test with Swagger UI
Once the application starts, it will automatically launch the Swagger UI in your default web browser (e.g., `https://localhost:7084/swagger`).

**How to test the Authentication workflow:**
1. Expand the `POST /api/Auth/register` endpoint in Swagger.
2. Click **Try it out**, fill in a new Username, Email, and Password, and click **Execute**.
3. Expand the `POST /api/Auth/login` endpoint.
4. Click **Try it out**, provide the Username and Password you just created, and click **Execute**.
5. Copy the `token` string from the JSON response.
6. Scroll back to the top of the Swagger page and click the green **Authorize** button.
7. Paste your token preceded by `Bearer ` (e.g., `Bearer eyJhbG...`) into the Value field and click Authorize.

All future requests made through Swagger will now be authenticated.

### 5. Running Automated Tests
To execute the test suite:
```powershell
dotnet test
```
