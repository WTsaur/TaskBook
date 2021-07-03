using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskBook.Models
{
    public class TaskList
    {
        public Guid ID { get; set; } = new Guid();
        public string Name { get; set; } = "";
        public List<Item> Items { get; set; } = new List<Item>();

        public void AddItem(Item item) => Items.Add(item);

        public void RemoveItem(int idx)
        {
            if (idx >= 0 && idx < Items.Count)
            {
                Items.RemoveAt(idx);
            }
        }

        public bool RemoveItem(Item item) => Items.Remove(item);

        public List<Item> SearchFor(String str)
        {
            if (str.Length == 0)
            {
                return Items;
            }
            var results = from item in Items
                          where item.Name.ToLower().Contains(str.ToLower())
                          || item.Description.ToLower().Contains(str.ToLower())
                          || ((item is Appointment) && ((Appointment)item).Attendees.Contains(str))
                          select item;
            List<Item> filteredItems = new List<Item>(results.ToList());
            return filteredItems;
        }

        public List<Item> FilterByPriority(String str)
        {
            string priority = str.ToLower();
            if (priority.Equals("none"))
            {
                return Items;
            }
            var results = from item in Items
                          where item.Priority.ToLower() == priority
                          select item;
            List<Item> filteredItems = new List<Item>(results.ToList());
            return filteredItems;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
