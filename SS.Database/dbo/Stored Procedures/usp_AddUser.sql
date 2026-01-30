CREATE PROCEDURE usp_AddUser    
(    
    @Email NVARCHAR(256),    
    @PasswordHash NVARCHAR(200),    
    @FullName NVARCHAR(200),    
    @Role NVARCHAR(50),    
    @IsEmailVerified BIT,  
    @EmailVerificationToken NVARCHAR(200)    
)    
AS    
BEGIN    
    SET NOCOUNT ON;    
  
    INSERT INTO Users    
        (Email, PasswordHash, FullName, Role, IsActive, CreatedDate, IsEmailVerified, EmailVerificationToken)    
    VALUES    
        (@Email, @PasswordHash, @FullName, @Role, 1, GETDATE(), @IsEmailVerified, @EmailVerificationToken);    
  
    SELECT SCOPE_IDENTITY();    
END  
