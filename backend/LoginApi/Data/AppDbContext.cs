using LoginApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginApi.Data;

/// <summary>
/// Database context for the application
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Busta> Buste { get; set; }
    public DbSet<BanconoteWithdrawalAutomatic> BanconoteWithdrawalAutomatic { get; set; }
    public DbSet<ExpenseFund> ExpenseFund { get; set; }
    public DbSet<CashFund> CashFund { get; set; }
    public DbSet<MonetaryFund> MonetaryFund { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
            
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(u => u.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        
        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.HasIndex(rt => rt.Token).IsUnique();
            
            entity.Property(rt => rt.Token).IsRequired();
            entity.Property(rt => rt.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Relationship: RefreshToken belongs to User
            entity.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Busta configuration
        modelBuilder.Entity<Busta>(entity =>
        {
            entity.HasKey(b => b.Id);
            
            entity.Property(b => b.DataRiferimento).IsRequired();
            entity.Property(b => b.Totale).HasPrecision(18, 2);
            entity.Property(b => b.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Relationship: Busta belongs to User
            entity.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
