﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Vocab.Infrastructure.Persistence;

#nullable disable

namespace Vocab.WebApi.Migrations
{
    [DbContext(typeof(VocabContext))]
    [Migration("20240409063919_RenameStatementPairOriginToSource")]
    partial class RenameStatementPairOriginToSource
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Vocab.Core.Entities.StatementDictionary", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("StatementDictionaries");
                });

            modelBuilder.Entity("Vocab.Core.Entities.StatementPair", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("RelatedDictionaryId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("StatementCategory")
                        .HasColumnType("integer");

                    b.Property<string>("Target")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RelatedDictionaryId");

                    b.ToTable("StatementPairs");
                });

            modelBuilder.Entity("Vocab.Core.Entities.StatementPair", b =>
                {
                    b.HasOne("Vocab.Core.Entities.StatementDictionary", "StatementsDictionary")
                        .WithMany("StatementPairs")
                        .HasForeignKey("RelatedDictionaryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("StatementsDictionary");
                });

            modelBuilder.Entity("Vocab.Core.Entities.StatementDictionary", b =>
                {
                    b.Navigation("StatementPairs");
                });
#pragma warning restore 612, 618
        }
    }
}