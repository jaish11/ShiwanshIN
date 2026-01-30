CREATE PROCEDURE sp_GetAppliedJobsByUser
(
  @UserId INT
)
AS
BEGIN
  SELECT 
    Id,
    JobId,
    JobTitle,
    JobType,
    AppliedDate
  FROM ApplyJobs
  WHERE UserId = @UserId
  ORDER BY AppliedDate DESC
END
