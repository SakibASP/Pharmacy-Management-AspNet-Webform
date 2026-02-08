CREATE DATABASE PharmacyDB;
GO

ALTER DATABASE PharmacyDB SET RECOVERY SIMPLE;

GO

USE PharmacyDB;
GO

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(256) NOT NULL
);
GO

CREATE TABLE Medicine (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    GenericName NVARCHAR(150),
    Category NVARCHAR(100),
    BatchNo NVARCHAR(50) NOT NULL,
    ExpiryDate DATE NOT NULL,
    Quantity INT NOT NULL DEFAULT 0,
    UnitPrice DECIMAL(18,2) NOT NULL DEFAULT 0
);
GO

CREATE TABLE SalesMaster (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceNumber NVARCHAR(20) NOT NULL UNIQUE,
    InvoiceDate DATE NOT NULL,
    CustomerName NVARCHAR(150) NOT NULL,
    CustomerContact NVARCHAR(50),
    SubTotal DECIMAL(18,2) NOT NULL DEFAULT 0,
    Discount DECIMAL(18,2) NOT NULL DEFAULT 0,
    GrandTotal DECIMAL(18,2) NOT NULL DEFAULT 0,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE SalesDetail (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceId INT NOT NULL,
    MedicineId INT NOT NULL,
    BatchNo NVARCHAR(50),
    ExpiryDate DATE,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    LineTotal DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (InvoiceId) REFERENCES SalesMaster(Id),
    FOREIGN KEY (MedicineId) REFERENCES Medicine(Id)
);
GO

INSERT INTO Users (Username, Password) VALUES ('admin', 'admin123');
GO

GO

CREATE OR ALTER PROCEDURE dbo.usp_ValidateUser
    @Username NVARCHAR(50),
    @Password NVARCHAR(256)
AS
BEGIN
    SELECT Id, Username FROM Users
    WHERE Username = @Username AND Password = @Password;
END
GO


CREATE OR ALTER PROCEDURE dbo.usp_MedicineOperations
    @OperationId INT,
    @Id INT = NULL,
    @Name NVARCHAR(150) = NULL,
    @GenericName NVARCHAR(150) = NULL,
    @Category NVARCHAR(100) = NULL,
    @BatchNo NVARCHAR(50) = NULL,
    @ExpiryDate DATE = NULL,
    @Quantity INT = NULL,
    @UnitPrice DECIMAL(18,2) = NULL
AS
BEGIN
    IF @OperationId = 1 -- Get all medicines
    BEGIN
        SELECT Id, Name, GenericName, Category, BatchNo, ExpiryDate, Quantity, UnitPrice
        FROM Medicine
        ORDER BY Name;
    END
    IF @OperationId = 2 -- Get medicine by Id
    BEGIN
        SELECT Id, Name, GenericName, Category, BatchNo, ExpiryDate, Quantity, UnitPrice
        FROM Medicine
        WHERE Id = @Id;
    END
    IF @OperationId = 3 -- Add new medicine
    BEGIN
        INSERT INTO Medicine (Name, GenericName, Category, BatchNo, ExpiryDate, Quantity, UnitPrice)
        VALUES (@Name, @GenericName, @Category, @BatchNo, @ExpiryDate, @Quantity, @UnitPrice);
    END
    IF @OperationId = 4 -- Update existing medicine
    BEGIN
        UPDATE Medicine
        SET Name = @Name, GenericName = @GenericName, Category = @Category,
            BatchNo = @BatchNo, ExpiryDate = @ExpiryDate, Quantity = @Quantity, UnitPrice = @UnitPrice
        WHERE Id = @Id;
    END
    IF @OperationId = 5 -- Delete medicine
    BEGIN
        DELETE FROM Medicine WHERE Id = @Id;
    END
    IF @OperationId = 6 -- Get medicines for dropdown
    BEGIN
        SELECT Id, Name, BatchNo, ExpiryDate, Quantity, UnitPrice
        FROM Medicine
        WHERE Quantity > 0
        ORDER BY Name;
    END
    IF @OperationId = 7 -- Check medicine stock
    BEGIN
        SELECT Quantity FROM Medicine WHERE Id = @Id;
    END
END


GO

CREATE OR ALTER PROCEDURE dbo.usp_GetNextInvoiceNumber
AS
BEGIN
    DECLARE @LastNumber INT;
    SELECT @LastNumber = ISNULL(MAX(CAST(REPLACE(InvoiceNumber, 'INV-', '') AS INT)), 0)
    FROM SalesMaster;
    SELECT 'INV-' + RIGHT('00000' + CAST(@LastNumber + 1 AS VARCHAR(5)), 5) AS NextInvoiceNumber;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_InsertSalesMaster
    @InvoiceNumber NVARCHAR(20),
    @InvoiceDate DATE,
    @CustomerName NVARCHAR(150),
    @CustomerContact NVARCHAR(50),
    @SubTotal DECIMAL(18,2),
    @Discount DECIMAL(18,2),
    @GrandTotal DECIMAL(18,2),
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO SalesMaster (InvoiceNumber, InvoiceDate, CustomerName, CustomerContact, SubTotal, Discount, GrandTotal)
    VALUES (@InvoiceNumber, @InvoiceDate, @CustomerName, @CustomerContact, @SubTotal, @Discount, @GrandTotal);
    SET @Id = SCOPE_IDENTITY();
END

GO


CREATE OR ALTER PROCEDURE dbo.usp_InsertSalesDetail
    @InvoiceId INT,
    @MedicineId INT,
    @BatchNo NVARCHAR(50),
    @ExpiryDate DATE,
    @Quantity INT,
    @UnitPrice DECIMAL(18,2),
    @LineTotal DECIMAL(18,2)
AS
BEGIN
    INSERT INTO SalesDetail (InvoiceId, MedicineId, BatchNo, ExpiryDate, Quantity, UnitPrice, LineTotal)
    VALUES (@InvoiceId, @MedicineId, @BatchNo, @ExpiryDate, @Quantity, @UnitPrice, @LineTotal);

    UPDATE Medicine SET Quantity = Quantity - @Quantity WHERE Id = @MedicineId;
END
GO


CREATE OR ALTER PROCEDURE usp_GetSaleById
    @Id INT
AS
BEGIN
    SELECT Id, InvoiceNumber, InvoiceDate, CustomerName, CustomerContact, SubTotal, Discount, GrandTotal
    FROM SalesMaster
    WHERE Id = @Id;

    SELECT sd.Id, sd.MedicineId, m.Name AS MedicineName, sd.BatchNo, sd.ExpiryDate, sd.Quantity, sd.UnitPrice, sd.LineTotal
    FROM SalesDetail sd
    INNER JOIN Medicine m ON sd.MedicineId = m.Id
    WHERE sd.InvoiceId = @Id;
END
GO

CREATE OR ALTER PROCEDURE usp_GetAllSales
AS
BEGIN
    SELECT Id, InvoiceNumber, InvoiceDate, CustomerName, CustomerContact, SubTotal, Discount, GrandTotal, CreatedDate
    FROM SalesMaster
    ORDER BY CreatedDate DESC;
END