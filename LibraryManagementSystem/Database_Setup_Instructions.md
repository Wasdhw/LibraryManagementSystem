# Library Management System - Database Setup Instructions

## Overview
This document provides step-by-step instructions for setting up the SQL Server database for the Library Management System using Azure SQL Database.

## Prerequisites
- Azure SQL Database instance
- SQL Server Management Studio (SSMS) or Azure Data Studio
- Access to create databases and users in Azure SQL

## Database Schema

### Tables Created:
1. **users** - Stores user information with role-based access
2. **books** - Stores book catalog information
3. **issues** - Tracks book borrowing and returns

### Key Features:
- Role-based authentication (admin/student)
- Soft delete functionality
- Audit trails with timestamps
- Performance-optimized indexes
- Comprehensive views and stored procedures

## Setup Instructions

### Step 1: Create Database
1. Connect to your Azure SQL Database server
2. Run the SQL script: `LibraryManagementSystem_Database_Schema.sql`
3. The script will create:
   - Database schema with all tables
   - Indexes for performance
   - Views for common queries
   - Stored procedures for business logic
   - Sample data for testing

### Step 2: Verify Database Creation
Run the following query to verify all tables were created:
```sql
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

### Step 3: Test Default Admin Account
The script creates a default admin account:
- **Username:** admin
- **Password:** admin123
- **Role:** admin

### Step 4: Connection String Configuration
Update your application's connection strings to use:
```
Server=tcp:sdsc-johnmenardmarcelo.database.windows.net,1433;Initial Catalog=LibrarySystemDB;Persist Security Info=False;User ID=app_user;Password=StrongP@ssw0rd!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

## Database Schema Details

### Users Table
```sql
CREATE TABLE users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    idcode NVARCHAR(50) NOT NULL UNIQUE,
    username NVARCHAR(50) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL,
    role NVARCHAR(20) NOT NULL DEFAULT 'student' CHECK (role IN ('admin', 'student')),
    date_register DATETIME2 NOT NULL DEFAULT GETDATE(),
    date_update DATETIME2 NULL,
    date_delete DATETIME2 NULL,
    is_active BIT NOT NULL DEFAULT 1
);
```

### Books Table
```sql
CREATE TABLE books (
    id INT IDENTITY(1,1) PRIMARY KEY,
    book_title NVARCHAR(200) NOT NULL,
    author NVARCHAR(100) NOT NULL,
    published_date DATE NOT NULL,
    quantity INT NOT NULL DEFAULT 1,
    status NVARCHAR(20) NOT NULL DEFAULT 'Available' CHECK (status IN ('Available', 'Issued', 'Maintenance')),
    image NVARCHAR(500) NULL,
    date_insert DATETIME2 NOT NULL DEFAULT GETDATE(),
    date_update DATETIME2 NULL,
    date_delete DATETIME2 NULL,
    is_active BIT NOT NULL DEFAULT 1
);
```

### Issues Table
```sql
CREATE TABLE issues (
    id INT IDENTITY(1,1) PRIMARY KEY,
    issue_id NVARCHAR(50) NOT NULL UNIQUE,
    user_id INT NOT NULL,
    book_id INT NOT NULL,
    full_name NVARCHAR(100) NOT NULL,
    contact NVARCHAR(20) NOT NULL,
    book_title NVARCHAR(200) NOT NULL,
    author NVARCHAR(100) NOT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'Not Return' CHECK (status IN ('Not Return', 'Return', 'Overdue')),
    issue_date DATE NOT NULL,
    return_date DATE NOT NULL,
    actual_return_date DATE NULL,
    date_insert DATETIME2 NOT NULL DEFAULT GETDATE(),
    date_update DATETIME2 NULL,
    date_delete DATETIME2 NULL,
    is_active BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT FK_issues_user_id FOREIGN KEY (user_id) REFERENCES users(id),
    CONSTRAINT FK_issues_book_id FOREIGN KEY (book_id) REFERENCES books(id)
);
```

## Role-Based Access Control

### Admin Role
- Full access to all system features
- Can manage books (add, edit, delete)
- Can issue and return books
- Can view all user accounts
- Redirected to `MainForm` after login

### Student Role
- Limited access to student features
- Can view available books
- Can view their borrowing history
- Cannot manage books or other users
- Redirected to `StudentForm` after login

## Key Views

### vw_active_users
Shows all active users (not deleted)

### vw_available_books
Shows all available books for borrowing

### vw_current_issues
Shows all books currently issued (not returned)

### vw_overdue_books
Shows all overdue books with days overdue

## Stored Procedures

### sp_AuthenticateUser
Authenticates user login and returns user information including role

### sp_GetDashboardData
Returns dashboard statistics (total users, books, available books, etc.)

### sp_IssueBook
Handles book issuing with automatic quantity updates

### sp_ReturnBook
Handles book returns with automatic quantity updates

## Security Features

1. **Role-based Access**: Users are assigned roles (admin/student) that determine their access level
2. **Soft Delete**: Records are marked as deleted rather than physically removed
3. **Audit Trail**: All changes are tracked with timestamps
4. **Input Validation**: Database constraints ensure data integrity
5. **SQL Injection Protection**: All queries use parameterized statements

## Application Integration

### Login Process
1. User enters username and password
2. System queries database for user credentials
3. If valid, system checks user role
4. Admin users → MainForm
5. Student users → StudentForm

### Registration Process
1. New users are automatically assigned 'student' role
2. Username and ID code must be unique
3. Password confirmation required

## Troubleshooting

### Common Issues:
1. **Connection Failed**: Verify Azure SQL server is accessible and firewall rules allow connections
2. **Login Failed**: Check username/password and ensure user is active
3. **Role Issues**: Verify user has correct role assigned in database
4. **Permission Errors**: Ensure app_user has necessary permissions

### Useful Queries:
```sql
-- Check all users
SELECT * FROM vw_active_users;

-- Check available books
SELECT * FROM vw_available_books;

-- Check current issues
SELECT * FROM vw_current_issues;

-- Check overdue books
SELECT * FROM vw_overdue_books;
```

## Maintenance

### Regular Tasks:
1. Monitor overdue books using `vw_overdue_books`
2. Clean up old deleted records periodically
3. Update book quantities as needed
4. Backup database regularly

### Performance Optimization:
- Indexes are created for frequently queried columns
- Views provide optimized query paths
- Stored procedures reduce network traffic

## Support

For technical support or questions about the database schema, please refer to the application documentation or contact the development team.
