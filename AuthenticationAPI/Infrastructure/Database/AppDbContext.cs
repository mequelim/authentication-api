using AuthenticationAPI.Entities;
using AuthenticationAPI.Infrastructure.Database.Configurations;
using AuthenticationAPI.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationAPI.Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        // Constructor:
        /// <summary>
        /// Represents the Entity Framework database context for the application.
        /// </summary>
        /// <remarks>
        /// This context provides access to the database and manages entity configurations.
        /// It facilitates querying and persisting objects to a relational database by configuring entity mappings, relationships, and conventions.
        /// </remarks>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        // Methods:
        /// <summary>
        /// Configures the entity mappings, relationships, and conventions for the application database.
        /// </summary>
        /// <param name="modelBuilder">
        /// An instance of <see cref="ModelBuilder"/> used to configure the database schema and entity relationships.
        /// </param>
        /// <remarks>
        /// This method is responsible for applying specific configurations to the database model, including:
        /// - Mapping entity configurations registered using <see cref="IEntityTypeConfiguration{TEntity}"/>.
        /// - Applying global conventions defined in the application.
        /// - Configuring additional properties and behaviors for specific entities, such as concurrency tokens and column mapping.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UsersTable());
            modelBuilder.ApplyGlobalConventions();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}