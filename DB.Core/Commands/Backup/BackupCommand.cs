using DB.Core.State;
using Newtonsoft.Json.Linq;
using System.Linq;
using DB.Core.Helpers;
using System.Collections.Concurrent;

namespace DB.Core.Commands.Backup
{
    public class BackupCommand : ICommand
    {
        public string Name => "backup";

        public JObject Execute(IDbState state, JObject parameters)
        {
            if (parameters.Count != 0)
                return Result.Error.InvalidRequest;

            return Result.Ok.WithContent(new JObject(state.Collections.Select(collection => new JProperty(collection.Key, collection.Value.Select(kvp => GetJObject(kvp.Key, kvp.Value))))));
        }
        
        private static JObject GetJObject(string id, ConcurrentDictionary<string, string> document)
    => new(
        new JProperty(id,
            new JObject(document.Select(kvp => new JProperty(kvp.Key, kvp.Value)))));
        
    }
}
