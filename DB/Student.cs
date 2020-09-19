using System;

namespace DB
{
    public class Student
    {
        public int Id { set; get; }
        public string Name { get; set; }
        public bool IsStudy { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}