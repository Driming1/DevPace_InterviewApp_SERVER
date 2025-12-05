DROP TABLE IF EXISTS [Customers];

GO

CREATE TABLE [Customers]
(
    [Id] BIGINT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
    [Name] NVARCHAR(200) NOT NULL,
    [Email] NVARCHAR(255),
    [Phone] NVARCHAR(100) NULL,
    [CreateDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

GO

DROP PROCEDURE IF EXISTS [SearchCustomers]

GO

CREATE PROCEDURE [SearchCustomers]
    @name NVARCHAR(MAX),
    @email NVARCHAR(MAX),
    @phone NVARCHAR(MAX),
    @skip INT,
    @take INT
AS
BEGIN
    SELECT _customer.[Id],
           _customer.[Name], 
           _customer.[Email],
           _customer.[Phone],
           _customer.[CreateDate] 
    FROM [Customers] _customer
    ORDER BY _customer.[Name] DESC
    OFFSET @skip ROWS
	FETCH NEXT @take ROWS ONLY
END

GO

TRUNCATE TABLE [Customers];
GO

ALTER TABLE [Customers]
ADD [ActiveState] TINYINT NOT NULL DEFAULT(1);
GO

ALTER TABLE [Customers] 
ALTER COLUMN [Email] NVARCHAR(255) NOT NULL;
GO

;WITH Numbers AS
(
    SELECT TOP (2000000)
        ROW_NUMBER() OVER (ORDER BY a.object_id) AS n
    FROM sys.all_objects a
    CROSS JOIN sys.all_objects b
)
INSERT INTO [Customers]
(
    [Name],
    [Email],
    [Phone],
    [ActiveState],
    [CreateDate]
)
SELECT
    [Name] =
        (CASE n % 10
             WHEN 0 THEN N'Michael'
             WHEN 1 THEN N'Olena'
             WHEN 2 THEN N'Andriy'
             WHEN 3 THEN N'Sophia'
             WHEN 4 THEN N'Ivan'
             WHEN 5 THEN N'Natalia'
             WHEN 6 THEN N'Dmytro'
             WHEN 7 THEN N'Anna'
             WHEN 8 THEN N'Volodymyr'
             WHEN 9 THEN N'Kateryna'
         END)
        + N' ' +
        (CASE (n / 10) % 10
             WHEN 0 THEN N'Shevchenko'
             WHEN 1 THEN N'Kovalenko'
             WHEN 2 THEN N'Sokolov'
             WHEN 3 THEN N'Melnyk'
             WHEN 4 THEN N'Petrenko'
             WHEN 5 THEN N'Romanov'
             WHEN 6 THEN N'Brown'
             WHEN 7 THEN N'Johnson'
             WHEN 8 THEN N'Smith'
             WHEN 9 THEN N'Garcia'
         END),
    [Email] =
        LOWER(
            (CASE n % 10
                 WHEN 0 THEN N'michael'
                 WHEN 1 THEN N'olena'
                 WHEN 2 THEN N'andriy'
                 WHEN 3 THEN N'sophia'
                 WHEN 4 THEN N'ivan'
                 WHEN 5 THEN N'natalia'
                 WHEN 6 THEN N'dmytro'
                 WHEN 7 THEN N'anna'
                 WHEN 8 THEN N'volodymyr'
                 WHEN 9 THEN N'kateryna'
             END)
            + N'.' +
            (CASE (n / 10) % 10
                 WHEN 0 THEN N'shevchenko'
                 WHEN 1 THEN N'kovalenko'
                 WHEN 2 THEN N'sokolov'
                 WHEN 3 THEN N'melnyk'
                 WHEN 4 THEN N'petrenko'
                 WHEN 5 THEN N'romanov'
                 WHEN 6 THEN N'brown'
                 WHEN 7 THEN N'johnson'
                 WHEN 8 THEN N'smith'
                 WHEN 9 THEN N'garcia'
             END)
        )
        + CAST(n AS NVARCHAR(10))
        + N'@example.com',
    [Phone]       = N'+380' + RIGHT('000000000' + CAST(n AS NVARCHAR(9)), 9),
    [ActiveState] = CASE WHEN n % 2 = 0 THEN 1 ELSE 0 END,
    [CreateDate]  = DATEADD(DAY, - (n % 5), GETUTCDATE())
FROM Numbers;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'UX_Customers_Email' AND object_id = OBJECT_ID('dbo.Customers')
)
BEGIN
    CREATE UNIQUE INDEX UX_Customers_Email
        ON [Customers]([Email]);
END
GO


IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_Customers_Search' 
      AND object_id = OBJECT_ID('dbo.Customers')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Customers_Search
        ON [Customers] (ActiveState, Name,Email, Phone);
        
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_Customers_ActiveState_CreateDate' 
      AND object_id = OBJECT_ID('dbo.Customers')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Customers_ActiveState_CreateDate
        ON [Customers] (ActiveState, Id);
END
GO

DROP PROCEDURE IF EXISTS [SearchCustomers];
GO

CREATE PROCEDURE [SearchCustomers]
    @name         NVARCHAR(200) = NULL,        
    @email        NVARCHAR(255) = NULL,        
    @phone        NVARCHAR(100) = NULL,        
    @activeState  TINYINT       = 1,           
    @sortColumn   NVARCHAR(50)  = N'CreateDate', 
    @sortDirection NVARCHAR(4)  = N'DESC',   
    @skip         INT           = 0,
    @take         INT           = 50
AS
BEGIN
    SET NOCOUNT ON;

    IF (@skip < 0) SET @skip = 0;
    IF (@take <= 0) SET @take = 50;

    ;WITH Filtered AS
    (
        SELECT
            c.[Id],
            c.[Name],
            c.[Email],
            c.[Phone],
            c.[ActiveState],
            c.[CreateDate]
        FROM [Customers] c
        WHERE
            (@name  IS NULL OR c.[Name]  LIKE @name  + N'%')
        AND (@email IS NULL OR c.[Email] LIKE @email + N'%')
        AND (@phone IS NULL OR c.[Phone] LIKE @phone + N'%')
        AND (@activeState IS NULL OR c.[ActiveState] = @activeState)
    )
    SELECT
        [Id],
        [Name],
        [Email],
        [Phone],
        [ActiveState],
        [CreateDate]
    FROM Filtered
    ORDER BY
        CASE WHEN @sortColumn = N'Name'        AND @sortDirection = N'ASC'  THEN [Name]        END ASC,
        CASE WHEN @sortColumn = N'Name'        AND @sortDirection = N'DESC' THEN [Name]        END DESC,
        CASE WHEN @sortColumn = N'Email'       AND @sortDirection = N'ASC'  THEN [Email]       END ASC,
        CASE WHEN @sortColumn = N'Email'       AND @sortDirection = N'DESC' THEN [Email]       END DESC,
        CASE WHEN @sortColumn = N'CreateDate'  AND @sortDirection = N'ASC'  THEN [CreateDate]  END ASC,
        CASE WHEN @sortColumn = N'CreateDate'  AND @sortDirection = N'DESC' THEN [CreateDate]  END DESC,
        CASE WHEN @sortColumn = N'ActiveState' AND @sortDirection = N'ASC'  THEN [ActiveState] END ASC,
        CASE WHEN @sortColumn = N'ActiveState' AND @sortDirection = N'DESC' THEN [ActiveState] END DESC,
        [Id] ASC
    OFFSET @skip ROWS
    FETCH NEXT @take ROWS ONLY
    OPTION (RECOMPILE);
END
GO
