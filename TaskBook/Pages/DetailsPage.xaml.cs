using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace TaskBook.Pages
{
    public partial class DetailsPage : ContentPage
    {

        public Models.TaskList List;

        public DetailsPage(Models.TaskList list)
        {
            InitializeComponent();
            List = list;
            TaskCV.ItemsSource = List.Items.Where(item => item is Models.Task);
            AppointmentCV.ItemsSource = List.Items.Where(item => item is Models.Appointment);
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

        async void CreateButton_Clicked(System.Object sender, System.EventArgs e)
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

        void CreateApptButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Models.Appointment appointment = new Models.Appointment();
            appointment.Name = ApptName.Text;
            appointment.Description = ApptDescription.Text;
            appointment.Priority = ApptPriorityPicker.SelectedItem.ToString();
            DateTime startDate = ApptStartDatePicker.Date;
            TimeSpan startTime = ApptStartTimePicker.Time;
            DateTime startDateTime = new DateTime(startDate.Year, startDate.Month, startDate.Day, startTime.Hours, startTime.Minutes, startTime.Seconds);
            DateTime endDate = ApptEndDatePicker.Date;
            TimeSpan endTime = ApptEndTimePicker.Time;
            DateTime endDateTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, endTime.Hours, endTime.Minutes, endTime.Seconds);
            appointment.Start = startDateTime;
            appointment.Stop = endDateTime;
            //attendees
            ApptFrame.IsVisible = false;
        }
    }
}
