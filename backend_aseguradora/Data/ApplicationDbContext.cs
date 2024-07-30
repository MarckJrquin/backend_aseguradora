using Microsoft.EntityFrameworkCore;
using backend_aseguradora.Models;

namespace backend_aseguradora.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<InsuranceType> InsuranceTypes { get; set; }
        public DbSet<Coverage> Coverages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.Person)
                .WithOne(p => p.User)
                .HasForeignKey<Person>(p => p.UserId);

            // Se inicializan roles predeterminados
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "User" },
                new Role { Id = 2, Name = "Admin" }
            );

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Quote>()
                .HasOne(q => q.Car)
                .WithMany()
                .HasForeignKey(q => q.CarId);

            modelBuilder.Entity<Quote>()
                .HasOne(q => q.User)
                .WithMany(u => u.Quotes)
                .HasForeignKey(q => q.UserId);

            modelBuilder.Entity<Quote>()
                .HasOne(q => q.InsuranceType)
                .WithMany()
                .HasForeignKey(q => q.InsuranceTypeId);

            modelBuilder.Entity<Quote>()
                .HasOne(q => q.Coverage)
                .WithMany()
                .HasForeignKey(q => q.CoverageId);

            // Inicializar datos de InsuranceType y Coverage
            modelBuilder.Entity<InsuranceType>().HasData(
                new InsuranceType { Id = 1, Name = "terceros" },
                new InsuranceType { Id = 2, Name = "completo" }
            );

            modelBuilder.Entity<Coverage>().HasData(
                new Coverage { Id = 1, Name = "responsabilidad civil" },
                new Coverage { Id = 2, Name = "limitada" },
                new Coverage { Id = 3, Name = "amplia" }
            );

        }
    }
}
