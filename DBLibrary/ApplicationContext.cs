using DBLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace DBLibrary
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Department> Departments { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            string adminEmail = "admin@mail.ru";
            string adminPassword = "123456";

            // добавляем роли
            Role adminRole = new Role { Id = 1, RoleName = "admin", RoleDescription = "Администратор" };
            Role userRole = new Role { Id = 2, RoleName = "user", RoleDescription = "Пользователь" };
            Role kipRole = new Role { Id = 3, RoleName = "kip", RoleDescription = "КИПиА" };
            Role producerRole = new Role { Id = 4, RoleName = "producer", RoleDescription = "Режисерское управление" };

            Department department = new Department { Id = 1, DepartmentName = "Отдел IT", SequenceNumber = 29 };
            User adminUser = new User { Id = 1, Email = adminEmail, Login = "admin", Password = adminPassword, RoleId = adminRole.Id };

            modelBuilder.Entity<Department>().HasData(new Department[] { department });
            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole, kipRole, producerRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });

            modelBuilder.Entity<Friend>()
                .HasKey(f => new { f.AddedById, f.AddedToId });

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.AddedBy)
                .WithMany(u => u.AddedBys)
                .HasForeignKey(f => f.AddedById).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.AddedTo)
                .WithMany(u => u.AddedTos)
                .HasForeignKey(f => f.AddedToId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
