using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemoriesBack.Entities
{
    [Table("class")]
    public class SchoolClass
    {
        [Key]
        [Column("idclass")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("class_name")]
        public string ClassName { get; set; }
    }
}
