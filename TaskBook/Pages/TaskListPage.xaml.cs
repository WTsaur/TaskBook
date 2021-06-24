using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Linq;

namespace TaskBook.Pages
{
    public partial class TaskListPage : ContentPage
    {
        public ObservableCollection<Models.TaskList> TaskLists;
        public TaskListPage()
        {
            InitializeComponent();
            TaskLists = new ObservableCollection<Models.TaskList> { new Models.TaskList { Name = "Test" } };
            BindingContext = this;
        }

        void SearchEntry_Completed(System.Object sender, System.EventArgs e)
        {

        }

        async void TasklistCV_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            var selectedItem = e.CurrentSelection.FirstOrDefault() as Models.TaskList;
            if (selectedItem != null)
            {
                await Navigation.PushAsync(new DetailsPage(selectedItem));
            }
        }

        void DeleteSwipeItem_Invoked(System.Object sender, System.EventArgs e)
        {
            var itemToDelete = ((SwipeItem)sender).BindingContext as Models.TaskList;
            OnPropertyChanged("TaskLists");
            TaskLists.Remove(itemToDelete);
        }

        async void CreateButton_Clicked(System.Object sender, System.EventArgs e)
        {
            string result = await DisplayPromptAsync("Task List Creator", "Enter a name for your new task list.");
            if (result != "")
            {
                OnPropertyChanged("TaskLists");
                TaskLists.Add(new Models.TaskList { Name = result });
            }
        }
    }
}
