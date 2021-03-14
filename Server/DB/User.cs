using System;
using System.Collections.Generic;


namespace HW5.DB
{
    public class User
    {
        public int UserId { get; set; }
        
        public string Name { get; set; }
        
        public string Email { get; set;}
        
        public string Login { get; set; }
        
        public string PasswordHash { get; set; }
        
        public List<Product> UserProducts { get; set; }
    }
}