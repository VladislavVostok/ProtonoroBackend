using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace ProtonoroBackend.Models
{
    public class PomodoroDBContext : IdentityDbContext<AppUser>
    {


        public PomodoroDBContext() { }
        public PomodoroDBContext(DbContextOptions<PomodoroDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
