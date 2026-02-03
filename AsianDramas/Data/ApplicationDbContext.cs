using AsianDramas.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AsianDramas.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Drama> Dramas { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<DramaActor> DramaActors { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<MyListItem> MyListItems { get; set; }
        public DbSet<ActorReview> ActorReviews { get; set; } = null!;



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<DramaActor>()
                .HasKey(da => new { da.DramaId, da.ActorId });

            builder.Entity<Drama>().Property(d => d.Title).HasMaxLength(150).IsRequired();
            builder.Entity<Drama>().Property(d => d.Description).HasMaxLength(1000).IsRequired();

            builder.Entity<Actor>().Property(a => a.Name).HasMaxLength(100).IsRequired();
            builder.Entity<Actor>().Property(a => a.Biography).HasMaxLength(200);

            builder.Entity<Review>().Property(r => r.ReviewerName).HasMaxLength(50).IsRequired();
            builder.Entity<Review>().Property(r => r.Comment).HasMaxLength(1000);
            


           
            builder.Entity<Drama>()
                   .Property(d => d.AverageRating)
                   .HasColumnType("decimal(5,2)")
                   .HasPrecision(5, 2);
        }



    }
}
