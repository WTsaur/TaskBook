using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        public override string ToString()
        {
            return Name;
        }
    }
}
