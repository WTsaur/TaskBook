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
            ListName.Text = List.Name;
            TaskCV.ItemsSource = List.Tasks;
            AppointmentCV.ItemsSource = List.Appointments;
            AttendeeList = new ObservableCollection<string>();
            BindingContext = this;
        }

        void SearchBar_TextChanged(System.Object sender,
            Xamarin.Forms.TextChangedEventArgs e)
        {
            SearchAndFilter();
        }

        void FilterPicker_SelectedIndexChanged(System.Object sender,
            System.EventArgs e)
        {
            if (FilterPicker.SelectedIndex == -1) return;
            if (FilterPicker.SelectedIndex == 0)
            {
                FilterPicker.SelectedIndex = -1;
            }
            SearchAndFilter();
        }

        void SearchAndFilter()
        {
            bool doSearch = SearchBar.Text != null && SearchBar.Text.Length > 0;
            bool doFilter = FilterPicker.SelectedIndex > 0;
            if (doSearch && doFilter)
            {
                string lowerText = SearchBar.Text.ToLower();
                string priority = FilterPicker.SelectedItem.ToString();
                TaskCV.ItemsSource = List.Tasks.Where(task =>
                    task.Priority == priority &&
                    (task.Name.ToLower().Contains(lowerText) ||
                    task.Description.ToLower().Contains(lowerText)));
                AppointmentCV.ItemsSource = List.Appointments.Where(appt =>
                    appt.Priority == priority &&
                    (appt.Name.ToLower().Contains(lowerText) ||
                    appt.Description.ToLower().Contains(lowerText) ||
                    appt.Attendees.Contains(SearchBar.Text)));
            }
            else if (doSearch && !doFilter)
            {
                string lowerText = SearchBar.Text.ToLower();
                TaskCV.ItemsSource = List.Tasks.Where(task =>
                    task.Name.ToLower().Contains(lowerText) ||
                    task.Description.ToLower().Contains(lowerText));
                AppointmentCV.ItemsSource = List.Appointments.Where(appt =>
                    appt.Name.ToLower().Contains(lowerText) ||
                    appt.Description.ToLower().Contains(lowerText) ||
                    appt.Attendees.Contains(SearchBar.Text));
            }
            else if (!doSearch && doFilter)
            {
                string priority = FilterPicker.SelectedItem.ToString();
                TaskCV.ItemsSource = List.Tasks.Where(task =>
                    task.Priority == priority);
                AppointmentCV.ItemsSource = List.Appointments.Where(appt =>
                    appt.Priority == priority);
            }
            else
            {
                TaskCV.ItemsSource = List.Tasks;
                AppointmentCV.ItemsSource = List.Appointments;
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
            var itemToDelete = ((SwipeItem)sender)
                .BindingContext as Models.Item;
            if (itemToDelete is Models.Task)
            {
                OnPropertyChanged("Tasks");
                List.Tasks.Remove(itemToDelete as Models.Task);
            }
            else
            {
                OnPropertyChanged("Appointments");
                List.Appointments.Remove(itemToDelete as Models.Appointment);
            }
            Global.Save();
            SearchAndFilter();
        }

        async void AddButton_Clicked(System.Object sender, System.EventArgs e)
        {
            string action = await DisplayActionSheet("Pick an item to create",
                "Cancel", null, "Task", "Appointment");
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

        async void CreateApptButton_Clicked(System.Object sender,
            System.EventArgs e)
        {
            Models.Appointment appointment = new();
            int idx = 0;
            if (BeingEdited != null)
            {
                if (BeingEdited is Models.Appointment)
                {
                    appointment = BeingEdited as Models.Appointment;
                    idx = List.Appointments.IndexOf(appointment);
                }
            }
            if (ApptName.Text == null || ApptName.Text.Trim().Length == 0)
            {
                await DisplayAlert("Uh oh!", "Please enter a name for the task",
                    "OK");
                return;
            }
            else
            {
                appointment.Name = ApptName.Text.Trim();
            }
            appointment.Description = ApptDescription.Text?.Trim() ?? "";
            appointment.Priority = ApptPriorityPicker
                .SelectedItem?.ToString() ?? "Low";
            appointment.BgColor = appointment.Priority.Equals("High") ?
                Constants.ItemHighPriColor : Constants.ItemLowPriColor;
            DateTime startDate = ApptStartDatePicker.Date;
            TimeSpan startTime = ApptStartTimePicker.Time;
            DateTime startDateTime = new(startDate.Year,
                startDate.Month, startDate.Day, startTime.Hours,
                startTime.Minutes, startTime.Seconds);
            DateTime endDate = ApptEndDatePicker.Date;
            TimeSpan endTime = ApptEndTimePicker.Time;
            DateTime endDateTime = new(endDate.Year, endDate.Month,
                endDate.Day, endTime.Hours, endTime.Minutes, endTime.Seconds);
            appointment.Start = startDateTime;
            appointment.Stop = endDateTime;
            appointment.Attendees = AttendeeList.ToList();
            OnPropertyChanged("Appointments");
            if (BeingEdited == null)
            {
                List.Appointments.Add(appointment);
            }
            else
            {
                List.Appointments.RemoveAt(idx);
                OnPropertyChanged("Appointments");
                List.Appointments.Insert(idx, appointment);
                BeingEdited = null;
            }
            Global.Save();
            SearchAndFilter();
            ClearApptInput();
            ApptFrame.IsVisible = false;
        }

        async void CreateTaskButton_Clicked(System.Object sender,
            System.EventArgs e)
        {
            Models.Task task = new();
            int idx = 0;
            if (BeingEdited != null)
            {
                if (BeingEdited is Models.Task)
                {
                    task = BeingEdited as Models.Task;
                    idx = List.Tasks.IndexOf(task);
                }
            }
            if (TaskName.Text == null || TaskName.Text.Trim().Length == 0)
            {
                await DisplayAlert("Uh oh!", "Please enter a name for the task",
                    "OK");
                return;
            }
            else
            {
                task.Name = TaskName.Text.Trim();
            }
            task.Description = TaskDescription.Text?.Trim() ?? "";
            task.Priority = TaskPriorityPicker.SelectedItem?.ToString() ??
                "Low";
            task.BgColor = task.Priority.Equals("High") ?
                Constants.ItemHighPriColor : Constants.ItemLowPriColor;
            DateTime date = TaskDatePicker.Date;
            TimeSpan time = TaskTimePicker.Time;
            DateTime deadline = new(date.Year, date.Month,
                date.Day, time.Hours, time.Minutes, time.Seconds);
            task.Deadline = deadline;
            OnPropertyChanged("Tasks");
            if (BeingEdited == null)
            {
                List.Tasks.Add(task);
            }
            else
            {
                List.Tasks.RemoveAt(idx);
                OnPropertyChanged("Tasks");
                List.Tasks.Insert(idx, task);
                BeingEdited = null;
            }
            Global.Save();
            SearchAndFilter();
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

        void Attendees_SelectionChanged(System.Object sender,
            Xamarin.Forms.SelectionChangedEventArgs e)
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

        async void DetailButton_Clicked(System.Object sender,
            System.EventArgs e)
        {
            var item = ((Button)sender).BindingContext as Models.Item;
            if (item != null)
            {
                await DisplayAlert("", item.ToString(), "OK");
            }
        }

        void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            Global.Save();
        }
    }
}
