using Mandarin.Models.Artists;
using Mandarin.Models.Commissions;
using Mandarin.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace Mandarin.Database
{
    /// <summary>
    /// Represents a Database Context for accessing The Little Mandarin data.
    /// </summary>
    public class MandarinDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinDbContext"/> class.
        /// </summary>
        public MandarinDbContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinDbContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public MandarinDbContext(DbContextOptions<MandarinDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the Database Set related to stockist commission periods.
        /// </summary>
        public virtual DbSet<Commission> Commission { get; set; }

        /// <summary>
        /// Gets or sets the Database Set related to commission rates.
        /// </summary>
        public virtual DbSet<CommissionRateGroup> CommissionRateGroup { get; set; }

        /// <summary>
        /// Gets or sets the Database Set related to stockists.
        /// </summary>
        public virtual DbSet<Stockist> Stockist { get; set; }

        /// <summary>
        /// Gets or sets the Database Set related to stockist's personal details.
        /// </summary>
        public virtual DbSet<StockistDetail> StockistDetail { get; set; }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Commission>(entity =>
            {
                entity.Property(e => e.InsertedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.StartDate).HasDefaultValueSql("CURRENT_DATE");

                entity.HasOne(d => d.RateGroup)
                      .WithMany(p => p.Commissions)
                      .HasForeignKey(d => d.RateGroupId)
                      .HasConstraintName("commission_rate_group_fkey");

                entity.HasOne(d => d.Stockist)
                      .WithMany(p => p.Commissions)
                      .HasForeignKey(d => d.StockistId)
                      .HasConstraintName("commission_stockist_id_fkey");
            });

            modelBuilder.Entity<CommissionRateGroup>(entity =>
            {
                entity.HasKey(e => e.GroupId)
                      .HasName("commission_rate_group_pkey");
            });

            modelBuilder.Entity<Stockist>(entity =>
            {
                entity.HasIndex(e => e.StockistCode)
                      .HasName("stockist_stockist_code_key")
                      .IsUnique();
            });

            modelBuilder.Entity<StockistDetail>(entity =>
            {
                entity.HasKey(e => e.StockistId)
                      .HasName("stockist_detail_pkey");

                entity.Property(e => e.StockistId).ValueGeneratedNever();

                entity.HasOne(d => d.Stockist)
                      .WithOne(p => p.Details)
                      .HasForeignKey<StockistDetail>(d => d.StockistId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("stockist_detail_stockist_id_fkey");
            });
        }
    }
}
