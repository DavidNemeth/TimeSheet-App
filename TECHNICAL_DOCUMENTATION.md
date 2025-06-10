# Technical Documentation - TimeSheet Application

This document provides technical details for developers working on the TimeSheet Application.

## Table of Contents

1.  [Project Overview](#project-overview)
2.  [Getting Started & Prerequisites](#getting-started--prerequisites)
3.  [Project Setup](#project-setup)
    *   [Cloning the Repository](#cloning-the-repository)
    *   [Backend Setup (TimeSheetAPI)](#backend-setup-timesheetapi)
    *   [Frontend Setup (TimeSheet.Web)](#frontend-setup-timesheetweb)
4.  [Backend - TimeSheetAPI](#backend---timesheetapi)
    *   [Technology Stack](#technology-stack-backend)
    *   [Project Structure](#project-structure-backend)
    *   [Database](#database)
    *   [Configuration](#configuration-backend)
    *   [Authentication and Authorization](#authentication-and-authorization-backend)
    *   [API Endpoints](#api-endpoints)
    *   [Running the Backend](#running-the-backend)
    *   [Running Backend Tests](#running-backend-tests)
5.  [Frontend - TimeSheet.Web](#frontend---timesheetweb)
    *   [Technology Stack](#technology-stack-frontend)
    *   [Project Structure](#project-structure-frontend)
    *   [Configuration](#configuration-frontend)
    *   [Key Components](#key-components-frontend)
    *   [Services](#services-frontend)
    *   [Authentication](#authentication-frontend)
    *   [Running the Frontend](#running-the-frontend)
    *   [Running Frontend Tests](#running-frontend-tests)
6.  [Deployment](#deployment)
7.  [Contribution Guidelines](#contribution-guidelines)

---

## 1. Project Overview

The TimeSheet Application is a full-stack solution with a .NET Web API backend and an ASP.NET Core hosted Blazor frontend. It allows users to manage timesheet entries, track work hours, and submit them for approval.

---

## 2. Getting Started & Prerequisites

Before you begin, ensure you have the following installed:
*   **.NET SDK:** Version 8.0 or higher.
*   **Docker:** (Optional, for running the application in containers).
*   **Git:** For cloning the repository.
*   **IDE:** An IDE like Visual Studio 2022, JetBrains Rider, or VS Code.
*   **Database:** Access to a PostgreSQL instance for the backend.
*   **Azure DevOps PAT:** A Personal Access Token for Azure DevOps with 'Packaging (read)' permissions if you need to restore the `SharedLib` NuGet package from the private feed. This is required for building from source if the package is not already cached or available elsewhere.

---

## 3. Project Setup

### Cloning the Repository
```bash
git clone <repository_url> # Replace <repository_url> with the actual URL
cd <repository_directory>
```

### Backend Setup (TimeSheetAPI)
1.  **Navigate to the backend directory:**
    ```bash
    cd backend/TimeSheetAPI
    ```
2.  **Restore Dependencies:**
    ```bash
    dotnet restore
    ```
3.  **Configure `appsettings.Development.json`:**
    *   Ensure you have a valid PostgreSQL connection string. The application looks for a key like `PG-TIMESHEET-SQL-CONNECTION-STRING-DEV`.
        ```json
        "PG-TIMESHEET-SQL-CONNECTION-STRING-DEV": "Host=your_postgres_host;Port=5432;Database=your_timesheet_db;Username=your_user;Password=your_password;"
        ```
    *   Configure Azure AD settings and KeyVault endpoint if needed for your local development, or ensure appropriate user secrets are set up. The application attempts to load secrets from Azure KeyVault.
    *   The application uses HTTPS and expects a certificate (details in `Program.cs` and `appsettings.json` for `OPPO-APP-TLS-CERT-NAME`). For local development on Windows, development certificates are often handled by IIS Express or `dotnet dev-certs`. For Linux/macOS or direct Kestrel usage, you might need to set up a development certificate and configure its path and password in user secrets or `appsettings.Development.json`. The configured path `/etc/tls/` is Linux-specific.
4.  **Database Migrations:**
    *   Ensure the Entity Framework Core tools are installed (`dotnet tool install --global dotnet-ef`).
    *   Apply database migrations from the `backend/TimeSheetAPI` directory:
        ```bash
        dotnet ef database update
        ```

### Frontend Setup (TimeSheet.Web)
1.  **Navigate to the frontend directory:**
    ```bash
    cd frontend/TimeSheet/TimeSheet.Web
    ```
2.  **Configure NuGet for Private Feed (if needed):**
    *   The project references a `SharedLib` package from a private Azure DevOps feed (`https://pkgs.dev.azure.com/ContainerboardIT/...`).
    *   If you haven't configured this feed before, you might need to add it to your NuGet sources or ensure your Azure DevOps PAT is available to restore this package. The `dockerfile` for the frontend includes steps to use a `NUGET_FEED_PAT`. For local development, ensure your environment can authenticate to this feed. One way is to create a `nuget.config` file in the solution root or user profile with the necessary credentials or use a credential provider.
3.  **Restore Dependencies:**
    ```bash
    dotnet restore
    ```
4.  **Configure `appsettings.Development.json`:**
    *   Set the `ApiBaseUrl` to point to your running backend instance (e.g., `https://localhost:8443/apitimesheet/` if running backend locally with default settings).
        ```json
        "ApiBaseUrl": "https://localhost:8443/apitimesheet/"
        ```
    *   Review `AzureAd` settings. For local development, ensure the `ClientId` and `TenantId` are appropriate and that the application is registered in Azure AD with the correct redirect URIs (e.g., `https://localhost:your_frontend_port/signin-oidc`).
    *   Similar to the backend, configure HTTPS certificate details if needed for local Kestrel, or rely on default development certificate handling. The configured path `/etc/tls/` is Linux-specific. The frontend also uses `OPPO-APP-TLS-CERT-NAME`.
    *   Ensure other settings like `UrlSettings:BaseUrl` and Redis configuration (`Redis:InstanceName`) are suitable for your local environment if those features are actively used during development.

---

## 4. Backend - TimeSheetAPI

### Technology Stack (Backend)
*   **.NET 8.0:** Core framework for the API.
*   **ASP.NET Core:** For building web APIs.
*   **Entity Framework Core:** ORM for database interaction.
*   **PostgreSQL:** Relational database.
*   **Swashbuckle (Swagger):** For API documentation and testing (available at `/apitimesheet/swagger`).
*   **AutoMapper:** For object-to-object mapping.
*   **Docker:** Containerization support is available.

### Project Structure (Backend)
Located under `backend/TimeSheetAPI/`:
*   **`Controllers/`:** Contains API controllers (e.g., `TimesheetEntriesController.cs`).
*   **`Data/`:** Contains `AppDbContext.cs` for Entity Framework Core.
*   **`DTOs/`:** Data Transfer Objects used for API requests and responses.
*   **`Mappings/`:** AutoMapper profiles.
*   **`Migrations/`:** EF Core database migration files.
*   **`Models/`:** Domain models/entities.
*   **`Services/`:** Business logic services (e.g., `TimesheetService.cs`).
    *   **`Interfaces/`:** Service interfaces.
*   **`Program.cs`:** Application startup and service configuration.
*   **`appsettings.json` (and environment variants):** Configuration files.
*   **`dockerfile`:** For building the Docker image.
*   **`TimeSheet.Api.csproj`:** Project file defining dependencies and build settings.

### Database
*   The application uses PostgreSQL as its database.
*   Entity Framework Core is used for data access and migrations.
*   The connection string is configured in `appsettings.json` (or environment-specific versions / user secrets / KeyVault) under a key like `PG-TIMESHEET-SQL-CONNECTION-STRING-<ENV>`.
*   Database schema is managed via EF Core migrations located in the `Migrations/` folder.

### Configuration (Backend)
*   Configuration is managed via `appsettings.json`, environment-specific files (e.g., `appsettings.Development.json`), environment variables, user secrets, and Azure KeyVault.
*   Key settings in `appsettings.json`:
    *   `AppSettings:ENV`: Specifies the current environment (e.g., "DEV", "PROD").
    *   `AppSettings:ExposedHttpsPort`: The port Kestrel will listen on for HTTPS.
    *   `AzureAd`: Configuration for Azure Active Directory.
    *   `AzureKeyVault:Endpoint`: Endpoint for Azure KeyVault.
*   Secrets like database connection strings and certificate passwords should be stored securely.

### Authentication and Authorization (Backend)
*   The system uses a token-based authentication mechanism, likely for service-to-service calls or if the API is consumed by non-user clients.
*   `TokenService.cs` and `AuthenticationMessageHandler.cs` are involved in handling this.
*   Azure AD settings are present, used for KeyVault access and potentially other service principal authentications.
*   User authentication for the entire system is primarily handled by the frontend's OpenID Connect flow with Azure AD. The backend API then expects authenticated calls, potentially validating tokens passed from the frontend.

### API Endpoints
The primary controller is `TimesheetEntriesController.cs`, mounted under `/api/timesheetentries` (prefixed by `/apitimesheet` globally, so full base path is `/apitimesheet/api/timesheetentries`).

*   **`GET /`**: Retrieves timesheet entries.
    *   Query Parameters: `fromDate` (DateTime), `toDate` (DateTime), `userId` (string, optional), `forRole` (bool, optional), `archived` (bool, optional).
*   **`GET /{id}`**: Retrieves a specific timesheet entry by ID.
*   **`POST /`**: Creates a new timesheet entry.
    *   Request Body: `TimesheetEntryDto`
*   **`PUT /{id}`**: Updates an existing timesheet entry.
    *   Request Body: `TimesheetEntryDto`
*   **`DELETE /{id}`**: Deletes a timesheet entry.
*   **`POST /{id}/archive`**: Archives a timesheet entry.
    *   Request Body: `string` (representing `archivedBy`).
*   **`POST /{id}/unarchive`**: Unarchives a timesheet entry.
    *   Request Body: `string` (representing `unArchivedBy`).
*   **`POST /inithistory`**: (Potentially a utility endpoint for initializing history records).

Swagger UI is available at `/apitimesheet/swagger` for detailed API exploration and testing.

### Running the Backend
1.  Ensure all setup steps (dependencies, configuration, database) are complete.
2.  From the `backend/TimeSheetAPI` directory:
    ```bash
    dotnet run
    ```
3.  The API will typically be available at `https://localhost:<ExposedHttpsPort>/apitimesheet` (e.g., `https://localhost:8443/apitimesheet` if default `ExposedHttpsPort` from `appsettings.json` is 8443).

**Using Docker:**
1.  Ensure Docker is running.
2.  From the `backend/TimeSheetAPI` directory:
    ```bash
    docker build -t timesheet-api .
    # The Dockerfile exposes 8080 (HTTP). Map to a host port.
    docker run -p 8080:8080 timesheet-api
    ```
    The API inside the container would be at `http://localhost:8080/apitimesheet`. For HTTPS, the Docker setup would need adjustments.

### Running Backend Tests
*(This section assumes no specific test project was found in the initial `ls`.)*
No dedicated backend test project was identified in the initial file listing. If tests are added later, they would typically be run using `dotnet test` in the test project's directory.

---

## 5. Frontend - TimeSheet.Web

### Technology Stack (Frontend)
*   **.NET 8.0:** Core framework.
*   **ASP.NET Core hosted Blazor:** (Blazor Web App with Interactive Server render mode).
*   **Razor Components:** For building the UI.
*   **Fluent UI Blazor Components:** Used for the UI elements (via `SharedLib`).
*   **C#:** Language for UI logic.
*   **Docker:** Containerization support is available.

### Project Structure (Frontend)
Located under `frontend/TimeSheet/TimeSheet.Web/`:
*   **`Components/`:** Contains Razor components.
    *   **`Pages/`:** Routable components (e.g., `Home.razor`).
    *   **`Shared/`:** Reusable UI components (e.g., `TimesheetForm.razor`, `TimesheetHistoryModal.razor`).
    *   **`Layout/`:** (Expected to contain main layout components like `MainLayout.razor`, though may be part of `SharedLib`).
    *   **`App.razor`:** The root component of the Blazor application.
    *   **`_Imports.razor`:** Common imports for Razor components.
*   **`DTOs/`, `Enums/`, `ViewModels/`:** Client-side data structures.
*   **`Services/`:** Client-side services.
    *   **`Interfaces/`:** Service interfaces.
    *   **`TimesheetService.cs`:** Handles communication with the backend TimeSheet API.
*   **`wwwroot/`:** Static assets (CSS, images, etc.).
*   **`Program.cs`:** Application startup, service registration, and middleware configuration.
*   **`appsettings.json` (and environment variants):** Configuration files.
*   **`dockerfile`:** For building the Docker image.
*   **`TimeSheet.Web.csproj`:** Project file.
*   **`nuget.config`:** Specifies NuGet package sources, including the private Azure DevOps feed for `SharedLib`.

### Configuration (Frontend)
*   Configuration is managed via `appsettings.json`, environment-specific files, environment variables, user secrets, and Azure KeyVault.
*   Key settings in `appsettings.json`:
    *   `ApiBaseUrl`: Crucial setting, points to the backend API (e.g., `https://<backend_host>/apitimesheet/`).
    *   `AzureAd`: Configuration for Azure Active Directory authentication (TenantId, ClientId, CallbackPath).
    *   `UrlSettings`: Base URLs for different parts of a larger application ecosystem.
    *   `AppSettings:ENV`: Current environment.
    *   `AppSettings:ExposedHttpsPort`: Port Kestrel will listen on for HTTPS (default 8443).
    *   `OPPO-APP-TLS-CERT-NAME`: Name of the HTTPS certificate.
*   Path base for the application is `/timesheet`.

### Key Components (Frontend)
*   **`App.razor`:** The root component, sets up routing.
*   **`Home.razor` (`Components/Pages/`):** The main page displaying the timesheet grid, action buttons (Add New, Export), month selector, and tabs for Active/Archived entries. Uses `GenericDataGrid` from `SharedLib`.
*   **`TimesheetForm.razor` (`Components/Shared/`):** A dialog component for creating and editing timesheet entries. Includes fields for date, machine, overtime, dirt bonus, descriptions, and approval actions.
*   **`TimesheetHistoryModal.razor` (`Components/Shared/`):** A dialog for displaying the history of a timesheet entry.
*   Layout components (likely `MainLayout.razor`, potentially part of `SharedLib` or in `Components/Layout/`) define the overall page structure.

### Services (Frontend)
*   **`ITimesheetService` / `TimesheetService.cs`:** Responsible for making HTTP calls to the backend `TimeSheetAPI` for CRUD operations on timesheet entries, archiving, and unarchiving.
*   Services from `SharedLib` (e.g., `IDialogService`, `IToastService`, `INotificationService`, `IUserHttpService`, `IEntityHistoryHttpService`) are injected and used for UI interactions, user data, and history.

### Authentication (Frontend)
*   Uses Microsoft Identity Platform (Azure AD) for user authentication via OpenID Connect.
*   Configuration is handled in `Program.cs` with `AddMicrosoftIdentityUI()` and settings from `appsettings.json` (`AzureAd` section).
*   `[Authorize]` attributes and user permission checks (e.g., `AppUser.Permissions.Any(...)`) are used throughout components to control access to features.
*   User information (`UserViewModel`) is cascaded or injected into components.

### Running the Frontend
1.  Ensure the Backend API is running and accessible.
2.  Ensure all frontend setup steps (dependencies, configuration) are complete.
3.  From the `frontend/TimeSheet/TimeSheet.Web` directory:
    ```bash
    dotnet run
    ```
4.  The application will typically be available at `https://localhost:<ExposedHttpsPort>/timesheet` (e.g., `https://localhost:8443/timesheet` if default `ExposedHttpsPort` is 8443).

**Using Docker:**
1.  Ensure Docker is running.
2.  From the `frontend/TimeSheet/TimeSheet.Web` directory:
    ```bash
    # You'll need to pass the NUGET_FEED_PAT as a build argument
    docker build --build-arg NUGET_FEED_PAT="YOUR_AZDO_PAT" -t timesheet-web .
    # The Dockerfile exposes 8443 (HTTPS). Map to a host port.
    # This assumes the container has the necessary certs configured as per Program.cs.
    # For local testing without complex cert setup in Docker, you might modify the
    # Dockerfile to run on HTTP and map that.
    docker run -p 8443:8443 timesheet-web
    ```
    The application inside the container would be at `https://localhost:8443/timesheet`.

### Running Frontend Tests
*(This section assumes no specific test project was found in the initial `ls`.)*
No dedicated frontend test project was identified in the initial file listing. If tests (e.g., bUnit for component tests) are added later, they would typically be run using `dotnet test` in the test project's directory.

---

## 6. Deployment

*(Placeholder - More specific details would depend on the target environment, e.g., Azure App Service, Kubernetes, IIS.)*

General steps for deployment would involve:
*   **Build Release Artifacts:**
    ```bash
    dotnet publish -c Release -o ./publish_output backend/TimeSheetAPI/TimeSheet.Api.csproj
    dotnet publish -c Release -o ./publish_output_frontend frontend/TimeSheet/TimeSheet.Web/TimeSheet.Web.csproj
    ```
    Or, build Docker images for both backend and frontend using their respective Dockerfiles.
*   **Configuration Management:** Ensure all environment-specific configurations (connection strings, API URLs, KeyVault endpoints, Azure AD settings) are correctly set up in the deployment environment (e.g., using App Service Configuration, Kubernetes ConfigMaps/Secrets, environment variables).
*   **Backend Deployment:** Deploy the `TimeSheetAPI` publish output or Docker image to the hosting environment. Ensure it's accessible to the frontend.
*   **Frontend Deployment:** Deploy the `TimeSheet.Web` publish output or Docker image. Ensure `ApiBaseUrl` in its configuration points to the deployed backend.
*   **HTTPS Certificates:** Properly configure HTTPS certificates in the hosting environment. The application is designed to load certificates from a specified path.
*   **Database:** Ensure the PostgreSQL database is deployed and accessible, and migrations have been run.
*   **Azure Resources:** Ensure Azure KeyVault, Azure AD App Registrations, and potentially Redis cache are set up and accessible.

The provided `azure-pipelines.yml` files in both `backend/TimeSheetAPI` and `frontend/TimeSheet` suggest that Azure Pipelines are used for CI/CD, which would contain the automated build and deployment steps.

---

## 7. Contribution Guidelines

*(Placeholder - To be defined by the project team.)*
General guidelines usually include:
*   **Branching Strategy:** e.g., GitFlow (feature branches, develop, main/master).
*   **Code Style:** Adhere to existing code style. Consider using linters or formatters.
*   **Pull Requests (PRs):**
    *   Create PRs for all changes.
    *   Ensure PRs are reviewed.
    *   Ensure builds pass and any tests are successful.
*   **Commit Messages:** Follow a consistent style (e.g., Conventional Commits).
*   **Issue Tracking:** Use the project's issue tracker.
*   **Testing:** Write unit/integration tests for new features or bug fixes.

---
This technical documentation should provide a good starting point for developers.
