CREATE TABLE [dbo].[Users] (
    [Username] VARCHAR (50) NOT NULL,
    [Password] VARCHAR (50) NOT NULL, 
    [UserID] INT NOT NULL UNIQUE IDENTITY(1,1), 
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserID]) 
);

