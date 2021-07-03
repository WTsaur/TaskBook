using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Linq;

namespace TaskBook.Pages
{
    public partial class TaskListPage : ContentPage
    {
        public ObservableCollection<Models.TaskList> TaskLists { get; set; }

        public TaskListPage()
        {
            InitializeComponent();
            TaskLists = new ObservableCollection<Models.TaskList> { new Models.TaskList { Name = "Test", Items = new List<Models.Item> { new Models.Task { Name = "task 1" }, new Models.Appointment { Name = "appointment 1" } } } };
            BindingContext = this;
        }

        async void TasklistCV_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            var selectedItem = e.CurrentSelection.FirstOrDefault() as Models.TaskList;
            if (selectedItem != null)
            {
                await Navigation.PushAsync(new DetailsPage(selectedItem));
            }
            TasklistCV.SelectedItem = null;
        }

        void DeleteSwipeItem_Invoked(System.Object sender, System.EventArgs e)
        {
            var itemToDelete = ((SwipeItem)sender).BindingContext as Models.TaskList;
            OnPropertyChanged("TaskLists");
            TaskLists.Remove(itemToDelete);
            if (SearchBar.Text != null && SearchBar.Text.Length != 0)
            {
                TasklistCV.ItemsSource = SearchFor(SearchBar.Text);
            }
        }

        async void CreateButton_Clicked(System.Object sender, System.EventArgs e)
        {
            string result = await DisplayPromptAsync("Task List Creator", "Enter a name for your new task list.");
            if (result != null && result != "")
            {
                OnPropertyChanged("TaskLists");
                TaskLists.Add(new Models.TaskList { Name = result.Trim() });
            }
        }

        void SearchBar_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            SearchBar searchBar = (SearchBar)sender;
            TasklistCV.ItemsSource = SearchFor(searchBar.Text);
        }

        public ObservableCollection<Models.TaskList> SearchFor(String str)
        {
            if (str.Trim().Length == 0)
            {
                return TaskLists;
            }
            var results = from list in TaskLists
                          where list.Name.ToLower().Contains(str.ToLower())
                          select list;
            ObservableCollection<Models.TaskList> filteredItems = new ObservableCollection<Models.TaskList>(results.ToList());
            return filteredItems;
        }
    }
}
