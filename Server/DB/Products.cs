using System;
using System.Collections.Generic;


namespace HW5.DB
{
    public class Product
    {
        public int ProductId { get; set; }
        
        public string ProductName { get; set; }
        
        public int Count { get; set; }
        
        public DateTime ShelfLife { get; set; }
        
        public List<User> ProductOfUsers { get; set; }
        
        public List<Recipe> Recipes { get; set; }
    }
}