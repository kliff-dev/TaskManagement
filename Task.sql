CREATE TABLE [dbo].[Task]
(  
	[TaskId] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [UserId] INT NOT NULL FOREIGN KEY REFERENCES Users([UserID]), 
    [Title] VARCHAR(50) NOT NULL, 
    [Description] VARCHAR(MAX) NOT NULL, 
    [Duedate] DATETIME NULL, 
    [Priority] VARCHAR(50) NOT NULL, 
	
    [Category] VARCHAR(MAX) NOT NULL
	 
)

