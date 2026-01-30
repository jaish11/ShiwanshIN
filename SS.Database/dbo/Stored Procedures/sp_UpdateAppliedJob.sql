CREATE PROCEDURE sp_UpdateAppliedJob
(
    @Id INT,
    @JobId INT,
    @JobTitle NVARCHAR(200),
    @JobType NVARCHAR(100),

    @FullName NVARCHAR(200),
    @Email NVARCHAR(200),
    @Phone NVARCHAR(50),

    @ResumeUrl NVARCHAR(MAX),
    @ResumeFile NVARCHAR(MAX),

    @University NVARCHAR(250),
    @Degree NVARCHAR(200),
    @Department NVARCHAR(200),
    @GPA NVARCHAR(50),
    @GraduationYear INT,

    @CompanyName NVARCHAR(200),
    @Position NVARCHAR(200),
    @TotalExperience NVARCHAR(100),
    @NoticePeriod NVARCHAR(50),
    @ExperienceFrom DATETIME,
    @ExperienceTo DATETIME,

    @LinkedIn NVARCHAR(500),
    @GitHub NVARCHAR(500)
)
AS
BEGIN
    UPDATE ApplyJobs
    SET
        JobId = @JobId,
        JobTitle = @JobTitle,
        JobType = @JobType,
        FullName = @FullName,
        Email = @Email,
        Phone = @Phone,
        ResumeUrl = @ResumeUrl,
        ResumeFile = @ResumeFile,
        University = @University,
        Degree = @Degree,
        Department = @Department,
        GPA = @GPA,
        GraduationYear = @GraduationYear,
        CompanyName = @CompanyName,
        Position = @Position,
        TotalExperience = @TotalExperience,
        NoticePeriod = @NoticePeriod,
        ExperienceFrom = @ExperienceFrom,
        ExperienceTo = @ExperienceTo,
        LinkedIn = @LinkedIn,
        GitHub = @GitHub
    WHERE Id = @Id;
END;