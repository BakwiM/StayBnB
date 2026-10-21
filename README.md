# Staybnb - PROGRAMMING 742

ASP.NET Core MVC + Entity Framework Core + SQL Server implementation of the Staybnb assignment.

## Requirements implemented
- Guest registration and automatic Guest role.
- SuperAdmin seeding at startup.
- After admin approval, the guest position is eliminated and the guest becomes the host.
- Dashboard protection based on roles.
- A property image is required for the host application.
- The active/inactive toggle and host property administration.
- Requesting a reservation, approving or rejecting it, and calculating the total cost.
- Upload a guest ID or passport for reservations that have been approved.
- Rules for host check-in and necessary paperwork.
- SuperAdmin guest-to-Admin promotion, logs, and admin application assessment.
- Messages and alerts supported by databases.
- Registration, login, role changes, applications, and booking changes all have activity logs.

## Seeded SuperAdmin
Email: superadmin@staybnb.local
Password: Admin123!

Change the seeded password before production use.

## How to run
1. Install SQL Server LocalDB/SQL Server and the.NET 8 SDK.
2. In this folder, launch a terminal.
3. Packages for restoration
`dotnet restore`
4. Start the initial migration:
`dotnet ef migrations add InitialCreate`
5. Use it:
`dotnet ef database update`
6. Run
`dotnet run`
7. Click on the HTTPS URL that ASP.NET Core displays.

The file `appsettings.json` contains the connection string. If your SQL Server instance is different, update it.

## Submission
The assignment requests a single ZIP under 50 MB containing the complete project, report and supporting files. This ZIP contains the source project and setup documentation. Build artifacts such as bin/obj are intentionally excluded.

## Checklist for testing
-Create a new account; it should be named Guest.
-A guest must submit an image in order to be considered as a host.
-Approve the application after logging in as SuperAdmin.
-Verify that the applicant is no longer a guest and is now a host.
-Toggle properties, set up check-in, and accept or reject reservations on the host panel.
-Visitors can explore, filter, make reservations, and view the computed total.
-Upload your ID or passport if you are an authorized guest.
-The booking is checked in after the host verifies the uploaded document.Check notifications and send messages.
-Administrators should examine activity logs and review applications.
