CREATE PROCEDURE sp_UpdateJobOpportunity  
(  
    @Id INT,  
    @UserId INT,  
    @Role NVARCHAR(50),  

    @Title NVARCHAR(200),  
    @Description NVARCHAR(MAX),  
    @Type NVARCHAR(100),  
    @Experience NVARCHAR(100),  
    @Salary NVARCHAR(100),  
    @Department NVARCHAR(100),  
    @Category NVARCHAR(100),  

    @CompanyName NVARCHAR(200),  
    @IsActive BIT,  

    @Image NVARCHAR(MAX),  
    @Duration NVARCHAR(100),  
    @Location NVARCHAR(200),  

    @SkillsJson NVARCHAR(MAX),  
    @ResponsibilitiesJson NVARCHAR(MAX)  
)  
AS  
BEGIN  
    -- SuperAdmin can update any job
    IF @Role = 'SuperAdmin'
    BEGIN
        UPDATE JobOpportunities  
        SET  
            Title = @Title,  
            Description = @Description,  
            Type = @Type,  
            Experience = @Experience,  
            Salary = @Salary,  
            Department = @Department,  
            Category = @Category,  
            CompanyName = @CompanyName,  
            IsActive = @IsActive,  
            Image = @Image,  
            Duration = @Duration,  
            Location = @Location,  
            SkillsJson = @SkillsJson,  
            ResponsibilitiesJson = @ResponsibilitiesJson  
        WHERE Id = @Id;  
    END
    ELSE
    BEGIN
        -- Admin can update ONLY his own job
        UPDATE JobOpportunities  
        SET  
            Title = @Title,  
            Description = @Description,  
            Type = @Type,  
            Experience = @Experience,  
            Salary = @Salary,  
            Department = @Department,  
            Category = @Category,  
            CompanyName = @CompanyName,  
            IsActive = @IsActive,  
            Image = @Image,  
            Duration = @Duration,  
            Location = @Location,  
            SkillsJson = @SkillsJson,  
            ResponsibilitiesJson = @ResponsibilitiesJson  
        WHERE Id = @Id  
          AND CreatedByUserId = @UserId;  

        -- If no row updated → unauthorized
        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50001, 'Unauthorized job update', 1;
        END
    END
END;
