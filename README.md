# Gym Management System

Windows Forms application targeting .NET Framework 4.8 that helps gyms manage members, staff, schedules, and inventory from a single desktop client. The solution currently ships with receptionist, trainer, manager, and member workflows implemented as dedicated forms under the `WindowsProgramDesign` project.

## Prerequisites
- Windows 10/11 with Visual Studio 2019 or later and the **.NET desktop development** workload installed
- .NET Framework 4.8 Developer Pack
- SQL Server (Express or full) with a `GymManagementSystem` database
- Access to update the connection string stored in `WindowsProgramDesign/DatabaseConfig.cs`

## Getting Started
1. Clone the repository and open `WindowsProgramDesign.sln` in Visual Studio.
2. Update `WindowsProgramDesign/DatabaseConfig.cs` so `DatabaseConfig.ConnectionString` points to your SQL Server instance.
3. Ensure the database contains the tables referenced across the forms (for example `Users`, `Receptionists`, `ReceptionistProfile`, `GymMembers`, `TrainingSession`, `Products`).
4. Set `WindowsProgramDesign` as the startup project, press `F5`, and log in with a user stored in the `Users` table.

## Project Layout
| Path | Purpose |
| --- | --- |
| `WindowsProgramDesign/WindowsProgramDesign.csproj` | WinForms project file |
| `WindowsProgramDesign/DatabaseConfig.cs` | Central connection string helper |
| `WindowsProgramDesign/LoginPage.cs` | Application entry form for authentication |
| `WindowsProgramDesign/Registration.cs` | Receptionist self-registration workflow |
| `WindowsProgramDesign/*Main.cs` | Role-specific dashboards (Manager, Receptionist, Trainer, Gym Member) |
| `WindowsProgramDesign/*.Designer.cs` | Auto-generated UI layout definitions |
| `WindowsProgramDesign/*.resx` | Localized resources for each form |

## Configuration Notes
- Keep `DatabaseConfig.ConnectionString` synchronized with your environment; use Windows Authentication where possible, or secure SQL credentials through user secrets or environment variables.
- Each form that interacts with SQL Server uses `System.Data.SqlClient`; make sure the target database schema matches the expected columns before running the application.

## Build & Troubleshooting
- Use Visual Studio's Build > Build Solution to compile; the project is configured for C# 7.3 / .NET Framework 4.8.
- If you encounter `SqlException` errors, verify the server name, firewall access, and whether TLS requires `TrustServerCertificate=True` in the connection string.
- Designer issues typically resolve by cleaning the solution (`Build > Clean Solution`) and reopening the form designers.

## Contributing
1. Create a branch from `master` and keep changes scoped (UI, data access, reporting, etc.).
2. Run through key workflows (login, receptionist registration, dashboard navigation) before opening a pull request.
3. Document any database schema changes and provide scripts or migration notes alongside the code update.
