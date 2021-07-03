using System;

namespace TaskBook.Models
{
    public class Task : Item
    {
        public DateTime Deadline { get; set; } = DateTime.Today;
        public bool IsCompleted { get; set; } = false;

        public override string ToString()
        {
            string status = (this.IsCompleted) ?
                " (Completed)" : " (Incomplete)";
            return $"{this.Name} {status}" +
                $"\n\tPriority: {Priority}" +
                $"\n\tDescription: {Description}" +
                $"\n\tDeadline: {Deadline}";
        }
    }
}
