using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemoriesBack.Entities
{
    [Table("password_reset_tokens")]
    public class PasswordResetToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column("token")]
        public string Token { get; set; }

        [Required]
        [ForeignKey("UserId")]
        [Column("user_id")]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }
    }
}
