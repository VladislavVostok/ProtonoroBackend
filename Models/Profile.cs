using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtonoroBackend.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(100)]
        public string Country { get; set; }

        public string About { get; set; }

        [MaxLength(255)]
        public string Image { get; set; } = "default-user.jpg";

        [Required]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public override string ToString()
        {
            return !string.IsNullOrEmpty(FullName) ? FullName : User.FullName;
        }
    }
}