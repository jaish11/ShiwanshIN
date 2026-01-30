CREATE PROCEDURE sp_GetActiveJobById
(
    @Id INT
)
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
    WHERE Id = @Id
      AND IsActive = 1;
END
