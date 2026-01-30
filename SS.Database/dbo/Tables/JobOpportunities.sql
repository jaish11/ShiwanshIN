CREATE TABLE [dbo].[JobOpportunities] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [Title]                NVARCHAR (MAX) NOT NULL,
    [Description]          NVARCHAR (MAX) NOT NULL,
    [Type]                 NVARCHAR (MAX) NOT NULL,
    [Experience]           NVARCHAR (MAX) NOT NULL,
    [Salary]               NVARCHAR (MAX) NOT NULL,
    [Department]           NVARCHAR (MAX) NOT NULL,
    [Category]             NVARCHAR (MAX) NOT NULL,
    [Image]                NVARCHAR (MAX) NOT NULL,
    [Duration]             NVARCHAR (MAX) NULL,
    [Location]             NVARCHAR (MAX) NOT NULL,
    [CreatedDate]          DATETIME2 (7)  NOT NULL,
    [SkillsJson]           NVARCHAR (MAX) NULL,
    [ResponsibilitiesJson] NVARCHAR (MAX) NULL,
    [CreatedByUserId]      INT            NOT NULL,
    [CompanyName]          NVARCHAR (200) NOT NULL,
    [IsActive]             BIT            DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_JobOpportunities] PRIMARY KEY CLUSTERED ([Id] ASC)
);

