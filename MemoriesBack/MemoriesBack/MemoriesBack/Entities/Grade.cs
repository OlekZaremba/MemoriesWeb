using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemoriesBack.Entities
{
    [Table("grades")]
    public class Grade
    {
        [Key]
        [Column("idgrades")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int GradeValue { get; set; }

        public string? Description { get; set; }

        public string? Type { get; set; }

        [Required]
        [Column("issue_date", TypeName = "date")]
        public DateTime IssueDate { get; set; } = DateTime.Now;

        [Required]
        [Column("users_idstudent")]
        public int StudentId { get; set; }

        public User Student { get; set; }

        [Required]
        [Column("users_idteacher")]
        public int TeacherId { get; set; }

        public User Teacher { get; set; }

        [Required]
        [ForeignKey("SchoolClassId")]
        [Column("class_idclass")]
        public int SchoolClassId { get; set; }

        public SchoolClass SchoolClass { get; set; }
    }
}
