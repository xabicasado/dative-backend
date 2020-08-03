﻿// <auto-generated />
using DativeBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DativeBackend.Migrations
{
    [DbContext(typeof(CustomerContext))]
    [Migration("20200803130407_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Customer", b =>
                {
                    b.Property<int>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("PostalCode")
                        .HasColumnType("int");

                    b.Property<string>("Surname")
                        .HasColumnType("text");

                    b.HasKey("CustomerId");

                    b.ToTable("Customer");
                });
#pragma warning restore 612, 618
        }
    }
}