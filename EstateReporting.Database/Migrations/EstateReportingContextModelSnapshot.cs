﻿// <auto-generated />
using System;
using EstateReporting.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EstateReporting.Database.Migrations
{
    [DbContext(typeof(EstateReportingContext))]
    partial class EstateReportingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EstateReporting.Database.Entities.Estate", b =>
                {
                    b.Property<Guid>("EstateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EstateId");

                    b.ToTable("estate");
                });

            modelBuilder.Entity("EstateReporting.Database.Entities.EstateOperator", b =>
                {
                    b.Property<Guid>("EstateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OperatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("RequireCustomMerchantNumber")
                        .HasColumnType("bit");

                    b.Property<bool>("RequireCustomTerminalNumber")
                        .HasColumnType("bit");

                    b.HasKey("EstateId", "OperatorId");

                    b.ToTable("EstateOperators");
                });

            modelBuilder.Entity("EstateReporting.Database.Entities.EstateSecurityUser", b =>
                {
                    b.Property<Guid>("SecurityUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("EstateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmailAddress")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SecurityUserId", "EstateId");

                    b.ToTable("estatesecurityuser");
                });

            modelBuilder.Entity("EstateReporting.Database.Entities.Merchant", b =>
                {
                    b.Property<Guid>("EstateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MerchantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EstateId", "MerchantId");

                    b.ToTable("merchant");
                });

            modelBuilder.Entity("EstateReporting.Database.Entities.MerchantAddress", b =>
                {
                    b.Property<Guid>("EstateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MerchantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AddressId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AddressLine1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddressLine2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddressLine3")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddressLine4")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("PostalCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Region")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Town")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EstateId", "MerchantId", "AddressId");

                    b.ToTable("merchantaddress");
                });

            modelBuilder.Entity("EstateReporting.Database.Entities.MerchantContact", b =>
                {
                    b.Property<Guid>("EstateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MerchantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContactId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmailAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EstateId", "MerchantId", "ContactId");

                    b.ToTable("merchantcontact");
                });

            modelBuilder.Entity("EstateReporting.Database.Entities.MerchantDevice", b =>
                {
                    b.Property<Guid>("EstateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MerchantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DeviceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeviceIdentifier")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EstateId", "MerchantId", "DeviceId");

                    b.ToTable("merchantdevice");
                });

            modelBuilder.Entity("EstateReporting.Database.Entities.MerchantOperator", b =>
                {
                    b.Property<Guid>("EstateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MerchantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OperatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MerchantNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TerminalNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EstateId", "MerchantId", "OperatorId");

                    b.ToTable("MerchantOperators");
                });

            modelBuilder.Entity("EstateReporting.Database.Entities.MerchantSecurityUser", b =>
                {
                    b.Property<Guid>("EstateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MerchantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SecurityUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmailAddress")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EstateId", "MerchantId", "SecurityUserId");

                    b.ToTable("merchantsecurityuser");
                });
#pragma warning restore 612, 618
        }
    }
}
