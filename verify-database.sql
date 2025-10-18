-- Database Verification Script for ProTrack
-- This script verifies that the database was created successfully with all tables and seeded data

-- Check if database exists
SELECT name FROM sys.databases WHERE name = 'ProTrackDb';

-- Check all tables
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Check seeded demo user
SELECT Id, FirstName, LastName, Email, CompanyName 
FROM AspNetUsers 
WHERE Id = 'demo-user-123';

-- Check seeded clients
SELECT Id, Name, ContactEmail, PhoneNumber, IsActive 
FROM Clients 
WHERE UserId = 'demo-user-123';

-- Check seeded projects
SELECT Id, Title, Description, HourlyRate, Status 
FROM Projects 
WHERE UserId = 'demo-user-123';

-- Check table relationships
SELECT 
    t.name AS TableName,
    c.name AS ColumnName,
    fk.name AS ForeignKeyName,
    rt.name AS ReferencedTable
FROM sys.tables t
LEFT JOIN sys.foreign_keys fk ON t.object_id = fk.parent_object_id
LEFT JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
LEFT JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
LEFT JOIN sys.tables rt ON fkc.referenced_object_id = rt.object_id
WHERE t.name IN ('Clients', 'Projects', 'TimeEntries', 'Invoices')
ORDER BY t.name, c.name;
