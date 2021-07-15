using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

namespace TaskBook
{
    public static class Global
    {
        public static ObservableCollection<Models.TaskList> GetAll()
        {
            var handler = new WebRequestHandler();
            string json = handler.Get(Constants.WebApi_GetAll).Result;
            List<Models.TaskList> jsonList;
            if (json != null)
            {
                jsonList = JsonConvert
                .DeserializeObject<List<Models.TaskList>>(json);
            }
            else
            {
                jsonList = new List<Models.TaskList>();
            }
            return new ObservableCollection<Models.TaskList>(jsonList);
        }

        public static async Task Save(Models.TaskList tasklist)
        {
            var handler = new WebRequestHandler();
            await handler.Post(Constants.WebApi_AddOrUpdate, tasklist);
        }

        public static async Task Delete(Models.TaskList tasklist)
        {
            var handler = new WebRequestHandler();
            await handler.Post(Constants.WebApi_Delete, tasklist.Id);
        }

        public static async void Save(Guid listId, Models.Item item)
        {
            var handler = new WebRequestHandler();
            string url = Constants.WebApi_Base + "/" + listId + "/AddOrUpdate";
            if (item is Models.Task)
            {
                url = url + "Task";
                await handler.Post(url, item as Models.Task);
            }
            else
            {
                url = url + "Appt";
                await handler.Post(url, item as Models.Appointment);
            }
        }

        public static async void Delete(Guid listId, Models.Item item)
        {
            var handler = new WebRequestHandler();
            string url = Constants.WebApi_Base + "/" + listId + "/Delete";
            if (item is Models.Task)
            {
                url = url + "Task";
                await handler.Post(url, item as Models.Task);
            }
            else
            {
                url = url + "Appt";
                await handler.Post(url, item as Models.Appointment);
            }
        }

        public static ObservableCollection<Models.TaskList> SearchLists(string name)
        {
            string url = Constants.WebApie_SearchLists + $"?name={name}";
            if (name.Trim().Length == 0)
            {
                return GetAll();
            }
            var handler = new WebRequestHandler();
            string json = handler.Get(url).Result;
            List<Models.TaskList> jsonList;
            if (json != null)
            {
                jsonList = JsonConvert.DeserializeObject<List<Models.TaskList>>(json);
            }
            else
            {
                jsonList = new List<Models.TaskList>();
            }
            return new ObservableCollection<Models.TaskList>(jsonList);
        }
    }
}
