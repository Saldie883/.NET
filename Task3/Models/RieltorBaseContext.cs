using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Task1
{
    public partial class RieltorBaseContext : DbContext
    {
        public RieltorBaseContext()
        {
        }

        public RieltorBaseContext(DbContextOptions<RieltorBaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Agencyes> Agencyes { get; set; }
        public virtual DbSet<Brockers> Brockers { get; set; }
        public virtual DbSet<Invoices> Invoices { get; set; }
        public virtual DbSet<Operations> Operations { get; set; }
        public virtual DbSet<Persons> Persons { get; set; }
        public virtual DbSet<RealStateTypes> RealStateTypes { get; set; }
        public virtual DbSet<RealStates> RealStates { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=WIN-VKKPI9QJ4AH;Initial Catalog=RieltorBase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agencyes>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.License)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasColumnType("text");

                entity.HasOne(d => d.Director)
                    .WithMany(p => p.Agencyes)
                    .HasForeignKey(d => d.DirectorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Agencyes_fk0");
            });

            modelBuilder.Entity<Brockers>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasColumnType("text");

                entity.HasOne(d => d.Agency)
                    .WithMany(p => p.Brockers)
                    .HasForeignKey(d => d.AgencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Brockers_fk0");
            });

            modelBuilder.Entity<Invoices>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.RealStateAddress)
                    .IsRequired()
                    .HasColumnType("text");

                entity.HasOne(d => d.Brocker)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.BrockerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Invoices_fk1");

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.OperationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Invoices_fk0");
            });

            modelBuilder.Entity<Operations>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("text");
            });

            modelBuilder.Entity<Persons>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("text");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Persons)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Persons_fk0");
            });

            modelBuilder.Entity<RealStateTypes>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("text");
            });

            modelBuilder.Entity<RealStates>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Independency)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsFixedLength();

                entity.Property(e => e.RegistrationNumber)
                    .IsRequired()
                    .HasColumnType("text");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.RealStates)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RealStates_fk0");
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("text");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
