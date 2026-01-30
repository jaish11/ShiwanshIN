CREATE PROCEDURE sp_GetJobsByRole
(
    @UserId INT,
    @Role NVARCHAR(50)
)
AS
BEGIN
    IF (@Role = 'SuperAdmin')
    BEGIN
        SELECT * FROM JobOpportunities
        ORDER BY CreatedDate DESC;
    END
    ELSE
    BEGIN
        SELECT * FROM JobOpportunities
        WHERE CreatedByUserId = @UserId
        ORDER BY CreatedDate DESC;
    END
END;
