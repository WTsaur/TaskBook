using System;

namespace TaskBook.Models
{
    public class Item
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Priority { get; set; } = "low";
        public Guid Id { get; set; }
    }
}
