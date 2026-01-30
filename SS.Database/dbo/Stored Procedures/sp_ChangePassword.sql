CREATE PROCEDURE sp_ChangePassword
    @UserId INT,
    @PasswordHash NVARCHAR(MAX)
AS
BEGIN
    UPDATE Users
    SET PasswordHash = @PasswordHash
    WHERE Id = @UserId
END
