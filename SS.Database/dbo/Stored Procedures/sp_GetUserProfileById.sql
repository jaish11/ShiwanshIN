CREATE PROCEDURE sp_GetUserProfileById
    @Id INT
AS
BEGIN
    SELECT *
    FROM UserProfiles
    WHERE Id = @Id;
END
