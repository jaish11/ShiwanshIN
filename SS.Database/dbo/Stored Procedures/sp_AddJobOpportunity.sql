CREATE PROCEDURE sp_AddJobOpportunity
(
    @Title NVARCHAR(200),
    @Description NVARCHAR(MAX),
    @Type NVARCHAR(100),
    @Experience NVARCHAR(100),
    @Salary NVARCHAR(100),
    @Department NVARCHAR(100),
    @Category NVARCHAR(100),
    @Image NVARCHAR(MAX),
    @Duration NVARCHAR(100),
    @Location NVARCHAR(200),
    @CompanyName NVARCHAR(200),
    @CreatedByUserId INT,
    @IsActive BIT,
    @SkillsJson NVARCHAR(MAX),
    @ResponsibilitiesJson NVARCHAR(MAX)
)
AS
BEGIN
    INSERT INTO JobOpportunities
    (
        Title, Description, Type, Experience, Salary,
        Department, Category, Image, Duration, Location,
        CompanyName, CreatedByUserId, IsActive,
        SkillsJson, ResponsibilitiesJson, CreatedDate
    )
    VALUES
    (
        @Title, @Description, @Type, @Experience, @Salary,
        @Department, @Category, @Image, @Duration, @Location,
        @CompanyName, @CreatedByUserId, 1,
        @SkillsJson, @ResponsibilitiesJson, GETDATE()
    )
END
