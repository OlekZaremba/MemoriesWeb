using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemoriesBack.Entities
{
    [Table("group_members")]
    public class GroupMember
    {
        [Key]
        [Column("idgroup_members")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("UserGroupId")]
        [Column("user_group_id")]
        public int UserGroupId { get; set; }

        public UserGroup UserGroup { get; set; }

        [Required]
        [ForeignKey("UserId")]
        [Column("users_idusers")]
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
