using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace HW5.DB
{
    public class Category
    {
        public int CategoryId { get; set; }
        
        public string CategoryName { get; set; }
        
        public List<Product> CategoryProducts { get; set; }
    }
}