using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Login.Entities;

public class LoginContext(DbContextOptions<LoginContext> options) : IdentityDbContext<IdentityUser>(options)
{
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        
        }
}