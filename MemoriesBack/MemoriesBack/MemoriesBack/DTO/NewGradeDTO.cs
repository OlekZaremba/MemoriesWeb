﻿namespace MemoriesBack.DTO
{
    public class NewGradeDTO
    {
        public int Id { get; set; }
        public int Grade { get; set; }
        public string Type { get; set; }
        public string IssueDate { get; set; }
        public string ClassName { get; set; }

        public NewGradeDTO(int id, int grade, string type, string issueDate, string className)
        {
            Id = id;
            Grade = grade;
            Type = type;
            IssueDate = issueDate;
            ClassName = className;
        }
    }
}