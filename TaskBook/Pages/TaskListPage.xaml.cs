using System;
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
            TaskLists = Global.GetAll();
            BindingContext = this;
        }

        async void TasklistCV_SelectionChanged(System.Object sender,
            Xamarin.Forms.SelectionChangedEventArgs e)
        {
            Models.TaskList selectedItem = e.CurrentSelection.FirstOrDefault()
                as Models.TaskList;
            if (selectedItem != null)
            {
                await Navigation.PushAsync(new DetailsPage(selectedItem));
            }
            TasklistCV.SelectedItem = null;
        }

        async void DeleteSwipeItem_Invoked(System.Object sender, System.EventArgs e)
        {
            var listToDelete = ((SwipeItem)sender).BindingContext
                as Models.TaskList;
            TaskLists.Remove(listToDelete);
            OnPropertyChanged("TaskLists");
            await Global.Delete(listToDelete);
            if (SearchBar.Text != null && SearchBar.Text.Length != 0)
            {
                TasklistCV.ItemsSource = Global.SearchLists(SearchBar.Text.Trim());
            }
        }

        async void CreateButton_Clicked(System.Object sender,
            System.EventArgs e)
        {
            string result = await DisplayPromptAsync("Task List Creator",
                "Enter a name for your new task list.");
            if (result != null && result != "")
            {
                var listToAdd = new Models.TaskList { Name = result.Trim() };
                TaskLists.Add(listToAdd);
                OnPropertyChanged("TaskLists");
                await Global.Save(listToAdd);
            }
            if (SearchBar.Text != null && SearchBar.Text.Length != 0)
            {
                TasklistCV.ItemsSource = Global.SearchLists(SearchBar.Text.Trim());
            }
        }

        void SearchBar_TextChanged(System.Object sender,
            Xamarin.Forms.TextChangedEventArgs e)
        {
            SearchBar searchBar = (SearchBar)sender;
            TasklistCV.ItemsSource = Global.SearchLists(searchBar.Text.Trim());
        }

        async void EditSwipeItem_Invoked(System.Object sender, System.EventArgs e)
        {
            var listToEdit = ((SwipeItem)sender).BindingContext
                as Models.TaskList;
            string result = await DisplayPromptAsync("Task List Creator",
                $"Enter a new name for your the tasklist: {listToEdit.Name}.");
            int idx = TaskLists.IndexOf(listToEdit);
            if (result != null && result != "")
            {
                listToEdit.Name = result.Trim();
                OnPropertyChanged("TaskLists");
                await Global.Save(listToEdit);
            }
            if (SearchBar.Text != null && SearchBar.Text.Length != 0)
            {
                TasklistCV.ItemsSource = Global.SearchLists(SearchBar.Text.Trim());
            }
        }
    }
}
