using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.EFCore.DataContext
{
    public class BookDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; } 
        public DbSet<Book> Books { get; set; } 
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Genre> Genres  { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-JIHG98N\SQLEXPRESS;Database=BookStoreManagement;Trusted_Connection=True;TrustServerCertificate=True");
        }
    }
}