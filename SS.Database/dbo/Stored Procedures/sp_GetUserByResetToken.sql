CREATE PROCEDURE sp_GetUserByResetToken  
    @Token NVARCHAR(200)  
AS  
BEGIN  
    SELECT TOP 1 *
    FROM Users  
    WHERE PasswordResetToken = @Token  
      AND PasswordResetTokenExpiry > GETDATE()
END
