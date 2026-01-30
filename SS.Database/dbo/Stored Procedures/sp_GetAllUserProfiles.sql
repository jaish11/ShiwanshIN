CREATE PROCEDURE sp_GetAllUserProfiles
AS
BEGIN
    SELECT *
    FROM UserProfiles
    ORDER BY CreatedAt DESC;
END
