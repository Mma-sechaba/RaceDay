# Part 2 implementation notes

## Decisions kept aligned with Part 1

- Database name remains `RaceDayDB`.
- Entity mappings use the seven submitted tables and the existing key/foreign-key relationships.
- Session state stores `UserId`, `Role`, `Email`, and `FullName`; role authorization is enforced by ASP.NET Core authorization attributes and ownership checks.
- Passwords created by the API are PBKDF2-SHA256 hashes with random salts.
- Endpoint aliases support the plan's registration/enrolment naming where the submitted plan lists both.

## Existing Part 1 issues that should be discussed in the video

1. The `RESULT` table does not declare `RegistrationId` UNIQUE even though the ERD relationship describes zero-or-one result per registration. The API checks for duplicates, but adding a database unique constraint would be safer; it has not been added here to avoid silently changing the submitted schema.
2. The SQL script's seeded accounts contain plain-text example passwords. Part 2 does not accept these as valid hashes. Use the registration endpoint to create test accounts.
3. `WEATHER` has no separate alert fields and there is no weather-provider configuration. The alert route stores an alert string in `WeatherCondition`, and forecast/current routes read the saved WEATHER records. Live third-party weather retrieval is not implemented.
4. The endpoint plan contains both `/api/registrations` and `/api/enrolments` route naming. The API supports the specified enrolment POST/DELETE aliases and the organiser event-enrolment route.

## Local verification still required

The generation environment used to prepare these files did not have the .NET SDK installed, so `dotnet build` and `dotnet test` could not be executed here. Run the README commands in Visual Studio or a .NET 8 terminal, resolve any environment-specific errors, and confirm the GitHub Actions workflow is green before submission.
