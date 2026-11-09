# RaceDay Part 3 — implementation status (read first)

This archive contains the original Part 2 API and tests, an **initial MVC application**, Azure Blob upload endpoints, Dockerfile and GitHub Actions build workflow. It is **NOT a completed Part 3 submission**. The MVC pages for organiser CRUD, category management, enrolment, result capture, participant profile and filtering still need implementation and testing. The API upload endpoints have not been integration-tested. Do not submit this as a finished project.

## Start in Visual Studio 2022
1. Open `RaceDay.sln`. Set multiple startup projects: `RaceDay.Api` and `RaceDay.Mvc`.
2. Run `RaceDay.Api` and copy its actual HTTPS URL into `RaceDay.Mvc/appsettings.json` as `ApiBaseUrl` (include trailing slash).
3. Ensure SQL Server LocalDB instance `(localdb)\api_both` and database `Raceday` are available.
4. Run `RaceDay.Api/Database/Part3Migration.sql` against the database once.
5. Configure Azure Storage using .NET user secrets or environment variable `AzureStorage__ConnectionString` **on the API only**. Do not commit secrets. For local testing, a real storage account or configured Azurite endpoint is required. The upload code currently creates public-read blob containers; assess privacy requirements before production use.
6. Run MVC and log in through the API. The API session cookie is forwarded by the MVC HTTP service; review this before deployment.
7. To build MVC Docker image, from the folder containing `RaceDay.sln`: `docker build -f RaceDay.Mvc/Dockerfile -t raceday-mvc .`
8. Run container (adjust API URL for your Docker networking): `docker run --rm -p 8080:8080 -e ApiBaseUrl=http://host.docker.internal:5000/ raceday-mvc` . The API must be reachable at that URL; LocalDB on Windows does not run inside this MVC container.

## Known gaps / checks
- No .NET SDK available in the preparation environment; build and tests were **not run**. Fix any compilation or integration issues in Visual Studio before submission.
- Image URLs are saved in new nullable columns; the existing event/profile JSON endpoints need updating to expose these columns for display.
- Check actual API login response property names and API cookie forwarding. Login must return `userRole`.
- Add all remaining required MVC pages, form validation, ownership checks, role-aware access, organiser event summaries, participant history and status labels.
- Review SQL schema column names (especially `Organisers` table) for image upload ownership query.
- Do not claim 20 Git commits until you actually create 20 meaningful commits in your repository.
