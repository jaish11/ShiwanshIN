CREATE PROCEDURE sp_GetUserByToken  
(  
    @Token NVARCHAR(200)  
)  
AS  
BEGIN  
    SET NOCOUNT ON;  
  
    SELECT TOP 1 *  
    FROM Users  
    WHERE EmailVerificationToken = @Token;  
END
