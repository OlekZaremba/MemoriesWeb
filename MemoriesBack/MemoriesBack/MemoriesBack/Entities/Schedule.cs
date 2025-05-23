using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemoriesBack.Entities
{
    [Table("schedule")]
    public class Schedule
    {
        [Key]
        [Column("idschedule")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("lesson_date", TypeName = "date")]
        public DateTime LessonDate { get; set; }

        [Required]
        [Column("start_time")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Column("end_time")]
        public TimeSpan EndTime { get; set; }

        [Required]
        [ForeignKey("GroupMemberClassId")]
        [Column("group_members_has_class_id")]
        public int GroupMemberClassId { get; set; }

        public GroupMemberClass GroupMemberClass { get; set; }

        [Required]
        [Column("generated")]
        public bool Generated { get; set; } = false;
    }
}
