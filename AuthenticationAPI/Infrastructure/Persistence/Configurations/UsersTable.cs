using AuthenticationAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthenticationAPI.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Represents the configuration for the "users" table in the database.
    /// Defines the table name, primary key, and column mappings, including constraints and default values.
    /// </summary>
    public class UsersTable : IEntityTypeConfiguration<User>
    {
        /// <summary>
        /// Configures the "users" table in the database by defining its schema, properties, and constraints.
        /// </summary>
        /// <param name="builder">An instance of <see cref="EntityTypeBuilder{User}"/> used to configure the "users" table.</param>
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            // Id (GUID):
            builder.HasKey((user) => user.UserId);  // Primary Key (PK).

            builder
                .Property((user) => user.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            // Other fields:
            builder
                .Property((user) => user.Username)
                .HasColumnName("username")
                .HasMaxLength(300)
                .IsRequired();

            builder
                .Property((user) => user.PasswordHash)
                .HasColumnName("password")
                .HasMaxLength(150)
                .IsRequired();

            builder
                .Property((user) => user.Role)
                .HasColumnName("role")
                .HasMaxLength(50)
                .IsRequired();

            // Dates:
            builder.Property<DateTime>("created_at")
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()")
                .IsRequired();

            builder.Property<DateTime>("updated_at")
                .HasColumnName("updated_at")
                .HasDefaultValueSql("NOW()")
                .IsRequired();
        }
    }
}