using System.Collections.Generic;

namespace Server
{
    internal class KeyValueStore
    {
        private Dictionary<string, string> _data;

        public KeyValueStore()
        {
            _data = new Dictionary<string, string>();
        }

        private string Get(string key)
        {
            return _data[key];
        }

        private void Set(string key, string value)
        {
            _data.Add(key, value);
        }

        private void Delete(string key)
        {
            _data.Remove(key);
        }

        public string Execute(string operation)
        {
            if (string.IsNullOrWhiteSpace(operation))
            {
                return operation;
            }
            var response = "Sorry, I don't understand that command";
            var dataArr = operation.Split(" ");
            var command = dataArr[0];

            switch (command)
            {
                case "set":
                    Set(dataArr[1], dataArr[2]);
                    response = $"key {dataArr[1]} set to {_data[dataArr[1]]}";
                    break;
                case "get":
                    response = Get(dataArr[1]);
                    break;
                case "delete":
                    Delete(dataArr[1]);
                    response = $"key {dataArr[1]} deleted";
                    break;
                case "show":
                    response = string.Empty;
                    foreach (var key in _data.Keys)
                    {
                        response += $"{key}->{_data[key]}/n";
                    }

                    if (string.IsNullOrWhiteSpace(response))
                    {
                        response = "Nothing to show";
                    }
                    break;
            }

            return response;
        }
    }
}
