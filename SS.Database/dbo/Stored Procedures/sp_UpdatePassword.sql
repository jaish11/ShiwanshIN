CREATE PROCEDURE sp_UpdatePassword  
    @UserId INT,  
    @PasswordHash NVARCHAR(MAX)  
AS  
BEGIN  
    UPDATE Users  
    SET PasswordHash = @PasswordHash,  
        PasswordResetToken = NULL,  
        PasswordResetTokenExpiry = NULL  
    WHERE Id = @UserId
END
