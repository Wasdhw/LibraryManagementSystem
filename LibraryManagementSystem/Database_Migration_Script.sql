-- Library Management System - Database Migration Script
-- This script creates all required tables for the enhanced features

-- ============================================
-- 1. FINES TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fines]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[fines](
        [id] [int] IDENTITY(1,1) NOT NULL,
        [issue_id] [nvarchar](50) NOT NULL,
        [user_id] [int] NOT NULL,
        [amount] [decimal](10, 2) NOT NULL,
        [days_overdue] [int] NOT NULL,
        [status] [nvarchar](20) NOT NULL DEFAULT 'Unpaid',
        [date_created] [date] NOT NULL,
        [date_paid] [date] NULL,
        [date_delete] [date] NULL,
        CONSTRAINT [PK_fines] PRIMARY KEY CLUSTERED ([id] ASC),
        CONSTRAINT [FK_fines_issues] FOREIGN KEY([issue_id]) REFERENCES [dbo].[issues] ([issue_id]),
        CONSTRAINT [FK_fines_users] FOREIGN KEY([user_id]) REFERENCES [dbo].[users] ([id])
    )
    PRINT 'Table [fines] created successfully.'
END
ELSE
BEGIN
    PRINT 'Table [fines] already exists.'
END
GO

-- ============================================
-- 2. RESERVATIONS TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[reservations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[reservations](
        [id] [int] IDENTITY(1,1) NOT NULL,
        [user_id] [int] NOT NULL,
        [book_id] [int] NOT NULL,
        [status] [nvarchar](20) NOT NULL DEFAULT 'Pending',
        [date_reserved] [date] NOT NULL,
        [date_notified] [date] NULL,
        [date_expires] [date] NOT NULL,
        [date_delete] [date] NULL,
        CONSTRAINT [PK_reservations] PRIMARY KEY CLUSTERED ([id] ASC),
        CONSTRAINT [FK_reservations_users] FOREIGN KEY([user_id]) REFERENCES [dbo].[users] ([id]),
        CONSTRAINT [FK_reservations_books] FOREIGN KEY([book_id]) REFERENCES [dbo].[books] ([id])
    )
    PRINT 'Table [reservations] created successfully.'
END
ELSE
BEGIN
    PRINT 'Table [reservations] already exists.'
END
GO

-- ============================================
-- 3. SETTINGS TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[settings]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[settings](
        [id] [int] IDENTITY(1,1) NOT NULL,
        [key] [nvarchar](100) NOT NULL,
        [value] [nvarchar](500) NOT NULL,
        [description] [nvarchar](500) NULL,
        [date_insert] [date] NOT NULL DEFAULT GETDATE(),
        [date_update] [date] NULL,
        CONSTRAINT [PK_settings] PRIMARY KEY CLUSTERED ([id] ASC),
        CONSTRAINT [UQ_settings_key] UNIQUE ([key])
    )
    
    -- Insert default settings
    INSERT INTO [dbo].[settings] ([key], [value], [description]) VALUES
    ('borrowing_period_days', '14', 'Number of days a book can be borrowed'),
    ('max_books_per_user', '5', 'Maximum number of books a user can borrow at once'),
    ('overdue_threshold_days', '3', 'Days after return date before book is considered overdue'),
    ('fine_rate_per_day', '5.00', 'Fine amount per day for overdue books'),
    ('max_renewals', '2', 'Maximum number of times a book can be renewed'),
    ('renewal_days', '14', 'Number of days added when a book is renewed')
    
    PRINT 'Table [settings] created successfully with default values.'
END
ELSE
BEGIN
    PRINT 'Table [settings] already exists.'
END
GO

-- ============================================
-- 4. AUDIT_LOGS TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[audit_logs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[audit_logs](
        [id] [int] IDENTITY(1,1) NOT NULL,
        [user_id] [int] NULL,
        [action] [nvarchar](100) NOT NULL,
        [entity_type] [nvarchar](50) NULL,
        [entity_id] [int] NULL,
        [details] [nvarchar](max) NULL,
        [timestamp] [datetime] NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_audit_logs] PRIMARY KEY CLUSTERED ([id] ASC),
        CONSTRAINT [FK_audit_logs_users] FOREIGN KEY([user_id]) REFERENCES [dbo].[users] ([id])
    )
    PRINT 'Table [audit_logs] created successfully.'
END
ELSE
BEGIN
    PRINT 'Table [audit_logs] already exists.'
END
GO

-- ============================================
-- 5. CATEGORIES TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[categories]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[categories](
        [id] [int] IDENTITY(1,1) NOT NULL,
        [name] [nvarchar](100) NOT NULL,
        [description] [nvarchar](500) NULL,
        [date_insert] [date] NOT NULL DEFAULT GETDATE(),
        [date_update] [date] NULL,
        [date_delete] [date] NULL,
        CONSTRAINT [PK_categories] PRIMARY KEY CLUSTERED ([id] ASC),
        CONSTRAINT [UQ_categories_name] UNIQUE ([name])
    )
    
    -- Insert default categories
    INSERT INTO [dbo].[categories] ([name], [description]) VALUES
    ('Fiction', 'Fictional literature'),
    ('Non-Fiction', 'Non-fictional works'),
    ('Science', 'Scientific and technical books'),
    ('History', 'Historical books'),
    ('Biography', 'Biographical works'),
    ('Reference', 'Reference materials')
    
    PRINT 'Table [categories] created successfully with default categories.'
END
ELSE
BEGIN
    PRINT 'Table [categories] already exists.'
END
GO

-- ============================================
-- 6. ADD COLUMNS TO EXISTING TABLES
-- ============================================

-- Add renewal_count to issues table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[issues]') AND name = 'renewal_count')
BEGIN
    ALTER TABLE [dbo].[issues]
    ADD [renewal_count] [int] NOT NULL DEFAULT 0
    PRINT 'Column [renewal_count] added to [issues] table.'
END
ELSE
BEGIN
    PRINT 'Column [renewal_count] already exists in [issues] table.'
END
GO

-- Add actual_return_date to issues table (if not exists)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[issues]') AND name = 'actual_return_date')
BEGIN
    ALTER TABLE [dbo].[issues]
    ADD [actual_return_date] [date] NULL
    PRINT 'Column [actual_return_date] added to [issues] table.'
END
ELSE
BEGIN
    PRINT 'Column [actual_return_date] already exists in [issues] table.'
END
GO

-- Add condition to issues table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[issues]') AND name = 'condition')
BEGIN
    ALTER TABLE [dbo].[issues]
    ADD [condition] [nvarchar](20) NULL
    PRINT 'Column [condition] added to [issues] table.'
END
ELSE
BEGIN
    PRINT 'Column [condition] already exists in [issues] table.'
END
GO

-- Add enhanced metadata to books table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[books]') AND name = 'isbn')
BEGIN
    ALTER TABLE [dbo].[books]
    ADD [isbn] [nvarchar](50) NULL,
        [publisher] [nvarchar](200) NULL,
        [edition] [nvarchar](50) NULL,
        [language] [nvarchar](50) NULL,
        [pages] [int] NULL,
        [category_id] [int] NULL
    PRINT 'Enhanced metadata columns added to [books] table.'
END
ELSE
BEGIN
    PRINT 'Enhanced metadata columns already exist in [books] table.'
END
GO

-- Add foreign key for category_id if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_books_categories')
BEGIN
    ALTER TABLE [dbo].[books]
    ADD CONSTRAINT [FK_books_categories] FOREIGN KEY([category_id]) REFERENCES [dbo].[categories] ([id])
    PRINT 'Foreign key [FK_books_categories] added.'
END
ELSE
BEGIN
    PRINT 'Foreign key [FK_books_categories] already exists.'
END
GO

-- ============================================
-- 7. CREATE INDEXES FOR PERFORMANCE
-- ============================================

-- Index on fines.issue_id
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_fines_issue_id')
BEGIN
    CREATE INDEX [IX_fines_issue_id] ON [dbo].[fines] ([issue_id])
    PRINT 'Index [IX_fines_issue_id] created.'
END
GO

-- Index on fines.user_id
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_fines_user_id')
BEGIN
    CREATE INDEX [IX_fines_user_id] ON [dbo].[fines] ([user_id])
    PRINT 'Index [IX_fines_user_id] created.'
END
GO

-- Index on reservations.book_id
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_reservations_book_id')
BEGIN
    CREATE INDEX [IX_reservations_book_id] ON [dbo].[reservations] ([book_id])
    PRINT 'Index [IX_reservations_book_id] created.'
END
GO

-- Index on reservations.user_id
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_reservations_user_id')
BEGIN
    CREATE INDEX [IX_reservations_user_id] ON [dbo].[reservations] ([user_id])
    PRINT 'Index [IX_reservations_user_id] created.'
END
GO

-- Index on audit_logs.user_id
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_audit_logs_user_id')
BEGIN
    CREATE INDEX [IX_audit_logs_user_id] ON [dbo].[audit_logs] ([user_id])
    PRINT 'Index [IX_audit_logs_user_id] created.'
END
GO

-- Index on audit_logs.timestamp
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_audit_logs_timestamp')
BEGIN
    CREATE INDEX [IX_audit_logs_timestamp] ON [dbo].[audit_logs] ([timestamp])
    PRINT 'Index [IX_audit_logs_timestamp] created.'
END
GO

PRINT '============================================'
PRINT 'Database migration completed successfully!'
PRINT '============================================'
GO

