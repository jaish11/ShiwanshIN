CREATE PROCEDURE sp_DeleteJobOpportunity
(
    @Id INT,
    @UserId INT,
    @Role NVARCHAR(50)
)
AS
BEGIN
    IF @Role = 'SuperAdmin'
       OR EXISTS (
           SELECT 1 FROM JobOpportunities 
           WHERE Id = @Id AND CreatedByUserId = @UserId
       )
    BEGIN
        DELETE FROM JobOpportunities WHERE Id = @Id
    END
    ELSE
        THROW 50002, 'Unauthorized job delete', 1
END
