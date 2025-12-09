using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace ProtonoroBackend.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public override string UserName { get; set; }


        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(100)]
        public string? OTP { get; set; }

        [MaxLength(1000)]
        public string? RefreshToken { get; set; }

        public Profile Profile { get; set; }
    }
}
