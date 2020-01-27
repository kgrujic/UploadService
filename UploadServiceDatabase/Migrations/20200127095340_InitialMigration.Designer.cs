﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UploadServiceDatabase.Context;

namespace UploadServiceDatabase.Migrations
{
    [DbContext(typeof(UploadServiceContext))]
    [Migration("20200127095340_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1");

            modelBuilder.Entity("UploadServiceDatabase.DTOs.FileDTO", b =>
                {
                    b.Property<string>("FilePath")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("HashedContent")
                        .HasColumnType("BLOB");

                    b.HasKey("FilePath");

                    b.ToTable("Files");
                });
#pragma warning restore 612, 618
        }
    }
}
