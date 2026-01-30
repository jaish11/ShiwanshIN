CREATE PROCEDURE sp_GetUserByEmail  
    @Email NVARCHAR(256)  
AS  
BEGIN  
    SELECT * FROM Users WHERE Email = @Email;  
END
