SET NOCOUNT ON;
SET XACT_ABORT ON;

IF DB_ID(N'AccountingManagerDb') IS NULL
BEGIN
    CREATE DATABASE AccountingManagerDb;
END;
GO

USE AccountingManagerDb;
GO

IF OBJECT_ID(N'dbo.AccountingTypes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AccountingTypes
    (
        TypeId INT NOT NULL,
        TypeTitle NVARCHAR(150) NOT NULL,
        CONSTRAINT PK_AccountingTypes PRIMARY KEY (TypeId)
    );
END;
GO

IF OBJECT_ID(N'dbo.Customers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Customers
    (
        CustomerId INT IDENTITY(1, 1) NOT NULL,
        FullName NVARCHAR(300) NOT NULL,
        Mobile NVARCHAR(150) NOT NULL,
        Email NVARCHAR(150) NULL,
        Address NVARCHAR(800) NULL,
        CustomerImage VARCHAR(50) NOT NULL,
        CONSTRAINT PK_Customers PRIMARY KEY (CustomerId)
    );
END;
GO

IF OBJECT_ID(N'dbo.Accounting', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Accounting
    (
        Id INT IDENTITY(1, 1) NOT NULL,
        CustomerId INT NOT NULL,
        TypeId INT NOT NULL,
        Amount INT NOT NULL,
        Description NVARCHAR(800) NULL,
        DateTitle DATETIME NOT NULL,
        CONSTRAINT PK_Accounting PRIMARY KEY (Id),
        CONSTRAINT FK_Accounting_Customers FOREIGN KEY (CustomerId)
            REFERENCES dbo.Customers (CustomerId),
        CONSTRAINT FK_Accounting_AccountingTypes FOREIGN KEY (TypeId)
            REFERENCES dbo.AccountingTypes (TypeId),
        CONSTRAINT CK_Accounting_Amount_Positive CHECK (Amount > 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.Login', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Login
    (
        LoginId INT IDENTITY(1, 1) NOT NULL,
        UserName NVARCHAR(150) NOT NULL,
        Password NVARCHAR(150) NOT NULL,
        CONSTRAINT PK_Login PRIMARY KEY (LoginId),
        CONSTRAINT UQ_Login_UserName UNIQUE (UserName)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.AccountingTypes WHERE TypeId = 1)
    INSERT INTO dbo.AccountingTypes (TypeId, TypeTitle) VALUES (1, N'Receipt');

IF NOT EXISTS (SELECT 1 FROM dbo.AccountingTypes WHERE TypeId = 2)
    INSERT INTO dbo.AccountingTypes (TypeId, TypeTitle) VALUES (2, N'Payment');

IF NOT EXISTS (SELECT 1 FROM dbo.Login)
BEGIN
    INSERT INTO dbo.Login (UserName, Password)
    VALUES
    (
        N'admin',
        N'pbkdf2-sha256$100000$pLR9Gglu0W9WCaXhfgcVaQ==$dZP1ilsCFB8Kxmxm82Z8+Fwn4VYtcUYI9GsFMPva6fs='
    );
END;
GO
