using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Connect;
using MySql.Data.MySqlClient;

namespace DB
{
    internal static class Program
    {
        private static async Task Main()
        {
            DBConnect.Open();

            const string SQL = "SELECT * FROM table_student";
            var result = DBConnect.SelectQuery(SQL);
            var students = GetStudents(result);
            
            await using (var file = new FileStream("students.json", FileMode.OpenOrCreate))
            {
                await JsonSerializer.SerializeAsync(file, students);
            }
            
            var studentsJson = new List<Student>();
            await using (var file = new FileStream("students.json", FileMode.Open))
            {
                studentsJson = JsonSerializer.DeserializeAsync<List<Student>>(file).Result;
            }
            
            DBConnect.Reopen();
            foreach (var student in studentsJson)
            {
                var sql = $"INSERT INTO table_student (name, is_study, date_of_birth) VALUES ('{student.Name}', '{Convert.ToInt32(student.IsStudy)}', '{student.DateOfBirth:yyyy-MM-dd}')";
                if (DBConnect.NoSelectQuery(sql)) Console.WriteLine("OK");
            }

        }

        private static List<Student> GetStudents(MySqlDataReader result)
        {
            var students = new List<Student>();
            while (result.Read())
            {
                var student = new Student
                {
                    Id = result.GetInt32("id"),
                    Name = result.GetString("name"),
                    IsStudy = result.GetBoolean("is_study"),
                    DateOfBirth = result.GetDateTime("date_of_birth")
                };
                students.Add(student);
            }
            return students;
        }
    }
}