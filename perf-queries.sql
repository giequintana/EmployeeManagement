USE EmployeeManagementDb;
SET STATISTICS IO, TIME ON;

DECLARE @departmentId int = 3;
DECLARE @isActive bit = 1;
DECLARE @term nvarchar(100) = N'santos';

-- Q1: default list, first page
SELECT e.Id, e.EmployeeNumber, e.FirstName, e.LastName, e.Email, e.Position,
       e.DepartmentId, d.DepartmentName, e.HireDate, e.IsActive
FROM Employees e
INNER JOIN Departments d ON e.DepartmentId = d.Id
ORDER BY e.LastName, e.FirstName, e.Id
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;

-- Q2a: department + active filter, first page (count)
SELECT COUNT(*) FROM Employees e
WHERE e.DepartmentId = @departmentId AND e.IsActive = @isActive;

-- Q2b: department + active filter, first page (rows)
SELECT e.Id, e.EmployeeNumber, e.FirstName, e.LastName, e.Email, e.Position,
       e.DepartmentId, d.DepartmentName, e.HireDate, e.IsActive
FROM Employees e
INNER JOIN Departments d ON e.DepartmentId = d.Id
WHERE e.DepartmentId = @departmentId AND e.IsActive = @isActive
ORDER BY e.LastName, e.FirstName, e.Id
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;

-- Q3: search (count)
SELECT COUNT(*) FROM Employees e
WHERE e.EmployeeNumber LIKE N'%' + @term + N'%'
   OR e.FirstName LIKE N'%' + @term + N'%'
   OR e.LastName LIKE N'%' + @term + N'%'
   OR (e.FirstName + N' ' + e.LastName) LIKE N'%' + @term + N'%';

-- Q4: dashboard distribution
SELECT d.Id, d.DepartmentName,
       (SELECT COUNT(*) FROM Employees e WHERE e.DepartmentId = d.Id) AS EmployeeCount,
       (SELECT COUNT(*) FROM Employees e WHERE e.DepartmentId = d.Id AND e.IsActive = 1) AS ActiveCount
FROM Departments d
ORDER BY d.DepartmentName;

SET STATISTICS IO, TIME OFF;