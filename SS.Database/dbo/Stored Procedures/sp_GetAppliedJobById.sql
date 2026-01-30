CREATE PROCEDURE sp_GetAppliedJobById
(
    @Id INT
)
AS
BEGIN
    SELECT * FROM ApplyJobs WHERE Id = @Id;
END;