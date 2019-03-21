using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<Guest> Guest {get;set;}
        public DbSet<RSVP> RSVP {get;set;}
        public DbSet<Wedding> Wedding {get;set;}
        // base() calls the parent class' constructor passing the "options" parameter along
        
    }
    
}