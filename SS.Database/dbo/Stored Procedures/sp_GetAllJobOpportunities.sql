CREATE PROCEDURE sp_GetAllJobOpportunities
AS
BEGIN
    SELECT 
        Id, Title, Description, Type, Experience, Salary,
        Department, Category, Image, Duration, Location,
        SkillsJson, ResponsibilitiesJson, CreatedDate
    FROM JobOpportunities
    ORDER BY CreatedDate DESC;
END;
