# RaceDay — PROG6212 Part 2 (ASP.NET Core Web API)

RaceDay is a web-based event management system for South African running, walking, and cycling events. Part 2 implements the REST API planned in Part 1 using ASP.NET Core Web API, C#, Entity Framework Core, and SQL Server.

## Roles

- **Organiser:** manages their own events and categories, views registrations for their events, and records results.
- **Participant:** registers and logs in, browses events, enrols in an event/category, views their own registrations and results, and updates their profile.

## Technology

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8 with SQL Server provider
- Server-side session authentication using an HTTP-only session cookie
- PBKDF2-SHA256 password hashing with a per-password random salt
- Swagger / OpenAPI
- xUnit integration and security tests
- GitHub Actions build-and-test workflow

## Repository layout

```text
RaceDay-Part2/
├── Controllers/                 # API endpoints
├── Data/RaceDayDbContext.cs     # EF Core mappings and relationships
├── DTOs/                        # Request models and validation
├── Models/Entities.cs            # Entities mapped to the Part 1 SQL tables
├── Security/                    # Password hashing and session authentication
├── Tests/                       # xUnit tests
├── docs/
│   ├── API-Endpoint-Plan.md
│   ├── database.sql
│   └── ERD.png
├── .github/workflows/
│   ├── part1-validation.yml
│   └── part2-ci.yml
├── RaceDay.Api.csproj
├── RaceDay.sln
├── Program.cs
└── appsettings.json
```

## Prerequisites

1. Visual Studio 2022 with the **ASP.NET and web development** workload.
2. .NET 8 SDK.
3. SQL Server LocalDB, SQL Server Express, or SQL Server Developer Edition, and SQL Server Management Studio (SSMS).

## 1. Create the database

1. Open SSMS and connect to your SQL Server instance.
2. Open `docs/database.sql` and execute it to create `RaceDayDB` and its tables.
3. The provided Part 1 SQL script contains `DROP TABLE IF EXISTS` statements. Re-running it recreates the tables and deletes existing table data, so back up any data first.
4. For the default LocalDB connection, the database server should be `(localdb)\MSSQLLocalDB`.

The API entity mappings use the seven tables from Part 1: `EVENT_ORGANISER`, `PARTICIPANT`, `EVENT`, `CATEGORY`, `WEATHER`, `REGISTRATION`, and `RESULT`. The API is intentionally mapped to that schema rather than creating a different database design.

## 2. Configure the connection string

The default connection is in `appsettings.json`:

```json
"ConnectionStrings": {
  "RaceDayDb": "Server=(localdb)\\MSSQLLocalDB;Database=RaceDayDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

Change the `Server` value if your SQL Server instance is different. Do not commit production credentials or secrets to GitHub.

## 3. Open and run in Visual Studio

1. Open `RaceDay.sln` in Visual Studio.
2. Set `RaceDay.Api` as the startup project.
3. Restore NuGet packages when prompted.
4. Confirm that `RaceDayDB` exists and the connection string matches your SQL Server instance.
5. Press **F5** or **Ctrl+F5**.
6. Open Swagger at the URL shown by Visual Studio, normally `https://localhost:7184/swagger`.

The application maps to the existing database; it does not automatically run the destructive Part 1 SQL script.

## 4. Try authentication in Swagger

1. `POST /api/auth/register` with a request such as:

```json
{
  "firstName": "Alex",
  "lastName": "Runner",
  "email": "alex.runner@example.com",
  "password": "SecurePass123!",
  "role": "Participant",
  "dateOfBirth": "2001-05-12",
  "phone": "0821234567"
}
```

For an Organiser, use `"role": "Organiser"`; `dateOfBirth` is only required for Participants.

2. Call `POST /api/auth/login` using the same email and password. The server stores the user ID, role, email and display name in the session. Keep the session cookie for subsequent requests.
3. Use `POST /api/auth/logout` to clear the session.
4. Swagger's browser UI should retain cookies for same-origin requests. If testing through another client, enable cookie storage and send the returned session cookie on later requests.

**Important:** The sample users in the original Part 1 SQL script have plain-text example passwords. The Part 2 login code deliberately verifies PBKDF2 hashes for accounts created through the API. Register a new account through `/api/auth/register` to test login. Do not use the sample plaintext passwords as production credentials.

## 5. Endpoint groups

- `/api/auth` — register, login, logout
- `/api/profile` — current user's profile and password change
- `/api/organisers` — organiser lookup, creation, own-profile update/delete
- `/api/participants` — participant lookup, creation, own-profile update/delete
- `/api/events` — browse events; Organiser-only create/update/delete; event enrolment listing
- `/api/categories` — browse categories; Organiser-only category management for their events
- `/api/registrations` — Participant enrolment and role-checked registration management
- `/api/enrolments` — enrolment route aliases from the Part 1 endpoint plan
- `/api/results` — results lookup, personal results and Organiser result management
- `/api/weather` — stored weather lookup, forecast lookup and Organiser weather-alert recording

Protected endpoints return `401 Unauthorized` when there is no authenticated session and `403 Forbidden` when the signed-in role or account does not have permission. The API checks that an Organiser owns the event before allowing event-specific changes.

## 6. Run tests

From a terminal opened in this folder:

```bash
dotnet restore RaceDay.sln
dotnet test RaceDay.sln
```

The test project covers password hashing, registration/login and session use, rejecting Participant access to event creation, Organiser event/category creation, successful enrolment, and duplicate enrolment rejection. GitHub Actions runs restore, build and tests on pushes and pull requests to `main` and `master`.

## Notes about preserving the Part 1 plan

- The API uses the existing seven-table database model. No new database table has been introduced.
- The Part 1 endpoint plan has both `/api/registrations` and `/api/enrolments` terminology; the API supports the listed enrolment aliases where practical.
- The Part 1 SQL script does not define a `UNIQUE` constraint on `RESULT.RegistrationId`, although the ERD relationship suggests zero-or-one result per registration. The API checks for an existing result before insertion, but a database constraint would be stronger protection against concurrent duplicate inserts.
- The `WEATHER` table does not have dedicated alert fields or a separate alert entity. To avoid changing the approved schema, the alert endpoint records the alert type/message in `WEATHER.WeatherCondition`. This is a schema-preserving compromise, not a full alert-history design.
- The plan describes current/forecast weather, but the supplied project contains no external weather-provider credentials or integration specification. These endpoints return stored weather records; a live third-party weather feed remains additional work if required by the lecturer.
- Part 1's `database.sql` sample users store demonstration passwords as plain text. New Part 2 accounts are hashed. If you decide to migrate the sample users for login tests, update their password values to hashes through a controlled migration rather than weakening password verification.
- Review each implementation decision, run the API and tests locally, and be ready to explain the code in your own video presentation. Disclose AI assistance as required by the assignment instructions.

## Part 2 video and CI screenshot

After you run the project and GitHub Actions successfully, replace this section with your **unlisted YouTube demonstration link** and a screenshot of the actual green GitHub Actions run. Do not use a placeholder screenshot or claim a build passed before you verify it.
