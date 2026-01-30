CREATE PROCEDURE sp_GetJobOpportunityById  
(  
    @Id INT,  
    @UserId INT,  
    @Role NVARCHAR(50)  
)  
AS  
BEGIN  
    IF @Role = 'SuperAdmin'
    BEGIN
        SELECT   
            Id, Title, Description, Type, Experience, Salary,  
            Department, Category, CompanyName, IsActive,
            Image, Duration, Location,  
            SkillsJson, ResponsibilitiesJson, CreatedDate  
        FROM JobOpportunities  
        WHERE Id = @Id;  
    END
    ELSE
    BEGIN
        SELECT   
            Id, Title, Description, Type, Experience, Salary,  
            Department, Category, CompanyName, IsActive,
            Image, Duration, Location,  
            SkillsJson, ResponsibilitiesJson, CreatedDate  
        FROM JobOpportunities  
        WHERE Id = @Id  
          AND CreatedByUserId = @UserId;  
    END
END;
