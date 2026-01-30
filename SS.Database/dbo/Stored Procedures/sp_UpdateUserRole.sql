CREATE PROCEDURE sp_UpdateUserRole  
    @Id INT,  
    @Role NVARCHAR(50)  
AS  
BEGIN  
    UPDATE Users  
    SET Role = @Role  
    WHERE Id = @Id;  
END
