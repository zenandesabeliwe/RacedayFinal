RaceDay Part 3 - updates

Changes:
- Shared Organiser permissions for event editing, deletion, category management, enrolment viewing and result management.
- Event deletion still blocks events with categories/enrolments or other related SQL records. Remove related categories only when safe; never delete participant enrolments/results automatically.
- API error messages now appear in MVC instead of a generic error.
- Create Event offers an optional first category, saved through the existing Categories API after event creation.
- Event details display categories.
- Welcome page includes Get Started (Register) and Log In.
- Responsive dark-charcoal navigation with Bootstrap hamburger toggle.
- No new database entities, tables or enrolment category IDs.

Verification:
- Source reviewed and packaged. The .NET SDK and SQL Server are not available in this environment; build and integration tests have NOT been run.
- In Visual Studio, restore NuGet packages, build the solution and start both RaceDay.Api and RaceDay.Mvc.
- Verify MVC appsettings.json ApiBaseUrl matches the actual API HTTPS port.
- Test create event with category, edit, categories, enrolments, results, delete (with/without related records), and both user roles.
- Azure Blob Storage and Docker were not configured or executed.
