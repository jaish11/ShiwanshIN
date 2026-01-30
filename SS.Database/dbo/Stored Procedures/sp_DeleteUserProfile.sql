CREATE PROCEDURE sp_DeleteUserProfile
    @Id INT
AS
BEGIN
    DELETE FROM UserProfiles
    WHERE Id = @Id;
END
