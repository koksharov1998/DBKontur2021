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

            if (collectionProperty.Value.Type != JTokenType.String)
                return Result.Error.InvalidRequest;

            var key = collectionProperty.Value.ToObject<string>();

            var collectionIndexies = state.Indexies.GetOrAdd(collectionName, _ => new ConcurrentDictionary<string, ConcurrentDictionary<string, List<string>>>());

            if (collectionIndexies.ContainsKey(key))
                return Result.Error.AlreadyExists;

            var indexKeys = collectionIndexies.GetOrAdd(key, _ => new ConcurrentDictionary<string, List<string>>());

            var collection = state.Collections.GetOrAdd(collectionName, _ => new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>());

            // заполнить indexKeys[возможное значение ключа] = "список айдишников, где есть такое значение ключа"
            foreach (var idAndDocument in collection)
            {
                var document = idAndDocument.Value;
                if (document.TryGetValue(key, out var value))
                    indexKeys.AddOrUpdate(value, new List<string> { idAndDocument.Key }, (key, list) =>
                    {
                        list.Add(idAndDocument.Key);
                        return list;
                    });
            }

            return Result.Ok.Empty;
        }
    }
}
