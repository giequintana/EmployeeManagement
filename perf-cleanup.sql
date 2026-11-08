USE EmployeeManagementDb;
DELETE FROM Employees WHERE EmployeeNumber LIKE N'PERF-%';
UPDATE STATISTICS Employees WITH FULLSCAN;