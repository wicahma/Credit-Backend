using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public partial class CreditContext: DbContext
    {
        public CreditContext()
        {
        }

        public CreditContext(DbContextOptions<CreditContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MstUser> MstUsers { get; set; }

        public virtual DbSet<MstLoans> MstLoans { get; set; }

        public virtual DbSet<TrnFunding> TrnFundings { get; set; }

        public virtual DbSet<TrnRepayment> TrnRepayments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Name=ConnectionStrings:DefaultConnection");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MstUser>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("mst_user_pkey");

                entity.ToTable("mst_user");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Balance).HasColumnName("balance");
                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email");
                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .HasColumnName("name");
                entity.Property(e => e.Pass)
                    .HasMaxLength(150)
                    .HasColumnName("pass");
                entity.Property(e => e.Role)
                    .HasMaxLength(30)
                    .HasColumnName("role");
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
