using System.Data.Entity;

namespace FProj.Data
{
    public class FProjContext: DbContext
    {
        public DbSet<UserAccount> UserAccount { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Film> Film { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<Actor> Actor { get; set; }
        public DbSet<Genre> Genre { get; set; }

        public FProjContext(): base("FProjConnection")
        {

        }

        public FProjContext(string connectionString): base(connectionString)
        {

        }
    }
}
