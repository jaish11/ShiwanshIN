CREATE PROCEDURE sp_VerifyUserEmail  
(  
    @Id INT  
)  
AS  
BEGIN  
    SET NOCOUNT ON;  
  
    UPDATE Users  
    SET   
        IsEmailVerified = 1,  
        EmailVerificationToken = NULL  
    WHERE Id = @Id;  
END
