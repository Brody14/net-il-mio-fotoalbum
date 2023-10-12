using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Database
{
    public class PhotoContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=Photo;Integrated Security=True;TrustServerCertificate=True");
        }

    }
}
