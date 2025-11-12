using Microsoft.EntityFrameworkCore;
using MyGymSystem.Models;
using System.Collections.Generic;

namespace MyGymSystem.Context
{

    public class GymDbContext : DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { }

        // ---------- DbSets ----------
        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Userlogin> Userlogins => Set<Userlogin>();

        public DbSet<Member> Members => Set<Member>();
        public DbSet<Trainer> Trainers => Set<Trainer>();

        public DbSet<Membershipplan> Membershipplans => Set<Membershipplan>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();

        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Invoice> Invoices => Set<Invoice>();

        public DbSet<Testimonial> Testimonials => Set<Testimonial>();

        public DbSet<Homepage> Homepages => Set<Homepage>();
        public DbSet<Aboutuspage> Aboutuspages => Set<Aboutuspage>();
        public DbSet<Contactuspage> Contactuspages => Set<Contactuspage>();
        public DbSet<Workoutplan> Workoutplans => Set<Workoutplan>();   
        public DbSet<MemberTrainer> MemberTrainer => Set<MemberTrainer>();


		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================================================
            // Subscription <-> Payment : 1:1 via unique FK (allow many NULLs)
            // =====================================================
            // ======================================================================
            // 1️⃣ Subscription ↔ Payment  (one-to-one)
            // ======================================================================
            // Each subscription can have one payment record.
            // Many subscriptions can be unpaid (Paymentid = null).
            // Once a payment is linked, it must be unique — one payment = one subscription.
            // Restrict delete: you cannot delete a Payment while it's linked to a Subscription.

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Payment)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(s => s.Paymentid)
                .OnDelete(DeleteBehavior.Restrict);


            // Create a unique index on Paymentid (only when it's NOT NULL)
            // This means multiple unpaid subscriptions are allowed (many NULLs),
            // but no two subscriptions can share the same Paymentid.
            // SQL Server filtered unique index (keeps many NULLs)
            modelBuilder.Entity<Subscription>()
                .HasIndex(s => s.Paymentid)
                .IsUnique()
                .HasFilter("[Paymentid] IS NOT NULL");



            // Add indexes to speed up admin searches and reports by dates.

            // Helpful date indexes for admin filtering/reporting
            modelBuilder.Entity<Subscription>().HasIndex(s => s.Createdat);
            modelBuilder.Entity<Subscription>().HasIndex(s => s.Startdate);
            modelBuilder.Entity<Subscription>().HasIndex(s => s.Enddate);




            // =====================================================
            // Invoice <-> Payment : 1:1 (unique FK; allow many NULLs)
            // =====================================================
            // ======================================================================
            // 2️⃣ Invoice ↔ Payment  (one-to-one)
            // ======================================================================
            // Each payment can generate one invoice.
            // If a payment is deleted, the invoice is deleted too (Cascade delete).
            // Same logic: allow many nulls, enforce uniqueness when set.
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Payment)
                .WithMany(p => p.Invoices)
                .HasForeignKey(i => i.Paymentid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.Paymentid)
                .IsUnique()
                .HasFilter("[Paymentid] IS NOT NULL");

            // =====================================================
            // Workoutplan (customized per member)
            // =====================================================
            // ======================================================================
            // 3️⃣ Workoutplan ↔ Member / Trainer  (one-to-many)
            // ======================================================================
            // Each Workoutplan belongs to one Member.
            // When a Member is deleted, all their Workoutplans are also deleted (Cascade).

            modelBuilder.Entity<Workoutplan>()
                .HasOne(w => w.Member)
                .WithMany(m => m.Workoutplans)
                .HasForeignKey(w => w.Memberid)
                .OnDelete(DeleteBehavior.Cascade);

            // Each Workoutplan is created by one Trainer.
            // If the Trainer is deleted, EF will block it (Restrict)
            // until all their Workoutplans are reassigned or removed.
            modelBuilder.Entity<Workoutplan>()
                .HasOne(w => w.Trainer)
                .WithMany(t => t.Workoutplans)
                .HasForeignKey(w => w.Trainerid)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================================
            // Static Pages / Plans Admin relations (explicit FKs)
            // =====================================================
            // ======================================================================
            // 4️⃣ Static Pages (Home/About/Contact) ↔ Admin  (optional one-to-many)
            // ======================================================================
            // Pages are updated by Admins. If the Admin is deleted, 
            // the page remains but the Updatedbyadminid is set to NULL.
            modelBuilder.Entity<Aboutuspage>()
                .HasOne(p => p.Updatedbyadmin)
                .WithMany(a => a.Aboutuspages)
                .HasForeignKey(p => p.Updatedbyadminid)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Contactuspage>()
                .HasOne(p => p.Updatedbyadmin)
                .WithMany(a => a.Contactuspages)
                .HasForeignKey(p => p.Updatedbyadminid)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Homepage>()
                .HasOne(p => p.Updatedbyadmin)
                .WithMany(a => a.Homepages)
                .HasForeignKey(p => p.Updatedbyadminid)
                .OnDelete(DeleteBehavior.SetNull);


            // Membershipplans are created by Admins.
            // If that Admin is deleted, the Plan remains but Createdbyadminid = null.

            modelBuilder.Entity<Membershipplan>()
                .HasOne(p => p.Createdbyadmin)
                .WithMany(a => a.Membershipplans)
                .HasForeignKey(p => p.Createdbyadminid)
                .OnDelete(DeleteBehavior.SetNull);


            // =====================================================
            // Testimonial status constraint (provider-dependent)
            // =====================================================
            // ======================================================================
            // 5️⃣ Testimonial ↔ Admin/Member  (optional one-to-many)
            // ======================================================================
            // Add a database check constraint to ensure only valid statuses are stored.
            // This protects against typos like "Apporved" or "OK".

            modelBuilder.Entity<Testimonial>()
                .ToTable(tb => tb.HasCheckConstraint(
                    "CK_Testimonial_Approvalstatus",
                    "Approvalstatus in ('Pending','Approved','Rejected')"));

            // =====================================================
            // Uniqueness & string length constraints (some already via annotations)
            // =====================================================
            // (Optional) You can add FKs for clarity if you want:
            // modelBuilder.Entity<Testimonial>()
            //     .HasOne(t => t.Member).WithMany(m => m.Testimonials)
            //     .HasForeignKey(t => t.Memberid).OnDelete(DeleteBehavior.Cascade);
            //
            // modelBuilder.Entity<Testimonial>()
            //     .HasOne(t => t.Approvedbyadmin).WithMany(a => a.Testimonials)
            //     .HasForeignKey(t => t.Approvedbyadminid)
            //     .OnDelete(DeleteBehavior.SetNull);

            // ======================================================================
            // 6️⃣ Userlogin (unique usernames)
            // ======================================================================
            // Make sure no two users share the same username.
            modelBuilder.Entity<Userlogin>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // =====================================================
            // Precision for money/decimal fields (also annotated, but keep here for safety)
            // =====================================================
            // ======================================================================
            // 7️⃣ Money and decimal precision
            // ======================================================================
            // Configure how decimals are stored in SQL Server for currency values.
            // DECIMAL(10,2) means:
            // up to 10 total digits, with 2 digits after the decimal point.
          
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amountpaid)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.Totalamount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.Tax)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.Discount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Membershipplan>()
                .Property(p => p.Price)
                .HasPrecision(10, 2);





			modelBuilder.Entity<MemberTrainer>()
	        .HasOne(mt => mt.Member)
	        .WithMany(m => m.MemberTrainers) // add ICollection<MemberTrainer> to Member
	        .HasForeignKey(mt => mt.MemberId)
	        .OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<MemberTrainer>()
				.HasOne(mt => mt.Trainer)
				.WithMany(t => t.MemberTrainers) // add ICollection<MemberTrainer> to Trainer
				.HasForeignKey(mt => mt.TrainerId)
				.OnDelete(DeleteBehavior.Restrict);

			// Enforce 1 active trainer per member (filtered unique index; SQL Server)
			modelBuilder.Entity<MemberTrainer>()
				.HasIndex(mt => new { mt.MemberId, mt.IsActive })
				.IsUnique()
				.HasFilter("[IsActive] = 1");

		}




    }



}
