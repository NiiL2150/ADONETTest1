using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows;

namespace ADONETTest1
{
    class Program
    {
        static void Main(string[] args)
        {
            string ConStr = "Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=Academy; Integrated Security = true;";
            using (SqlConnection connection = new SqlConnection(ConStr))
            {
                connection.Open();
                Init(connection);
                Console.WriteLine("1 - Отображение всей информации из таблицы со студентами и оценками\n" +
                    "2 - Отображение ФИО всех студентов\n" +
                    "3 - Отображение всех средних оценок\n" +
                    "4 - Показать ФИО всех студентов с минимальной оценкой, больше, чем указанная\n" +
                    "5 - Показать название всех предметов с минимальными средними оценками. Названия предметов должны быть уникальными\n" +
                    "6 - Показать минимальную среднюю оценку\n" +
                    "7 - Показать максимальную среднюю оценку\n" +
                    "8 - Показать количество студентов, у которых минимальная средняя оценка по математике\n" +
                    "9 - Показать количество студентов, у которых максимальная средняя оценка по математике\n" +
                    "10 - Показать количество студентов в каждой группе\n" +
                    "11 - Показать среднюю оценку по группе");
                int tmpint = Int32.Parse(Console.ReadLine());
                string query = "";
                Console.Clear();
                switch (tmpint)
                {
                    case 1:
                        query = query1;
                        break;
                    case 2:
                        query = query2;
                        break;
                    case 3:
                        query = query3;
                        break;
                    case 4:
                        query = query4;
                        Console.Write("Введите минимальное значение: ");
                        string tmpint2 = Console.ReadLine();
                        query += tmpint2;
                        break;
                    case 5:
                        query = query5;
                        break;
                    case 6:
                        query = query6;
                        break;
                    case 7:
                        query = query7;
                        break;
                    case 8:
                        query = query8;
                        break;
                    case 9:
                        query = query9;
                        break;
                    case 10:
                        query = query10;
                        break;
                    case 11:
                        query = query11;
                        break;
                    default:
                        break;
                }
                Console.Clear();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                do
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write($"{reader.GetName(i),-20}");
                    }
                    Console.WriteLine();
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write($"{reader[i].ToString(),-20}");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                } while (reader.NextResult());
                reader.Close();
            }
        }

        static void Init(SqlConnection connection)
        {
            try
            {
                string query = @"CREATE TABLE Groups(
                        Id int Identity (1,1) PRIMARY KEY,
                        GroupName nvarchar(100) not null
                        );

                        CREATE TABLE Students(
                        Id int Identity (1,1) PRIMARY KEY,
                        FirstName nvarchar(150) NOT NULL,
                        LastName nvarchar(200) NOT NULL,
                        GroupId int not NULL FOREIGN KEY REFERENCES Groups(Id)
                        );

                        CREATE TABLE Subjects(
                        Id int Identity (1,1) PRIMARY KEY,
                        [Name] nvarchar(100) NOT NULL
                        );

                        CREATE TABLE Marks(
                        Id int Identity (1,1) PRIMARY KEY,
                        StudentID int NOT NULL FOREIGN KEY REFERENCES Students(Id),
                        SubjectID int NOT NULL FOREIGN KEY REFERENCES Subjects(Id),
                        Mark int NOT NULL
                        );";
                SqlCommand command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
                MessageBox.Show("Tables created successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                query = @"INSERT INTO Groups(GroupName)
                        VALUES('Programming12013');
                        INSERT INTO Students(FirstName, LastName, GroupId)
                        VALUES (N'John', N'Doe', 1), (N'Cool', N'Dude', 1);
                        INSERT INTO Subjects([Name])
                        VALUES (N'Math'), (N'Programming');
                        INSERT INTO Marks(StudentID, SubjectID, Mark)
                        VALUES(1, 1, 12), (1, 2, 11), (2, 1, 10), (2, 2, 9);";
                command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                MessageBox.Show("Table is already created", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        static string query1 = @"SELECT G.GroupName, (S.FirstName + ' ' + S.LastName) AS FullName, Sub.[Name], M.Mark
FROM Students AS S
JOIN Marks AS M ON S.Id = M.StudentID
JOIN Groups AS G ON G.Id = S.GroupId
JOIN Subjects AS Sub ON M.SubjectID = Sub.Id";
        static string query2 = @"SELECT S.FirstName, S.LastName FROM Students as S";
        static string query3 = @"SELECT S.FirstName, S.LastName AVG(M.Mark) as AvgMark FROM Marks AS M
                        JOIN Students as S ON M.StudentID = S.Id
                        GROUP BY S.Id, S.FirstName, S.LastName";
        static string query4 = @"SELECT S.FirstName, S.LastName FROM Marks AS M
                        JOIN Students as S ON M.StudentID = S.Id
                        GROUP BY S.FirstName, S.LastName
                        HAVING MIN(M.Mark) > ";
        static string query5 = @"SELECT S.[Name] FROM Marks AS M
JOIN Subjects as S ON M.SubjectID = S.Id
GROUP BY S.[Name], M.SubjectID
HAVING AVG(M.Mark) = (SELECT MIN(Y.X) FROM (SELECT AVG(M.Mark) AS X FROM Marks as M JOIN Subjects as S ON M.SubjectID = S.Id
GROUP BY S.[Name]) AS Y)";

        static string query6 = @"SELECT MIN(Y.X) AS MinAvg FROM 
(SELECT AVG(M.Mark) AS X FROM Marks as M JOIN Subjects as S ON M.SubjectID = S.Id
GROUP BY S.[Name]) AS Y";

        static string query7 = @"SELECT MAX(Y.X) AS MaxAvg FROM 
(SELECT AVG(M.Mark) AS X FROM Marks as M JOIN Subjects as S ON M.SubjectID = S.Id
GROUP BY S.[Name]) AS Y";

        static string query8 = @"SELECT COUNT(Marks.Mark) AS StudentCount FROM Marks Where
Mark = (SELECT MIN(Y.X) as A FROM 
(SELECT AVG(M.Mark) AS X FROM Marks as M 
JOIN Subjects as S ON M.SubjectID = S.Id
JOIN Students as St ON St.Id = M.StudentID
WHERE S.[Name] = 'Math'
GROUP BY St.FirstName, St.LastName) AS Y)";

        static string query9 = @"SELECT COUNT(Marks.Mark) AS StudentCount FROM Marks Where
Mark = (SELECT MAX(Y.X) as A FROM 
(SELECT AVG(M.Mark) AS X FROM Marks as M 
JOIN Subjects as S ON M.SubjectID = S.Id
JOIN Students as St ON St.Id = M.StudentID
WHERE S.[Name] = 'Math'
GROUP BY St.FirstName, St.LastName) AS Y)";

        static string query10 = @"SELECT G.GroupName, COUNT(S.Id) AS StudentCount FROM Groups AS G
JOIN Students AS S ON S.GroupId = G.Id
GROUP BY G.Id, G.GroupName";

        static string query11 = @"SELECT G.GroupName, AVG(M.Mark) AS AvgMark FROM Groups AS G
JOIN Students AS S ON S.GroupId = G.Id
JOIN Marks AS M ON M.StudentID = S.Id
GROUP BY G.Id, G.GroupName";
    }
}
