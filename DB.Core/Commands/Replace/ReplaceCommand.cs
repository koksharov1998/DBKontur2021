using System.Collections.Concurrent;
using System.Collections.Generic;
using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Replace
{
    public class ReplaceCommand : ICommand
    {
        private readonly IReplaceCommandParser parser;

        public ReplaceCommand(IReplaceCommandParser parser)
            => this.parser = parser;

        public string Name => "replace";

        public JObject Execute(IDbState state, JObject parameters)
        {
            var (ok, collectionName, id, document, upsert) = parser.Parse(parameters);

            if (!ok)
                return Result.Error.InvalidRequest;

            var collection = state.Collections.GetOrAdd(collectionName, _ => new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>());

            if (!collection.ContainsKey(id) && !upsert)
                    return Result.Error.NotFound;

            if (collection.ContainsKey(id))
            {
                var deleted = collection[id];
                    foreach (var kvp in deleted)
                        if (state.Indexies.TryGetValue(collectionName, out var collectionIndexies) 
                        && collectionIndexies.TryGetValue(kvp.Key, out var indexFields)
                        && indexFields.TryGetValue(kvp.Value, out var list))
                                    list.Remove(id);
            }

            collection[id] = document.ToObject<ConcurrentDictionary<string, string>>();

            foreach (var kvp in collection[id])
                if (state.Indexies.TryGetValue(collectionName, out var collectionIndexies)
                    && collectionIndexies.TryGetValue(kvp.Key, out var indexFields))
                        indexFields.GetOrAdd(kvp.Value, new List<string>()).Add(id);

            return Result.Ok.Empty;
        }
    }
}