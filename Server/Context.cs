using System.Globalization;
using Microsoft.EntityFrameworkCore;
using HW5.DB;

namespace HW5
{
    public class Context: DbContext
    {
        public DbSet<User> Users { get; set; } 
        
        public DbSet<Product> Products { get; set; } 
        
        public DbSet<Recipe> Recipes { get; set; } 
        
        public DbSet<Category> Category { get; set; } 
        
        public DbSet<Subscribe> Subscribe { get; set; } 
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost,1433;Database=Lab5;User=sa;Password=P@55w0rd;");
        }
    }
}