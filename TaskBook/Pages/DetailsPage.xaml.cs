using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace TaskBook.Pages
{
    public partial class DetailsPage : ContentPage
    {
        public Models.TaskList List { get; set; }
        public ObservableCollection<string> AttendeeList { get; set; }

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

        void SearchEntry_Completed(System.Object sender, System.EventArgs e)
        {

        }

        void DeleteSwipeItem_Invoked(System.Object sender, System.EventArgs e)
        {
            var itemToDelete = ((SwipeItem)sender).BindingContext as Models.Item;
            OnPropertyChanged("List");
            List.RemoveItem(itemToDelete);
            if (itemToDelete is Models.Task)
            {
                TaskCV.ItemsSource = List.Items.Where(item => item is Models.Task);
            }
            else
            {
                AppointmentCV.ItemsSource = List.Items.Where(item => item is Models.Appointment);
            }
        }

        async void AddButton_Clicked(System.Object sender, System.EventArgs e)
        {
            string action = await DisplayActionSheet("Pick an item to create", "Cancel", null, "Task", "Appointment");
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
            if (ApptName.Text == null)
            {
                await DisplayAlert("Uh oh!", "Please enter a name for the task", "OK");
            }
            else
            {
                appointment.Name = ApptName.Text.Trim();
            }
            appointment.Description = ApptDescription.Text?.Trim() ?? "";
            appointment.Priority = ApptPriorityPicker.SelectedItem?.ToString() ?? "Low";
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
            List.AddItem(appointment);
            AppointmentCV.ItemsSource = List.Items.Where(item => item is Models.Appointment);
            ClearApptInput();
            ApptFrame.IsVisible = false;
        }

        async void CreateTaskButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Models.Task task = new Models.Task();
            if (TaskName.Text == null)
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
            DateTime date = TaskDatePicker.Date;
            TimeSpan time = TaskTimePicker.Time;
            DateTime deadline = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
            task.Deadline = deadline;
            OnPropertyChanged("List");
            List.AddItem(task);
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
            TaskName.Text = "";
            TaskDescription.Text = "";
            TaskPriorityPicker.SelectedIndex = -1;
            TaskDatePicker.Date = DateTime.Today;
            TaskTimePicker.Time = DateTime.Today.TimeOfDay;
        }

        void ClearApptInput()
        {
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
    }
}
