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
