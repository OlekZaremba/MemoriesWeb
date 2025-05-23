using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemoriesBack.Entities
{
    [Table("group_members_has_class")]
    public class GroupMemberClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("GroupMemberId")]
        [Column("group_members_idgroup_members")]
        public int GroupMemberId { get; set; }

        public GroupMember GroupMember { get; set; }

        [Required]
        [ForeignKey("SchoolClassId")]
        [Column("class_idclass")]
        public int SchoolClassId { get; set; }

        public SchoolClass SchoolClass { get; set; }

        public GroupMemberClass() { }

        public GroupMemberClass(GroupMember gm, SchoolClass cls)
        {
            GroupMember = gm;
            GroupMemberId = gm.Id;
            SchoolClass = cls;
            SchoolClassId = cls.Id;
        }
    }
}
