/* Optional SQL Server database bootstrap. EF Core is the source of truth.
   The application creates the database from EF migrations when migrations are added/applied.
   This script only creates the database container if you want to prepare SQL Server first. */
IF DB_ID(N'StaybnbDb') IS NULL
    CREATE DATABASE StaybnbDb;
GO
