﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using backend_pastebook_capstone.Data;

#nullable disable

namespace backend_pastebook_capstone.Migrations
{
    [DbContext(typeof(CapstoneDBContext))]
    partial class CapstoneDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("backend_pastebook_capstone.AuthenticationService.Models.AccessToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Token")
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.ToTable("AccessToken");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Album", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("AlbumName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Album");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CommentContent")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("CommenterId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("DateCommented")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("PostId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("CommenterId");

                    b.HasIndex("PostId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Friend", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("FriendshipDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsFriend")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("ReceiverId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("ReceiverId");

                    b.HasIndex("SenderId");

                    b.ToTable("Friend");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Like", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("LikerId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("PostId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("LikerId");

                    b.HasIndex("PostId");

                    b.ToTable("Like");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Notification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ContextId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsRead")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("NotificationType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("NotifiedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("NotifiedUserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("NotifiedUserId");

                    b.ToTable("Notification");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Photo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("AlbumId")
                        .HasColumnType("char(36)");

                    b.Property<string>("PhotoImageURL")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.ToTable("Photo");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("DatePosted")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("PhotoId")
                        .HasColumnType("char(36)");

                    b.Property<string>("PostBody")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PostTitle")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("PosterId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("TimelineId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("PhotoId");

                    b.HasIndex("PosterId");

                    b.HasIndex("TimelineId");

                    b.ToTable("Post");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Timeline", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("TimeLine");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("AboutMe")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("DATE");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("HashedPassword")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<Guid?>("PhotoId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Sex")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("PhotoId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Verification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("VerificationCode")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Verification");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Album", b =>
                {
                    b.HasOne("backend_pastebook_capstone.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Comment", b =>
                {
                    b.HasOne("backend_pastebook_capstone.Models.User", "Commenter")
                        .WithMany()
                        .HasForeignKey("CommenterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend_pastebook_capstone.Models.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Commenter");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Friend", b =>
                {
                    b.HasOne("backend_pastebook_capstone.Models.User", "Receiver")
                        .WithMany()
                        .HasForeignKey("ReceiverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend_pastebook_capstone.Models.User", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Like", b =>
                {
                    b.HasOne("backend_pastebook_capstone.Models.User", "Liker")
                        .WithMany()
                        .HasForeignKey("LikerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend_pastebook_capstone.Models.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Liker");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Notification", b =>
                {
                    b.HasOne("backend_pastebook_capstone.Models.User", "NotifiedUser")
                        .WithMany()
                        .HasForeignKey("NotifiedUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NotifiedUser");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Photo", b =>
                {
                    b.HasOne("backend_pastebook_capstone.Models.Album", "Album")
                        .WithMany()
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Album");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Post", b =>
                {
                    b.HasOne("backend_pastebook_capstone.Models.Photo", "Photo")
                        .WithMany()
                        .HasForeignKey("PhotoId");

                    b.HasOne("backend_pastebook_capstone.Models.User", "Poster")
                        .WithMany()
                        .HasForeignKey("PosterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend_pastebook_capstone.Models.Timeline", "Timeline")
                        .WithMany()
                        .HasForeignKey("TimelineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Photo");

                    b.Navigation("Poster");

                    b.Navigation("Timeline");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.Timeline", b =>
                {
                    b.HasOne("backend_pastebook_capstone.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("backend_pastebook_capstone.Models.User", b =>
                {
                    b.HasOne("backend_pastebook_capstone.Models.Photo", "Photo")
                        .WithMany()
                        .HasForeignKey("PhotoId");

                    b.Navigation("Photo");
                });
#pragma warning restore 612, 618
        }
    }
}
