# RaceDay Part 3 MVC additions

This update retains the supplied Part 2 API endpoints, request models, database schema, and existing Images controller. No category ID was added to enrolments.

## Added MVC pages
- Organiser dashboard with Create Event, Edit Event, Delete Event, Categories, Enrolments and Results.
- Participant event enrolment via the existing POST /api/Enrolments endpoint (eventID only).
- Participant My Results via GET /api/results/me.
- User profile display and update via existing Users API.
- Role-specific navigation.

## Important limitations inherited from Part 2
- The supplied API has no GET endpoint for the logged-in participant's enrolments. Therefore this package does not add a fake My Enrolments screen or modify Part 2 to introduce one.
- The supplied enrolment endpoint accepts eventID only, not a category selection; this remains unchanged as requested.
- Image functionality is retained as supplied, but image-upload UI has not been added here.
- Event lists are returned by the existing API; the organiser dashboard may display events created by other organisers. The API itself enforces ownership for modifications.

## Run
1. Open RaceDay.sln in Visual Studio 2022.
2. Set RaceDay.Api and RaceDay.Mvc to Start under multiple startup projects.
3. Confirm MVC appsettings.json ApiBaseUrl matches the API HTTPS URL (currently https://localhost:7187/).
4. Run the database setup required by your original API.
5. Test registering/logging in as organiser and participant, then the added views.

This package was reviewed statically; a .NET SDK and SQL Server were not available in the packaging environment, so it has not been compiled or integration-tested here.
