using JWTRefreshTokenNew.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JWTRefreshTokenNew.Data
{
    public class MyAuthContext:DbContext
    {
        public MyAuthContext()
        {
        }

        public MyAuthContext(DbContextOptions<MyAuthContext> context) : base(context)
        {

        }
        public DbSet<User> User { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
    }
   
}
