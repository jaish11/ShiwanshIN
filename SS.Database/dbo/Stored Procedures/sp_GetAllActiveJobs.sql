CREATE PROCEDURE sp_GetAllActiveJobs
AS
BEGIN
    SELECT 
        Id,
        Title,
        Description,
        Type,
        Experience,
        Salary,
        Department,
        Category,
        CompanyName,
        IsActive, 
        Image,
        Duration,
        Location,
        SkillsJson,
        ResponsibilitiesJson,
        CreatedDate
    FROM JobOpportunities
    WHERE IsActive = 1
    ORDER BY CreatedDate DESC;
END
