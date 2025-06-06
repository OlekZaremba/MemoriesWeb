﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace MemoriesBack.Entities
{
    [Table("users")]
    public class User
    {
        public enum Role
        {
            [EnumMember(Value = "T")]
            T, 

            [EnumMember(Value = "A")]
            A, 

            [EnumMember(Value = "S")]
            S  
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

        [Column("image", TypeName = "TEXT")]
        public string? Image { get; set; }

        public ICollection<Grade> ReceivedGrades { get; set; }

        public ICollection<Grade> GivenGrades { get; set; }

        public SensitiveData SensitiveData { get; set; }

        public ICollection<GroupMember> GroupMemberships { get; set; }
    }

}