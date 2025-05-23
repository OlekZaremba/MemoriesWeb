using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemoriesBack.Entities
{
    [Table("users")]
    public class User
    {
        public enum Role
        {
            T, // Teacher
            A, // Admin
            S  // Student
        }

        [Key]
        [Column("idusers")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        [Column("role")]
        public Role UserRole { get; set; }

        [Column("image", TypeName = "LONGBLOB")]
        public byte[] Image { get; set; }


        public ICollection<Grade> ReceivedGrades { get; set; }

        public ICollection<Grade> GivenGrades { get; set; }

        public SensitiveData SensitiveData { get; set; }

        public ICollection<GroupMember> GroupMemberships { get; set; }
    }
}
