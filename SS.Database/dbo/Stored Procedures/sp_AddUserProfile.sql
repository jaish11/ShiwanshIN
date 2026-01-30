CREATE PROCEDURE sp_AddUserProfile
    @UserId INT,
    @FullName NVARCHAR(200),
    @Email NVARCHAR(200),
    @Phone NVARCHAR(50),
    @Bio NVARCHAR(500) = NULL,
    @Description NVARCHAR(MAX) = NULL,
    @IsActive BIT = 1,
    @LinkedIn NVARCHAR(500) = NULL,
    @GitHub NVARCHAR(500) = NULL,
    @LeetCode NVARCHAR(500) = NULL,
    @University NVARCHAR(200) = NULL,
    @Degree NVARCHAR(200) = NULL,
    @Department NVARCHAR(200) = NULL,
    @GraduationYear INT = NULL,
    @GPA NVARCHAR(50) = NULL,
    @CompanyName NVARCHAR(200) = NULL,
    @Position NVARCHAR(200) = NULL,
    @ExperienceFrom DATETIME = NULL,
    @ExperienceTo DATETIME = NULL,
    @TotalExperience NVARCHAR(50) = NULL,
    @NoticePeriod NVARCHAR(50) = NULL,
    @ProfileImage NVARCHAR(500) = NULL,
    @ResumeFile NVARCHAR(500) = NULL,
    @ResumeUrl NVARCHAR(500) = NULL,
    @CreatedAt DATETIME = NULL
AS
BEGIN
    INSERT INTO UserProfiles
    (UserId, FullName, Email, Phone, Bio, Description, IsActive, LinkedIn, GitHub, LeetCode,
     University, Degree, Department, GraduationYear, GPA,
     CompanyName, Position, ExperienceFrom, ExperienceTo, TotalExperience, NoticePeriod,
     ProfileImage, ResumeFile, ResumeUrl, CreatedAt)
    VALUES
    (@UserId, @FullName, @Email, @Phone, @Bio, @Description, @IsActive, @LinkedIn, @GitHub, @LeetCode,
     @University, @Degree, @Department, @GraduationYear, @GPA,
     @CompanyName, @Position, @ExperienceFrom, @ExperienceTo, @TotalExperience, @NoticePeriod,
     @ProfileImage, @ResumeFile, @ResumeUrl, ISNULL(@CreatedAt, GETUTCDATE()));
END
