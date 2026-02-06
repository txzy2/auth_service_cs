using Microsoft.EntityFrameworkCore;
using MyMicroservice.Domain.Entities;

namespace MyMicroservice.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Roles> Roles { get; set; }
    public DbSet<Permissions> Permissions { get; set; }
    public DbSet<PermissionSettings> PermissionSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Конфигурация User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.Login)
                .HasColumnName("login")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired();

            entity.Property(e => e.ExtId)
                .HasColumnName("ext_id")
                .HasMaxLength(255);

            entity.Property(e => e.RoleId)
                .HasColumnName("role_id");

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            // Связь User -> Role (многие к одному)
            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Конфигурация Roles
        modelBuilder.Entity<Roles>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(e => e.Name)
                .IsUnique();
        });

        // Конфигурация Permissions
        modelBuilder.Entity<Permissions>(entity =>
        {
            entity.ToTable("permissions");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(e => e.Name)
                .IsUnique();
        });

        // Конфигурация PermissionSettings (связь User <-> Permission)
        modelBuilder.Entity<PermissionSettings>(entity =>
        {
            entity.ToTable("permission_settings");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(e => e.PermissionId)
                .HasColumnName("permission_id")
                .IsRequired();

            // Настройка связи с User
            entity.HasOne(ps => ps.User)
                .WithMany(u => u.PermissionSettings)
                .HasForeignKey(ps => ps.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка связи с Permission
            entity.HasOne(ps => ps.Permission)
                .WithMany(p => p.PermissionSettings)
                .HasForeignKey(ps => ps.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы для производительности
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.PermissionId);
            
            // Уникальное ограничение: один пользователь не может иметь одно разрешение дважды
            entity.HasIndex(e => new { e.UserId, e.PermissionId })
                .IsUnique();
        });
    }
}