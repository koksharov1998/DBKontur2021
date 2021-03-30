using System.Collections.Concurrent;
using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Insert
{
    public class InsertCommand : ICommand
    {
        private readonly IInsertCommandParser parser;

        public InsertCommand(IInsertCommandParser parser)
            => this.parser = parser;

        public string Name => "insert";

        public JObject Execute(IDbState state, JObject parameters)
        {
            var (ok, collectionName, id, document) = parser.Parse(parameters);

            if (!ok)
            {
                return Result.Error.InvalidRequest;
            }

            var collection = state.Collections.GetOrAdd(collectionName, _ => new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>());

            if (collection.ContainsKey(id))
            {
                return Result.Error.AlreadyExists;
            }

            collection[id] = document.ToObject<ConcurrentDictionary<string, string>>();

            foreach (var kvp in collection[id])
                if (state.Indexies.TryGetValue(collectionName, out var collectionIndexies))
                    if (collectionIndexies.TryGetValue(kvp.Key, out var indexFields))
                        if (indexFields.TryGetValue(kvp.Value, out var list))
                            list.Add(id);

            return Result.Ok.Empty;
        }
    }
}