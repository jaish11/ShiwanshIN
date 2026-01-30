CREATE PROCEDURE sp_SetPasswordResetToken
    @Email NVARCHAR(200),
    @Token NVARCHAR(200),
    @Expiry DATETIME
AS
BEGIN
    UPDATE Users
    SET PasswordResetToken = @Token,
        PasswordResetTokenExpiry = @Expiry
    WHERE Email = @Email
END
