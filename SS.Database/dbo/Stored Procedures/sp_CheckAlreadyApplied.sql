CREATE PROCEDURE sp_CheckAlreadyApplied
(
    @UserId INT,
    @JobId INT
)
AS
BEGIN
    SELECT COUNT(1)
    FROM ApplyJobs
    WHERE UserId = @UserId AND JobId = @JobId
END;