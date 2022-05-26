﻿// <auto-generated />
using System;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Database.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220526002142_UpdateUserRequestAccess")]
    partial class UpdateUserRequestAccess
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Database.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"), 1L, 1);

                    b.Property<int?>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ModifiedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CategoryId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("DeletedByUserId");

                    b.HasIndex("ModifiedByUserId");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            CategoryId = 1,
                            CreatedDateTime = new DateTime(2022, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc),
                            Name = "Any"
                        },
                        new
                        {
                            CategoryId = 2,
                            CreatedDateTime = new DateTime(2022, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc),
                            Name = "Breakfast"
                        },
                        new
                        {
                            CategoryId = 3,
                            CreatedDateTime = new DateTime(2022, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc),
                            Name = "Lunch"
                        },
                        new
                        {
                            CategoryId = 4,
                            CreatedDateTime = new DateTime(2022, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc),
                            Name = "Dinner"
                        },
                        new
                        {
                            CategoryId = 5,
                            CreatedDateTime = new DateTime(2022, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc),
                            Name = "Snack"
                        },
                        new
                        {
                            CategoryId = 6,
                            CreatedDateTime = new DateTime(2022, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc),
                            Name = "Desert"
                        });
                });

            modelBuilder.Entity("Database.Document", b =>
                {
                    b.Property<int>("DocumentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DocumentId"), 1L, 1);

                    b.Property<long>("ByteSize")
                        .HasColumnType("bigint");

                    b.Property<byte[]>("Bytes")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("ContentType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("DocumentKey")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ModifiedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RecipeId")
                        .HasColumnType("int");

                    b.HasKey("DocumentId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("DeletedByUserId");

                    b.HasIndex("ModifiedByUserId");

                    b.HasIndex("RecipeId");

                    b.ToTable("Document");
                });

            modelBuilder.Entity("Database.Log", b =>
                {
                    b.Property<int>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LogId"), 1L, 1);

                    b.Property<string>("Additional")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("FileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Level")
                        .HasColumnType("int");

                    b.Property<string>("LineNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("LogId");

                    b.HasIndex("UserId");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("Database.Recipe", b =>
                {
                    b.Property<int>("RecipeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RecipeId"), 1L, 1);

                    b.Property<string>("Author")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<TimeSpan?>("CookTime")
                        .HasColumnType("time");

                    b.Property<int?>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Directions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DirectionsHTML")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ingredients")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IngredientsHTML")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ModifiedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nutrition")
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan?>("PrepTime")
                        .HasColumnType("time");

                    b.Property<int?>("Servings")
                        .HasColumnType("int");

                    b.Property<string>("Storage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan?>("TotalTime")
                        .HasColumnType("time");

                    b.Property<string>("Yield")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RecipeId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("DeletedByUserId");

                    b.HasIndex("ModifiedByUserId");

                    b.ToTable("Recipes");
                });

            modelBuilder.Entity("Database.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedByIp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReasonRevoked")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReplacedByToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Revoked")
                        .HasColumnType("datetime2");

                    b.Property<string>("RevokedByIp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshToken");
                });

            modelBuilder.Entity("Database.Tag", b =>
                {
                    b.Property<int>("TagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TagId"), 1L, 1);

                    b.Property<int?>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DeletedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ModifiedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RecipeId")
                        .HasColumnType("int");

                    b.HasKey("TagId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("DeletedByUserId");

                    b.HasIndex("ModifiedByUserId");

                    b.HasIndex("RecipeId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Database.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"), 1L, 1);

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastIPAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastLogin")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastLogout")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("PasswordReset")
                        .HasColumnType("datetime2");

                    b.Property<bool>("RequestedAccess")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("RequestedAccessExpires")
                        .HasColumnType("datetime2");

                    b.Property<string>("ResetToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ResetTokenExpires")
                        .HasColumnType("datetime2");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("TimeZone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserGUID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("UtcOffset")
                        .HasColumnType("int");

                    b.Property<string>("VerificationToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Verified")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Database.Category", b =>
                {
                    b.HasOne("Database.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId");

                    b.HasOne("Database.User", "DeletedBy")
                        .WithMany()
                        .HasForeignKey("DeletedByUserId");

                    b.HasOne("Database.User", "ModifiedBy")
                        .WithMany()
                        .HasForeignKey("ModifiedByUserId");

                    b.Navigation("CreatedBy");

                    b.Navigation("DeletedBy");

                    b.Navigation("ModifiedBy");
                });

            modelBuilder.Entity("Database.Document", b =>
                {
                    b.HasOne("Database.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId");

                    b.HasOne("Database.User", "DeletedBy")
                        .WithMany()
                        .HasForeignKey("DeletedByUserId");

                    b.HasOne("Database.User", "ModifiedBy")
                        .WithMany()
                        .HasForeignKey("ModifiedByUserId");

                    b.HasOne("Database.Recipe", null)
                        .WithMany("Documents")
                        .HasForeignKey("RecipeId");

                    b.Navigation("CreatedBy");

                    b.Navigation("DeletedBy");

                    b.Navigation("ModifiedBy");
                });

            modelBuilder.Entity("Database.Log", b =>
                {
                    b.HasOne("Database.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Database.Recipe", b =>
                {
                    b.HasOne("Database.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Database.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId");

                    b.HasOne("Database.User", "DeletedBy")
                        .WithMany()
                        .HasForeignKey("DeletedByUserId");

                    b.HasOne("Database.User", "ModifiedBy")
                        .WithMany()
                        .HasForeignKey("ModifiedByUserId");

                    b.Navigation("Category");

                    b.Navigation("CreatedBy");

                    b.Navigation("DeletedBy");

                    b.Navigation("ModifiedBy");
                });

            modelBuilder.Entity("Database.RefreshToken", b =>
                {
                    b.HasOne("Database.User", null)
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Database.Tag", b =>
                {
                    b.HasOne("Database.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId");

                    b.HasOne("Database.User", "DeletedBy")
                        .WithMany()
                        .HasForeignKey("DeletedByUserId");

                    b.HasOne("Database.User", "ModifiedBy")
                        .WithMany()
                        .HasForeignKey("ModifiedByUserId");

                    b.HasOne("Database.Recipe", null)
                        .WithMany("Tags")
                        .HasForeignKey("RecipeId");

                    b.Navigation("CreatedBy");

                    b.Navigation("DeletedBy");

                    b.Navigation("ModifiedBy");
                });

            modelBuilder.Entity("Database.Recipe", b =>
                {
                    b.Navigation("Documents");

                    b.Navigation("Tags");
                });

            modelBuilder.Entity("Database.User", b =>
                {
                    b.Navigation("RefreshTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
