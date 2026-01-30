CREATE TABLE [dbo].[Users] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [Email]                    NVARCHAR (255) NOT NULL,
    [PasswordHash]             NVARCHAR (MAX) NOT NULL,
    [FullName]                 NVARCHAR (MAX) NOT NULL,
    [Role]                     NVARCHAR (MAX) NOT NULL,
    [IsActive]                 BIT            NOT NULL,
    [CreatedDate]              DATETIME2 (7)  NOT NULL,
    [IsEmailVerified]          BIT            NOT NULL,
    [EmailVerificationToken]   NVARCHAR (MAX) NULL,
    [PasswordResetToken]       NVARCHAR (200) NULL,
    [PasswordResetTokenExpiry] DATETIME       NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Users_Email] UNIQUE NONCLUSTERED ([Email] ASC)
);

