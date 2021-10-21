CREATE DATABASE Academy;
USE Academy

CREATE TABLE Students(
	Id int IDENTITY(1,1) PRIMARY KEY,
	FirstName NVARCHAR(150) NOT NULL,
	LastName NVARCHAR(150) NOT NULL,
)
DROP TABLE Students
DROP TABLE Marks
SELECT * FROM Students
SELECT AVG(m.Mark) as Mark, s.FirstName FROM Marks as m
JOIN Students as s ON s.Id = m.StudentId
GROUP BY s.FirstName