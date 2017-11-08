CREATE TABLE [dbo].[Customers] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [FirstName] NVARCHAR (30)  NOT NULL,
    [LastName]  NVARCHAR (30)  NOT NULL,
    [Gender]    NVARCHAR (MAX) NOT NULL,
    [Birthday]  DATETIME       NOT NULL,
    [Email]     NVARCHAR (MAX) NOT NULL,
    [Phone]     NVARCHAR (MAX) NOT NULL,
    [Username]  NVARCHAR (MAX) NOT NULL,
    [Password]  NVARCHAR (MAX) NOT NULL,
    [IsAdmin]   BIT            NOT NULL,
    CONSTRAINT [PK_dbo.Customers] PRIMARY KEY CLUSTERED ([ID] ASC)
);

