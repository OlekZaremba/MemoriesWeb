using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemoriesBack.Entities
{
    [Table("sensitive_data")]
    public class SensitiveData
    {
        [Key]
        [Column("idsensitive_data")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        [MaxLength(256)]
        public string Password { get; set; }
        
        [Column("email")]
        public string Email { get; set; }

        
        [Required]
        [ForeignKey("UserId")]
        [Column("users_idusers")]
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
