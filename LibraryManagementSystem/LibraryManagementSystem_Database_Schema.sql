-- =============================================
-- Library Management System Database Schema
-- For Azure SQL Database
-- =============================================

-- Create Database (if not exists)
-- Note: This should be run in Azure SQL Database Management Studio
-- CREATE DATABASE LibrarySystemDB;

-- Use the database
USE LibrarySystemDB;
GO

-- =============================================
-- Create Users Table
-- =============================================
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
GO

-- =============================================
-- Create Books Table
-- =============================================
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
GO

-- =============================================
-- Create Issues Table (Book Transactions)
-- =============================================
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
    
    -- Foreign Key Constraints
    CONSTRAINT FK_issues_user_id FOREIGN KEY (user_id) REFERENCES users(id),
    CONSTRAINT FK_issues_book_id FOREIGN KEY (book_id) REFERENCES books(id)
);
GO

-- =============================================
-- Create Indexes for Performance
-- =============================================

-- Users table indexes
CREATE INDEX IX_users_username ON users(username);
CREATE INDEX IX_users_idcode ON users(idcode);
CREATE INDEX IX_users_role ON users(role);
CREATE INDEX IX_users_active ON users(is_active);

-- Books table indexes
CREATE INDEX IX_books_title ON books(book_title);
CREATE INDEX IX_books_author ON books(author);
CREATE INDEX IX_books_status ON books(status);
CREATE INDEX IX_books_active ON books(is_active);

-- Issues table indexes
CREATE INDEX IX_issues_issue_id ON issues(issue_id);
CREATE INDEX IX_issues_user_id ON issues(user_id);
CREATE INDEX IX_issues_book_id ON issues(book_id);
CREATE INDEX IX_issues_status ON issues(status);
CREATE INDEX IX_issues_issue_date ON issues(issue_date);
CREATE INDEX IX_issues_return_date ON issues(return_date);
CREATE INDEX IX_issues_active ON issues(is_active);

-- =============================================
-- Insert Default Admin User
-- =============================================
INSERT INTO users (name, idcode, username, password, role, date_register)
VALUES ('System Administrator', 'ADMIN001', 'admin', 'admin123', 'admin', GETDATE());
GO

-- =============================================
-- Create Views for Common Queries
-- =============================================

-- View for Active Users
CREATE VIEW vw_active_users AS
SELECT 
    id,
    name,
    idcode,
    username,
    role,
    date_register
FROM users 
WHERE is_active = 1 AND date_delete IS NULL;
GO

-- View for Available Books
CREATE VIEW vw_available_books AS
SELECT 
    id,
    book_title,
    author,
    published_date,
    quantity,
    status,
    image
FROM books 
WHERE is_active = 1 AND date_delete IS NULL AND status = 'Available';
GO

-- View for Current Issues (Not Returned)
CREATE VIEW vw_current_issues AS
SELECT 
    i.id,
    i.issue_id,
    u.name as user_name,
    u.idcode,
    i.full_name,
    i.contact,
    b.book_title,
    b.author,
    i.status,
    i.issue_date,
    i.return_date,
    DATEDIFF(day, i.issue_date, GETDATE()) as days_issued
FROM issues i
INNER JOIN users u ON i.user_id = u.id
INNER JOIN books b ON i.book_id = b.id
WHERE i.is_active = 1 AND i.date_delete IS NULL AND i.status = 'Not Return';
GO

-- View for Overdue Books
CREATE VIEW vw_overdue_books AS
SELECT 
    i.id,
    i.issue_id,
    u.name as user_name,
    u.idcode,
    i.full_name,
    i.contact,
    b.book_title,
    b.author,
    i.issue_date,
    i.return_date,
    DATEDIFF(day, i.return_date, GETDATE()) as days_overdue
FROM issues i
INNER JOIN users u ON i.user_id = u.id
INNER JOIN books b ON i.book_id = b.id
WHERE i.is_active = 1 AND i.date_delete IS NULL 
    AND i.status = 'Not Return' 
    AND i.return_date < GETDATE();
GO

-- =============================================
-- Create Stored Procedures
-- =============================================

-- Procedure to Authenticate User
CREATE PROCEDURE sp_AuthenticateUser
    @username NVARCHAR(50),
    @password NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        id,
        name,
        idcode,
        username,
        role,
        date_register
    FROM users 
    WHERE username = @username 
        AND password = @password 
        AND is_active = 1 
        AND date_delete IS NULL;
END;
GO

-- Procedure to Get User Dashboard Data
CREATE PROCEDURE sp_GetDashboardData
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Total Users
    SELECT COUNT(*) as total_users FROM users WHERE is_active = 1 AND date_delete IS NULL;
    
    -- Total Books
    SELECT COUNT(*) as total_books FROM books WHERE is_active = 1 AND date_delete IS NULL;
    
    -- Available Books
    SELECT COUNT(*) as available_books FROM books WHERE is_active = 1 AND date_delete IS NULL AND status = 'Available';
    
    -- Issued Books
    SELECT COUNT(*) as issued_books FROM issues WHERE is_active = 1 AND date_delete IS NULL AND status = 'Not Return';
    
    -- Overdue Books
    SELECT COUNT(*) as overdue_books FROM issues 
    WHERE is_active = 1 AND date_delete IS NULL 
        AND status = 'Not Return' 
        AND return_date < GETDATE();
END;
GO

-- Procedure to Issue a Book
CREATE PROCEDURE sp_IssueBook
    @user_id INT,
    @book_id INT,
    @full_name NVARCHAR(100),
    @contact NVARCHAR(20),
    @issue_date DATE,
    @return_date DATE,
    @issue_id NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @book_title NVARCHAR(200);
    DECLARE @author NVARCHAR(100);
    DECLARE @current_quantity INT;
    
    -- Get book details
    SELECT @book_title = book_title, @author = author, @current_quantity = quantity
    FROM books 
    WHERE id = @book_id AND is_active = 1 AND date_delete IS NULL;
    
    -- Check if book is available
    IF @current_quantity <= 0
    BEGIN
        RAISERROR('Book is not available for issue', 16, 1);
        RETURN;
    END
    
    -- Generate issue ID
    SET @issue_id = 'ISS' + FORMAT(GETDATE(), 'yyyyMMdd') + RIGHT('000' + CAST(IDENT_CURRENT('issues') + 1 AS VARCHAR), 3);
    
    -- Insert issue record
    INSERT INTO issues (issue_id, user_id, book_id, full_name, contact, book_title, author, issue_date, return_date)
    VALUES (@issue_id, @user_id, @book_id, @full_name, @contact, @book_title, @author, @issue_date, @return_date);
    
    -- Update book quantity
    UPDATE books 
    SET quantity = quantity - 1,
        status = CASE WHEN quantity - 1 <= 0 THEN 'Issued' ELSE 'Available' END
    WHERE id = @book_id;
END;
GO

-- Procedure to Return a Book
CREATE PROCEDURE sp_ReturnBook
    @issue_id NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @book_id INT;
    DECLARE @user_id INT;
    
    -- Get book and user IDs
    SELECT @book_id = book_id, @user_id = user_id
    FROM issues 
    WHERE issue_id = @issue_id AND is_active = 1 AND date_delete IS NULL;
    
    IF @book_id IS NULL
    BEGIN
        RAISERROR('Issue record not found', 16, 1);
        RETURN;
    END
    
    -- Update issue status
    UPDATE issues 
    SET status = 'Return',
        actual_return_date = GETDATE(),
        date_update = GETDATE()
    WHERE issue_id = @issue_id;
    
    -- Update book quantity and status
    UPDATE books 
    SET quantity = quantity + 1,
        status = 'Available',
        date_update = GETDATE()
    WHERE id = @book_id;
END;
GO

-- =============================================
-- Create Triggers
-- =============================================

-- Trigger to automatically update date_update when records are modified
CREATE TRIGGER tr_users_update
ON users
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE users 
    SET date_update = GETDATE()
    FROM users u
    INNER JOIN inserted i ON u.id = i.id;
END;
GO

CREATE TRIGGER tr_books_update
ON books
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE books 
    SET date_update = GETDATE()
    FROM books b
    INNER JOIN inserted i ON b.id = i.id;
END;
GO

CREATE TRIGGER tr_issues_update
ON issues
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE issues 
    SET date_update = GETDATE()
    FROM issues i
    INNER JOIN inserted ins ON i.id = ins.id;
END;
GO

-- =============================================
-- Sample Data Insertion
-- =============================================

-- Insert sample books
INSERT INTO books (book_title, author, published_date, quantity, status, image)
VALUES 
    ('Introduction to Programming', 'John Smith', '2020-01-15', 5, 'Available', NULL),
    ('Database Design', 'Jane Doe', '2019-06-20', 3, 'Available', NULL),
    ('Web Development', 'Mike Johnson', '2021-03-10', 4, 'Available', NULL),
    ('Data Structures', 'Sarah Wilson', '2018-09-05', 2, 'Available', NULL),
    ('Software Engineering', 'David Brown', '2020-11-12', 3, 'Available', NULL);
GO

-- Insert sample students
INSERT INTO users (name, idcode, username, password, role, date_register)
VALUES 
    ('Alice Johnson', 'STU001', 'alice.johnson', 'password123', 'student', GETDATE()),
    ('Bob Smith', 'STU002', 'bob.smith', 'password123', 'student', GETDATE()),
    ('Carol Davis', 'STU003', 'carol.davis', 'password123', 'student', GETDATE()),
    ('David Wilson', 'STU004', 'david.wilson', 'password123', 'student', GETDATE());
GO

-- =============================================
-- Security and Permissions
-- =============================================

-- Create application user (if not exists)
-- Note: This should be run by a database administrator
/*
CREATE LOGIN app_user WITH PASSWORD = 'StrongP@ssw0rd!';
CREATE USER app_user FOR LOGIN app_user;
*/

-- Grant permissions to application user
-- GRANT SELECT, INSERT, UPDATE, DELETE ON users TO app_user;
-- GRANT SELECT, INSERT, UPDATE, DELETE ON books TO app_user;
-- GRANT SELECT, INSERT, UPDATE, DELETE ON issues TO app_user;
-- GRANT EXECUTE ON sp_AuthenticateUser TO app_user;
-- GRANT EXECUTE ON sp_GetDashboardData TO app_user;
-- GRANT EXECUTE ON sp_IssueBook TO app_user;
-- GRANT EXECUTE ON sp_ReturnBook TO app_user;

-- =============================================
-- Database Schema Complete
-- =============================================

PRINT 'Library Management System Database Schema Created Successfully!';
PRINT 'Default admin user created: username=admin, password=admin123';
PRINT 'Sample data inserted for testing';
