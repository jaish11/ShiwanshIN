CREATE   PROCEDURE sp_UpdateEmailToken
(
    @Id INT,
    @Token NVARCHAR(200)
)
AS
BEGIN
    UPDATE Users
    SET EmailVerificationToken = @Token
    WHERE Id = @Id;
END
