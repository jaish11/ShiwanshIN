CREATE PROCEDURE sp_GetUserProfileByUserId
    @UserId INT
AS
BEGIN
    SELECT *
    FROM UserProfiles
    WHERE UserId = @UserId;
END
