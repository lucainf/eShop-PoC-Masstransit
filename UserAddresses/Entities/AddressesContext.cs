using Microsoft.EntityFrameworkCore;

namespace UserAddresses.Entities;

public class AddressesContext(DbContextOptions<AddressesContext> options) : DbContext(options)
{
    public virtual DbSet<Address> Addresses { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }
}