CREATE PROCEDURE sp_UpdateUser  
    @Id INT,  
    @FullName NVARCHAR(200),  
    @IsActive BIT  
AS  
BEGIN  
    UPDATE Users  
    SET FullName = @FullName, IsActive = @IsActive  
    WHERE Id = @Id;  
END