using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace TaskBook.Pages
{
    public partial class DetailsPage : ContentPage
    {
        public Models.TaskList List { get; set; }
        public ObservableCollection<string> AttendeeList { get; set; }
        private Models.Item BeingEdited = null;

        public DetailsPage(Models.TaskList list)
        {
            InitializeComponent();
            List = list;
            ListName.Text = list.Name;
            TaskCV.ItemsSource = List.Items.Where(item => item is Models.Task);
            AppointmentCV.ItemsSource = List.Items.Where(item => item is Models.Appointment);
            AttendeeList = new ObservableCollection<string>();
            BindingContext = this;
        }

        void SearchBar_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            SearchBar searchBar = (SearchBar)sender;
            Models.TaskList filteredList = new Models.TaskList();
            if (searchBar.Text == null || searchBar.Text.Length == 0)
            {
                TaskCV.ItemsSource = List.Items.Where(item => item is Models.Task);
                AppointmentCV.ItemsSource = List.Items.Where(item => item is Models.Appointment);
            }
            if (FilterPicker.SelectedIndex > -1)
            {
                filteredList.Items = List.FilterByPriority(FilterPicker.SelectedItem.ToString());
                TaskCV.ItemsSource = filteredList.SearchFor(searchBar.Text).Where(item => item is Models.Task);
                AppointmentCV.ItemsSource = filteredList.SearchFor(searchBar.Text).Where(item => item is Models.Appointment);
            }
            else
            {
                TaskCV.ItemsSource = List.SearchFor(searchBar.Text).Where(item => item is Models.Task);
                AppointmentCV.ItemsSource = List.SearchFor(searchBar.Text).Where(item => item is Models.Appointment);
            }
        }

        void FilterPicker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if (FilterPicker.SelectedIndex == -1) return;
            string priority = FilterPicker.SelectedItem.ToString();
            Models.TaskList filteredList = new Models.TaskList();
            if (FilterPicker.SelectedIndex == 0)
            {
                FilterPicker.SelectedIndex = -1;
            }
            if (SearchBar.Text != null)
            {
                filteredList.Items = List.SearchFor(SearchBar.Text);
                TaskCV.ItemsSource = filteredList.FilterByPriority(priority).Where(item => item is Models.Task);
                AppointmentCV.ItemsSource = filteredList.FilterByPriority(priority).Where(item => item is Models.Appointment);
            }
            else
            {
                TaskCV.ItemsSource = List.FilterByPriority(priority).Where(item => item is Models.Task);
                AppointmentCV.ItemsSource = List.FilterByPriority(priority).Where(item => item is Models.Appointment);
            }
        }

        void EditSwipeItem_Invoked(System.Object sender, System.EventArgs e)
        {
            var itemToEdit = ((SwipeItem)sender).BindingContext as Models.Item;
            if (itemToEdit is Models.Task)
            {
                EditTask(itemToEdit as Models.Task);
            }
            else
            {
                EditAppointment(itemToEdit as Models.Appointment);
            }
        }

        void DeleteSwipeItem_Invoked(System.Object sender, System.EventArgs e)
        {
            var itemToDelete = ((SwipeItem)sender).BindingContext as Models.Item;
            OnPropertyChanged("List");
            List.RemoveItem(itemToDelete);
            if (itemToDelete is Models.Task)
            {
                if (SearchBar.Text == null || SearchBar.Text.Length == 0)
                {
                    TaskCV.ItemsSource = List.Items.Where(item => item is Models.Task);
                }
                else
                {
                    TaskCV.ItemsSource = List.SearchFor(SearchBar.Text).Where(item => item is Models.Task);
                }
            }
            else
            {
                if (SearchBar.Text == null || SearchBar.Text.Length == 0)
                {
                    AppointmentCV.ItemsSource = List.Items.Where(item => item is Models.Appointment);
                }
                else
                {
                    AppointmentCV.ItemsSource = List.SearchFor(SearchBar.Text).Where(item => item is Models.Appointment);
                }
            }
        }

        async void AddButton_Clicked(System.Object sender, System.EventArgs e)
        {
            string action = await DisplayActionSheet("Pick an item to create", "Cancel", null, "Task", "Appointment");
            if (action != "Cancel")
            {
                FilterPicker.SelectedIndex = -1;
                SearchBar.Text = "";
            }
            if (action == "Task")
            {
                ApptFrame.IsVisible = false;
                TaskFrame.IsVisible = true;
            }
            else if (action == "Appointment")
            {
                TaskFrame.IsVisible = false;
                ApptFrame.IsVisible = true;
            }
        }

        async void CreateApptButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Models.Appointment appointment = new Models.Appointment();
            if (BeingEdited != null)
            {
                if (BeingEdited is Models.Appointment)
                {
                    appointment = BeingEdited as Models.Appointment;
                }
            }
            if (ApptName.Text == null || ApptName.Text.Trim().Length == 0)
            {
                await DisplayAlert("Uh oh!", "Please enter a name for the task", "OK");
                return;
            }
            else
            {
                appointment.Name = ApptName.Text.Trim();
            }
            appointment.Description = ApptDescription.Text?.Trim() ?? "";
            appointment.Priority = ApptPriorityPicker.SelectedItem?.ToString() ?? "Low";
            appointment.BgColor = appointment.Priority.Equals("High") ? Constants.ItemHighPriColor : Constants.ItemLowPriColor;
            DateTime startDate = ApptStartDatePicker.Date;
            TimeSpan startTime = ApptStartTimePicker.Time;
            DateTime startDateTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, startTime.Hours, startTime.Minutes, startTime.Seconds);
            DateTime endDate = ApptEndDatePicker.Date;
            TimeSpan endTime = ApptEndTimePicker.Time;
            DateTime endDateTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, endTime.Hours, endTime.Minutes, endTime.Seconds);
            appointment.Start = startDateTime;
            appointment.Stop = endDateTime;
            appointment.Attendees = AttendeeList.ToList();
            OnPropertyChanged("List");
            if (BeingEdited == null)
            {
                List.AddItem(appointment);
            }
            else
            {
                BeingEdited = null;
            }
            AppointmentCV.ItemsSource = List.Items.Where(item => item is Models.Appointment);
            ClearApptInput();
            ApptFrame.IsVisible = false;
        }

        async void CreateTaskButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Models.Task task = new Models.Task();
            if (BeingEdited != null)
            {
                if (BeingEdited is Models.Task)
                {
                    task = BeingEdited as Models.Task;
                }
            }
            if (TaskName.Text == null || TaskName.Text.Trim().Length == 0)
            {
                await DisplayAlert("Uh oh!", "Please enter a name for the task", "OK");
                return;
            }
            else
            {
                task.Name = TaskName.Text.Trim();
            }
            task.Description = TaskDescription.Text?.Trim() ?? "";
            task.Priority = TaskPriorityPicker.SelectedItem?.ToString() ?? "Low";
            task.BgColor = task.Priority.Equals("High") ? Constants.ItemHighPriColor : Constants.ItemLowPriColor;
            DateTime date = TaskDatePicker.Date;
            TimeSpan time = TaskTimePicker.Time;
            DateTime deadline = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
            task.Deadline = deadline;
            OnPropertyChanged("List");
            if (BeingEdited == null)
            {
                List.AddItem(task);
            }
            else
            {
                BeingEdited = null;
            }
            TaskCV.ItemsSource = List.Items.Where(item => item is Models.Task);
            ClearTaskInput();
            TaskFrame.IsVisible = false;
        }

        void DismissApptButton_Clicked(System.Object sender, System.EventArgs e)
        {
            ApptFrame.IsVisible = false;
            ClearApptInput();
        }

        void DismissTaskButton_Clicked(System.Object sender, System.EventArgs e)
        {
            TaskFrame.IsVisible = false;
            ClearTaskInput();
        }

        void AttendeeEntry_Completed(System.Object sender, System.EventArgs e)
        {
            string name = AttendeeEntry.Text;
            if (name.Contains(','))
            {
                string[] nameList = name.Split(',');
                foreach(string n in nameList)
                {
                    string trimmedStr = n.Trim();
                    OnPropertyChanged("AttendeeList");
                    AttendeeList.Add(trimmedStr);
                }
            }
            else
            {
                OnPropertyChanged("AttendeeList");
                AttendeeList.Add(name.Trim());
            }
            AttendeeEntry.Text = "";
        }

        void Attendees_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            var selectedName = e.CurrentSelection.FirstOrDefault() as string;
            if (selectedName != null)
            {
                OnPropertyChanged("AttendeeList");
                AttendeeList.Remove(selectedName);
            }
            AttendeeCV.SelectedItem = null;
        }

        void ClearTaskInput()
        {
            TaskCreatorLabel.Text = "Task Creator";
            TaskName.Text = "";
            TaskDescription.Text = "";
            TaskPriorityPicker.SelectedIndex = -1;
            TaskDatePicker.Date = DateTime.Today;
            TaskTimePicker.Time = DateTime.Today.TimeOfDay;
        }

        void ClearApptInput()
        {
            ApptCreatorLabel.Text = "Appointment Creator";
            ApptName.Text = "";
            ApptDescription.Text = "";
            ApptPriorityPicker.SelectedIndex = -1;
            ApptStartDatePicker.Date = DateTime.Today;
            ApptStartTimePicker.Time = DateTime.Today.TimeOfDay;
            ApptEndDatePicker.Date = DateTime.Today;
            ApptEndTimePicker.Time = DateTime.Today.TimeOfDay;
            OnPropertyChanged("AttendeeList");
            AttendeeList.Clear();
        }

        void EditTask(Models.Task task)
        {
            BeingEdited = task;
            TaskCreatorLabel.Text = "Task Editor";
            TaskFrame.IsVisible = true;
            TaskName.Text = task.Name;
            TaskDescription.Text = task.Description;
            TaskPriorityPicker.SelectedItem = task.Priority;
            TaskDatePicker.Date = task.Deadline.Date;
            TaskTimePicker.Time = task.Deadline.TimeOfDay;
        }

        void EditAppointment(Models.Appointment appt)
        {
            BeingEdited = appt;
            ApptCreatorLabel.Text = "Appointment Editor";
            ApptFrame.IsVisible = true;
            ApptName.Text = appt.Name;
            ApptDescription.Text = appt.Description;
            ApptPriorityPicker.SelectedItem = appt.Priority;
            ApptStartDatePicker.Date = appt.Start.Date;
            ApptStartTimePicker.Time = appt.Start.TimeOfDay;
            ApptEndDatePicker.Date = appt.Stop.Date;
            ApptEndTimePicker.Time = appt.Stop.TimeOfDay;
            foreach (string name in appt.Attendees)
            {
                OnPropertyChanged("AttendeeList");
                AttendeeList.Add(name);
            }
        }

        async void DetailButton_Clicked(System.Object sender, System.EventArgs e)
        {
            var item = ((Button)sender).BindingContext as Models.Item;
            if (item != null)
            {
                await DisplayAlert("", item.ToString(), "OK");
            }
        }
    }
}
