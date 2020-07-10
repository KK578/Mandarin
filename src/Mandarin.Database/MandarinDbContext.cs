using Mandarin.Models.Artists;
using Mandarin.Models.Commissions;
using Mandarin.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace Mandarin.Services.Entity
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
        /// Gets or sets the Database Set related to Available statuses.
        /// </summary>
        public virtual DbSet<Status> Status { get; set; }

        /// <summary>
        /// Gets or sets the Database Set related to stockists.
        /// </summary>
        public virtual DbSet<Stockist> Stockist { get; set; }

        /// <summary>
        /// Gets or sets the Database Set related to stockist's personal details.
        /// </summary>
        public virtual DbSet<StockistDetail> StockistDetail { get; set; }

        /// <summary>
        /// Gets or sets the Database Set related to stockist commission periods.
        /// </summary>
        public virtual DbSet<Commission> Commission { get; set; }

        /// <summary>
        /// Gets or sets the Database Set related to commission rates.
        /// </summary>
        public virtual DbSet<CommissionRateGroup> CommissionRateGroup { get; set; }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Commission>(entity =>
            {
                entity.Property(e => e.InsertedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.StartDate).HasDefaultValueSql("CURRENT_DATE");
            });

            modelBuilder.Entity<Stockist>(entity =>
            {
                entity.HasIndex(e => e.StockistCode).HasName("stockist_stockist_code_key").IsUnique();

                entity.HasOne(d => d.Status)
                .WithMany(p => p.Stockists)
                .HasPrincipalKey(p => p.StatusCode)
                .HasForeignKey(d => d.StatusCode)
                .HasConstraintName("stockist_stockist_status_fkey");
            });

            modelBuilder.Entity<StockistDetail>();
        }
    }
}
