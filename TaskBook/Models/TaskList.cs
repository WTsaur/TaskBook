using System.Collections.ObjectModel;

namespace TaskBook.Models
{
    public class TaskList
    {
        public string Name { get; set; } = "";
        public ObservableCollection<Task> Tasks { get; set; } = new ObservableCollection<Task>();
        public ObservableCollection<Appointment> Appointments { get; set; } = new ObservableCollection<Appointment>();

        public override string ToString()
        {
            return Name;
        }
    }
}
