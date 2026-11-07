IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Departments] (
    [Id] int NOT NULL IDENTITY,
    [DepartmentName] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([Id])
);

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(50) NOT NULL,
    [Email] nvarchar(256) NOT NULL,
    [PasswordHash] nvarchar(256) NOT NULL,
    [Role] nvarchar(20) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [Employees] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeNumber] nvarchar(20) NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [Email] nvarchar(256) NOT NULL,
    [PhoneNumber] nvarchar(20) NULL,
    [DateOfBirth] date NOT NULL,
    [HireDate] date NOT NULL,
    [Position] nvarchar(100) NOT NULL,
    [Salary] decimal(18,2) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [DepartmentId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Employees_HireDate_AfterBirth] CHECK ([HireDate] > [DateOfBirth]),
    CONSTRAINT [CK_Employees_Salary_NonNegative] CHECK ([Salary] >= 0),
    CONSTRAINT [FK_Employees_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [IX_Departments_DepartmentName] ON [Departments] ([DepartmentName]);

CREATE INDEX [IX_Employees_DepartmentId] ON [Employees] ([DepartmentId]);

CREATE UNIQUE INDEX [IX_Employees_Email] ON [Employees] ([Email]);

CREATE UNIQUE INDEX [IX_Employees_EmployeeNumber] ON [Employees] ([EmployeeNumber]);

CREATE INDEX [IX_Employees_LastName_FirstName] ON [Employees] ([LastName], [FirstName]);

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);

CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261105030019_InitialCreate', N'10.0.14');

COMMIT;
GO

