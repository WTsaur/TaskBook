using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace TaskBook
{
    public static class Global
    {
        public static ObservableCollection<Models.TaskList> Data { get; set; }

        public static void Init()
        {
            if (Preferences.ContainsKey(Constants.DataKey))
            {
                string json = Preferences.Get(Constants.DataKey, "[]");
                List<Models.TaskList> jsonList = JsonConvert
                    .DeserializeObject<List<Models.TaskList>>(json);
                Data = new ObservableCollection<Models.TaskList>(jsonList);
            }
            else
            {
                Data = new ObservableCollection<Models.TaskList>();
            }
        }

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(Data);
            Preferences.Set(Constants.DataKey, json);
        }
    }
}
