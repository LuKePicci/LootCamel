using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using LootCamel.Data;

namespace LootCamel.Migrations
{
    [DbContext(typeof(LootCamelContext))]
    [Migration("20170412200653_ItemBasePrice")]
    partial class ItemBasePrice
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("LootCamel.Models.LootItem", b =>
                {
                    b.Property<int>("ID");

                    b.Property<int>("BasePrice");

                    b.Property<string>("ItemName");

                    b.HasKey("ID");

                    b.HasIndex("ItemName")
                        .IsUnique();

                    b.ToTable("LootItems");
                });

            modelBuilder.Entity("LootCamel.Models.LootPlayer", b =>
                {
                    b.Property<int>("ID");

                    b.Property<string>("Nickname");

                    b.HasKey("ID");

                    b.HasIndex("Nickname")
                        .IsUnique();

                    b.ToTable("LootPlayers");
                });

            modelBuilder.Entity("LootCamel.Models.Price", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<int>("LootItemID");

                    b.Property<int>("Value");

                    b.HasKey("ID");

                    b.HasIndex("LootItemID");

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("LootCamel.Models.Subscription", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ChatID");

                    b.Property<int>("LootItemID");

                    b.Property<int>("LootPlayerID");

                    b.Property<int>("PriceEvent");

                    b.HasKey("ID");

                    b.HasIndex("LootItemID");

                    b.HasIndex("LootPlayerID");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("LootCamel.Models.Price", b =>
                {
                    b.HasOne("LootCamel.Models.LootItem", "Item")
                        .WithMany("Prices")
                        .HasForeignKey("LootItemID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LootCamel.Models.Subscription", b =>
                {
                    b.HasOne("LootCamel.Models.LootItem", "Item")
                        .WithMany("Subscriptions")
                        .HasForeignKey("LootItemID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LootCamel.Models.LootPlayer", "Subscriber")
                        .WithMany()
                        .HasForeignKey("LootPlayerID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
