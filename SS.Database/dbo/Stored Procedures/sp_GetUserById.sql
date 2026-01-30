CREATE PROCEDURE sp_GetUserById  
    @Id INT  
AS  
BEGIN  
    SELECT * FROM Users WHERE Id = @Id;  
END