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

        // public virtual DbSet<Commission> Commission { get; set; }
        // public virtual DbSet<CommissionRateGroup> CommissionRateGroup { get; set; }

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
        public virtual DbSet<Models.Commissions.Commission> Commission { get; set; }

        /// <summary>
        /// Gets or sets the Database Set related to commission rates.
        /// </summary>
        public virtual DbSet<CommissionRateGroup> CommissionRateGroup { get; set; }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<Commission>(entity =>
            // {
            //     entity.ToTable("commission", "billing");
            //
            //     entity.Property(e => e.CommissionId).HasColumnName("commission_id");
            //
            //     entity.Property(e => e.EndDate)
            //         .HasColumnName("end_date")
            //         .HasColumnType("date");
            //
            //     entity.Property(e => e.InsertedAt)
            //         .HasColumnName("inserted_at")
            //         .HasDefaultValueSql("CURRENT_TIMESTAMP");
            //
            //     entity.Property(e => e.RateGroup).HasColumnName("rate_group");
            //
            //     entity.Property(e => e.StartDate)
            //         .HasColumnName("start_date")
            //         .HasColumnType("date")
            //         .HasDefaultValueSql("CURRENT_DATE");
            //
            //     entity.Property(e => e.StockistId).HasColumnName("stockist_id");
            //
            //     entity.HasOne(d => d.RateGroupNavigation)
            //         .WithMany(p => p.Commission)
            //         .HasForeignKey(d => d.RateGroup)
            //         .HasConstraintName("commission_rate_group_fkey");
            //
            //     entity.HasOne(d => d.Stockist)
            //         .WithMany(p => p.Commission)
            //         .HasForeignKey(d => d.StockistId)
            //         .HasConstraintName("commission_stockist_id_fkey");
            // });
            //
            // modelBuilder.Entity<CommissionRateGroup>(entity =>
            // {
            //     entity.HasKey(e => e.GroupId)
            //         .HasName("commission_rate_group_pkey");
            //
            //     entity.ToTable("commission_rate_group", "billing");
            //
            //     entity.Property(e => e.GroupId).HasColumnName("group_id");
            //
            //     entity.Property(e => e.Rate).HasColumnName("rate");
            // });
            //
            // modelBuilder.Entity<Status>(entity =>
            // {
            //     entity.ToTable("status", "static");
            //
            //     entity.HasIndex(e => e.StatusCode)
            //         .HasName("status_status_code_key")
            //         .IsUnique();
            //
            //     entity.Property(e => e.StatusId).HasColumnName("status_id");
            //
            //     entity.Property(e => e.Description)
            //         .HasColumnName("description")
            //         .HasMaxLength(100);
            //
            //     entity.Property(e => e.StatusCode)
            //         .IsRequired()
            //         .HasColumnName("status_code")
            //         .HasMaxLength(25);
            // });
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
