namespace HW5.DB
{
    public class Subscribe
    {
        public int Id { get; set; }
        
        public User User { get; set; }
        
        public User Subscriber { get; set; }
    }
}