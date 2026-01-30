CREATE PROCEDURE sp_DeleteAppliedJob
(
    @Id INT
)
AS
BEGIN
    DELETE FROM ApplyJobs WHERE Id = @Id;
END;