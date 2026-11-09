
# RaceDay Event Management System

## ASSIGNMENT 3: MVC DEVELOPMENT

## 1. PROJECT OVERVIEW

RaceDay is a web-based race event management system developed as part of a Software Development assignment.

The system allows Organisers to manage sporting events and Participants to browse upcoming races, enrol in events and view their race results.

The application consists of an ASP.NET Core Web API for backend operations, an ASP.NET Core MVC website for the user interface and a Microsoft SQL Server database for storing information.

RaceDay supports two user roles: Organiser and Participant. Each role has its own navigation menu and access to relevant system features.

## 2. TECHNOLOGIES USED

The following technologies were used to develop the RaceDay system:

- **C#:** Used to develop the application's functionality.
- **ASP.NET Core Web API:** Used to develop RESTful API endpoints.
- **ASP.NET Core MVC:** Used to develop the web application and user interface.
- **Razor Views:** Used to display dynamic information on web pages.
- **Visual Studio 2022:** Used to develop, build, debug and test the application.
- **Microsoft SQL Server:** Used to store and manage application data.
- **SQL Server Management Studio (SSMS):** Used to create and manage database tables.
- **ADO.NET:** Used to connect the API to SQL Server and execute SQL queries.
- **BCrypt:** Used for password hashing and verification.
- **Swagger:** Used to test the API endpoints.
- **xUnit:** Used for automated testing.
- **GitHub:** Used for source code version control and project submission.
- **GitHub Actions:** Used for CI workflow configuration, subject to successful workflow verification.

## 3. DATABASE IMPLEMENTATION

The application uses a SQL Server database named **Raceday**.

The database consists of seven main tables:

- **Users:** Stores registered user information, including names, email addresses, phone numbers, password hashes and user roles.
- **Organisers:** Stores organiser information linked to registered users.
- **Participants:** Stores participant information linked to registered users.
- **Events:** Stores sporting event details, including event names, dates, locations and other event information.
- **Categories:** Stores race categories associated with events.
- **Enrolments:** Records participants who have enrolled in sporting events.
- **Results:** Stores race results, including finishing times and finishing positions.

Primary keys and foreign keys are used to establish relationships between the database tables.

The API communicates with SQL Server using ADO.NET and parameterised SQL queries.

### Database Demonstration Data

The database can contain sample organisers, participants, sporting events, categories, enrolments and results for demonstration purposes.

A database setup script containing fictional demonstration records should be provided separately so that the application can be tested on another computer.

Real user information and passwords should not be included in a public repository.

## 4. RESTFUL API IMPLEMENTATION

The RaceDay Web API manages the backend functionality and communicates with the SQL Server database.

### 4.1 AuthController

The AuthController manages user registration, login and logout.

Users can register as either Organisers or Participants.

Passwords are hashed using BCrypt before being stored in the database. During login, the entered password is verified against the stored password hash.

### 4.2 UsersController

The UsersController manages registered user information.

Authenticated users can retrieve and update their profile information.

### 4.3 EventsController

The EventsController manages sporting events.

It provides functionality for retrieving events and for Organisers to create, update and delete events.

Role-based authorisation is used to restrict event management operations.

### 4.4 CategoriesController

The CategoriesController manages race categories associated with sporting events.

It provides functionality for creating, retrieving, updating and deleting categories.

### 4.5 EnrolmentsController

The EnrolmentsController manages participant enrolments.

Participants can enrol in events and retrieve their existing enrolments.

The API checks for duplicate enrolments to prevent participants from entering the same event more than once.

Organisers can retrieve enrolment information for events.

### 4.6 ResultsController

The ResultsController manages race results.

It provides functionality for recording finishing times and positions and retrieving results.

### 4.7 ParticipantsController

The ParticipantsController manages participant information.

It provides functionality for retrieving participant records according to the permissions implemented in the API.

## 5. MVC IMPLEMENTATION

The ASP.NET Core MVC application provides the user interface for RaceDay.

The MVC website communicates with the Web API to retrieve, display and manage information.

The application follows the Model-View-Controller design pattern.

- **Models:** Represent application data.
- **Views:** Display information to users using Razor.
- **Controllers:** Handle user requests and communicate with the API.

### 5.1 Home Page

The home page introduces the RaceDay system.

It provides navigation options for users to register or log in.

### 5.2 Registration and Login

Users can create accounts and log in using their registered email addresses and passwords.

After a successful login, users are redirected according to their roles.

- Organisers are redirected to the Organiser Dashboard.
- Participants are redirected to the Upcoming Races page.

### 5.3 Organiser Interface

The Organiser interface provides functionality for managing sporting events.

The interface includes:

- Organiser Dashboard
- Create Event
- Upcoming Races
- Event Management
- Enrolment Information
- Race Results
- My Profile
- Logout

### 5.4 Participant Interface

The Participant interface allows registered participants to access race-related information.

The interface includes:

- Personalised welcome message
- Upcoming Races
- Event Details
- Event Enrolment
- My Enrolments
- My Results
- My Profile
- Logout

### 5.5 Personalised Welcome Message

After a successful login, the application retrieves the user's first name from the session.

Participants are welcomed on the Upcoming Races page.

The first name is stored in the MVC session after authentication.

### 5.6 Event Enrolment

Participants can select an upcoming event and view its details.

The application communicates with the API to process enrolment requests.

Participants can view their registered events under My Enrolments.

The event details page also checks existing enrolment information to identify participants who have already enrolled.

## 6. AUTHENTICATION AND SECURITY

RaceDay implements authentication and role-based access control.

The system supports two user roles:

- Organiser
- Participant

BCrypt is used to hash and verify passwords.

Session information is used to identify authenticated users and determine their roles.

The MVC application uses session information to display navigation options appropriate to the logged-in user.

Protected pages redirect unauthenticated users to the login page.

The API uses authorisation checks to restrict access to protected operations.

Parameterised SQL queries are used to reduce the risk of SQL injection.

Logout functionality clears the MVC session and sends a logout request to the API.

## 7. ORGANISER FUNCTIONALITY

The Organiser role is designed for users responsible for managing sporting events.

The system provides functionality for:

- Viewing the Organiser Dashboard.
- Creating sporting events.
- Viewing upcoming races.
- Managing existing events.
- Working with race categories.
- Retrieving participant enrolments.
- Accessing race results functionality.
- Viewing and updating profile information.

The complete Organiser workflow, particularly category management and results entry through MVC, should be verified before final submission.

## 8. PARTICIPANT FUNCTIONALITY

The Participant role is designed for users who want to participate in sporting events.

The system provides functionality for:

- Registering a Participant account.
- Logging in securely.
- Viewing a personalised welcome message.
- Browsing upcoming races.
- Viewing event information.
- Enrolling in sporting events.
- Viewing existing enrolments.
- Preventing duplicate event enrolments.
- Accessing personal race results.
- Viewing and updating profile information.
- Logging out.

The My Enrolments feature retrieves participant enrolment information from the API and displays it on the MVC website.

## 9. API TESTING USING SWAGGER

Swagger was used during development to test the RaceDay Web API.

The tested functionality included:

- User registration and login.
- Organiser and Participant authentication.
- Creating and retrieving events.
- Updating and deleting events.
- Managing event categories.
- Enrolling participants in events.
- Preventing duplicate enrolments.
- Recording and retrieving race results.
- Retrieving participant information.
- Retrieving and updating user profiles.
- Checking access restrictions.

API responses were examined to confirm the behaviour of the implemented endpoints.

## 10. AUTOMATED UNIT TESTING

Automated tests were developed using the xUnit testing framework.

The tests are included in the RaceDay.Tests project.

The tests cover validation and application behaviour, including:

- User role validation.
- Event type validation.
- Password hashing and verification.
- Finishing time validation.
- Finishing position validation.

### Local Test Results

The previously recorded local test results were:

- Total tests: 24
- Passed: 24
- Failed: 0
- Skipped: 0
- Success rate: 100%
- Execution time: Approximately 1.8 seconds

These results describe the previously recorded local test run and should be checked again against the final submitted code.

## 11. GITHUB AND CONTINUOUS INTEGRATION

GitHub is used to store and manage the RaceDay source code.

The repository contains the ASP.NET Core Web API, MVC application and associated test project.

GitHub Actions is intended to automate the following processes:

- Restoring project dependencies.
- Building the application.
- Running automated tests.
- Reporting build and test results.

The final GitHub Actions workflow status must be verified in the repository before submission.

## 12. HOW TO RUN THE APPLICATION

### Requirements

- Visual Studio 2022
- The .NET SDK version required by the solution
- SQL Server LocalDB
- SQL Server Management Studio
- An available RaceDay database or database setup script

### Running the Project

1. Download or clone the RaceDay repository.
2. Open the RaceDay solution in Visual Studio 2022.
3. Ensure SQL Server LocalDB is installed.
4. Open SQL Server Management Studio.
5. Create or restore the Raceday database using the provided database script.
6. Verify the connection string in the API configuration.
7. Restore the required NuGet packages.
8. Build the solution using Ctrl + Shift + B.
9. Start the RaceDay API using its HTTPS launch profile.
10. Start the RaceDay MVC application.
11. Open the MVC website in a browser.
12. Register or log in using a demonstration account.
13. Test the features available to the selected user role.

The API must be running for the MVC application to communicate with it.

The MVC application's API base URL must match the address and port used by the running API.

### Running Automated Tests

1. Open the solution in Visual Studio 2022.
2. Select Test from the main menu.
3. Open Test Explorer.
4. Select Run All Tests.
5. Review the test results.

## 13. PROJECT STRUCTURE

The RaceDay solution contains the following main components:

- **RaceDay.Api:** Backend RESTful API, controllers, data access and database operations.
- **RaceDay.Mvc:** MVC website, controllers, Razor views and API communication services.
- **RaceDay.Tests:** Automated tests for the application.

The solution also includes configuration files and project dependencies required for development and execution.

## 14. CONCLUSION

The RaceDay Event Management System demonstrates the development of a web-based application using ASP.NET Core Web API, ASP.NET Core MVC, C# and Microsoft SQL Server.

The system combines backend functionality with a role-based website that allows Organisers to manage races and Participants to browse and enrol in events.

The project demonstrates practical knowledge of database development, RESTful APIs, MVC architecture, authentication, role-based authorisation, password security and automated testing.

The application was developed using Visual Studio 2022, with SQL Server used for data storage and GitHub used for source code management.

Final verification of the database setup, complete MVC workflows and GitHub Actions should be performed before submission.
https://github.com/zenandesabeliwe/RaceDay.git - github
