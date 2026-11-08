USE EmployeeManagementDb;
SET NOCOUNT ON;

IF EXISTS (SELECT 1 FROM Employees WHERE EmployeeNumber LIKE N'PERF-%')
BEGIN
    PRINT 'Performance data already seeded.';
    RETURN;
END;

DECLARE @Departments TABLE (Idx int IDENTITY(0, 1), DepartmentId int);
INSERT INTO @Departments (DepartmentId) SELECT Id FROM Departments ORDER BY Id;
DECLARE @DepartmentCount int = @@ROWCOUNT;

DECLARE @FirstNames TABLE (Idx int IDENTITY(0, 1), Name nvarchar(100));
INSERT INTO @FirstNames (Name) VALUES
    (N'Maria'), (N'Jose'), (N'Ana'), (N'Mark'), (N'Liza'),
    (N'Paolo'), (N'Grace'), (N'Ramon'), (N'Joy'), (N'Carlo');

DECLARE @LastNames TABLE (Idx int IDENTITY(0, 1), Name nvarchar(100));
INSERT INTO @LastNames (Name) VALUES
    (N'Santos'), (N'Reyes'), (N'Cruz'), (N'Bautista'), (N'Ocampo'),
    (N'Garcia'), (N'Mendoza'), (N'Torres'), (N'Tomas'), (N'Andrada'),
    (N'Castillo'), (N'Flores'), (N'Villanueva'), (N'Ramos'), (N'Castro'),
    (N'Rivera'), (N'Aquino'), (N'Navarro'), (N'Salazar'), (N'Mercado');

WITH Numbers AS (
    SELECT TOP (50000) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS N
    FROM sys.all_objects a CROSS JOIN sys.all_objects b
)
INSERT INTO Employees
    (EmployeeNumber, FirstName, LastName, Email, PhoneNumber, DateOfBirth, HireDate,
     Position, Salary, DepartmentId, IsActive, CreatedAt)
SELECT
    CONCAT(N'PERF-', n.N),
    f.Name,
    l.Name,
    CONCAT(N'perf.', n.N, N'@example.com'),
    NULL,
    DATEADD(DAY, -((n.N % 60) * 97), CAST('1998-12-31' AS date)),
    DATEADD(DAY, (n.N % 60) * 37, CAST('2015-01-05' AS date)),
    CASE n.N % 4 WHEN 0 THEN N'Software Developer' WHEN 1 THEN N'Analyst'
                 WHEN 2 THEN N'Specialist' ELSE N'Team Lead' END,
    30000 + (n.N % 60) * 750,
    d.DepartmentId,
    CASE WHEN n.N % 7 = 0 THEN 0 ELSE 1 END,
    SYSUTCDATETIME()
FROM Numbers n
JOIN @FirstNames f ON f.Idx = n.N % 10
JOIN @LastNames l ON l.Idx = n.N % 20
JOIN @Departments d ON d.Idx = n.N % @DepartmentCount;

PRINT CONCAT('Inserted ', @@ROWCOUNT, ' performance employees.');

UPDATE STATISTICS Employees WITH FULLSCAN;