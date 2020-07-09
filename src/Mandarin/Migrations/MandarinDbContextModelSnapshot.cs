﻿// <auto-generated />
using Mandarin.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
#pragma warning disable 1413
#pragma warning disable 1591
#pragma warning disable 1600
#pragma warning disable 1601

namespace Mandarin.Migrations
{
    [DbContext(typeof(MandarinDbContext))]
    internal partial class MandarinDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Mandarin.Models.Artists.Stockist", b =>
                {
                    b.Property<int>("StockistId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("stockist_id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasColumnType("character varying(500)")
                        .HasMaxLength(500);

                    b.Property<string>("StockistCode")
                        .IsRequired()
                        .HasColumnName("stockist_code")
                        .HasColumnType("text");

                    b.Property<string>("StockistName")
                        .IsRequired()
                        .HasColumnName("stockist_name")
                        .HasColumnType("text");

                    b.Property<string>("StockistStatus")
                        .HasColumnName("stockist_status")
                        .HasColumnType("character varying(25)");

                    b.HasKey("StockistId");

                    b.HasIndex("StockistCode")
                        .IsUnique()
                        .HasName("stockist_stockist_code_key");

                    b.HasIndex("StockistStatus");

                    b.ToTable("stockist","inventory");
                });

            modelBuilder.Entity("Mandarin.Models.Artists.StockistDetail", b =>
                {
                    b.Property<int>("StockistId")
                        .HasColumnName("stockist_id")
                        .HasColumnType("integer");

                    b.Property<string>("EmailAddress")
                        .HasColumnName("email_address")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<string>("FacebookHandle")
                        .HasColumnName("facebook_handle")
                        .HasColumnType("character varying(30)")
                        .HasMaxLength(30);

                    b.Property<string>("ImageUrl")
                        .HasColumnName("image_url")
                        .HasColumnType("character varying(150)")
                        .HasMaxLength(150);

                    b.Property<string>("InstagramHandle")
                        .HasColumnName("instagram_handle")
                        .HasColumnType("character varying(30)")
                        .HasMaxLength(30);

                    b.Property<string>("TumblrHandle")
                        .HasColumnName("tumblr_handle")
                        .HasColumnType("character varying(30)")
                        .HasMaxLength(30);

                    b.Property<string>("TwitterHandle")
                        .HasColumnName("twitter_handle")
                        .HasColumnType("character varying(30)")
                        .HasMaxLength(30);

                    b.Property<string>("WebsiteUrl")
                        .HasColumnName("website_url")
                        .HasColumnType("character varying(150)")
                        .HasMaxLength(150);

                    b.HasKey("StockistId");

                    b.ToTable("stockist_detail","inventory");
                });

            modelBuilder.Entity("Mandarin.Models.Common.Status", b =>
                {
                    b.Property<int>("StatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("status_id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<string>("StatusCode")
                        .IsRequired()
                        .HasColumnName("status_code")
                        .HasColumnType("character varying(25)")
                        .HasMaxLength(25);

                    b.HasKey("StatusId");

                    b.ToTable("status","static");
                });

            modelBuilder.Entity("Mandarin.Models.Artists.Stockist", b =>
                {
                    b.HasOne("Mandarin.Models.Common.Status", "CurrentStatus")
                        .WithMany("Stockists")
                        .HasForeignKey("StockistStatus")
                        .HasConstraintName("stockist_stockist_status_fkey")
                        .HasPrincipalKey("StatusCode");
                });

            modelBuilder.Entity("Mandarin.Models.Artists.StockistDetail", b =>
                {
                    b.HasOne("Mandarin.Models.Artists.Stockist", "Stockist")
                        .WithOne("StockistDetail")
                        .HasForeignKey("Mandarin.Models.Artists.StockistDetail", "StockistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
