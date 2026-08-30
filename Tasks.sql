CREATE TABLE [dbo].[Tasks] (
    [TaskId]      INT           IDENTITY (1, 1) NOT NULL,
    [UserId]      INT           NOT NULL ,
    [Title]       VARCHAR (50)  NOT NULL,
    [Description] VARCHAR (MAX) NOT NULL,
    [Duedate]     DATE          NOT NULL,
    [Priority]    VARCHAR (50)  NOT NULL,
    [Category]    VARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([TaskId] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserID]) ON DELETE CASCADE 
	
);

