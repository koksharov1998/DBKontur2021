using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DB.Core.Commands.AddIndex
{
    public class AddIndexCommand : ICommand
    {
        public string Name => "addIndex";

        public JObject Execute(IDbState state, JObject parameters)
        {
            if (parameters.Count != 1)
                return Result.Error.InvalidRequest;

            var collectionProperty = parameters.Properties().First();
            var collectionName = collectionProperty.Name;

            if (collectionProperty.Count != 1)
                return Result.Error.InvalidRequest;

            var key = parameters.ToObject<string>();

            var collectionIndexies = state.Indexies.GetOrAdd(collectionName, _ => new ConcurrentDictionary<string, ConcurrentDictionary<string, List<int>>>());

            if (collectionIndexies.ContainsKey(key))
                return Result.Error.AlreadyExists;

            var indexKeys = collectionIndexies.GetOrAdd(key, _ => new ConcurrentDictionary<string, List<int>>());

            var collection = state.Collections.GetOrAdd(collectionName, _ => new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>());

            // заполнить indexKeys[возможное значение ключа] = "список айдишников строк, где есть такое значение ключа"
            foreach (var idAndDocument in collection)
                indexKeys[document.key].Add(document.id);

            // state.Collections.Select(collection => new JProperty(collection.Key, collection.Value.Select(kvp => GetJObject(kvp.Key, kvp.Value))))



            return Result.Ok.Empty;
        }

        private static JObject GetJObject(string id, ConcurrentDictionary<string, string> document)
    => new(
        new JProperty(id,
            new JObject(document.Select(kvp => new JProperty(kvp.Key, kvp.Value)))));
    }
}
