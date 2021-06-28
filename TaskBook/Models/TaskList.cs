using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TaskBook.Models
{
    public class TaskList
    {
        public string Name { get; set; } = "";
        public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();
        public Guid Id { get; set; } = new Guid();

        public void AddItem(Item item) => Items.Add(item);

        public void RemoveItem(int idx)
        {
            if (idx >= 0 && idx < Items.Count)
            {
                Items.RemoveAt(idx);
            }
        }

        public bool RemoveItem(Item item) => Items.Remove(item);

        public ObservableCollection<Item> SearchFor(String str)
        {
            var results = from item in Items
                          where item.Name.ToLower().Contains(str) || item.Description.ToLower().Contains(str)
                          || ((item is Appointment) && ((Appointment)item).Attendees.Contains(str))
                          select item;
            ObservableCollection<Item> filteredItems = new ObservableCollection<Item>(results.ToList());
            return filteredItems;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
