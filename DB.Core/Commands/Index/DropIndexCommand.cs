using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DB.Core.Commands.DropIndex
{
    public class DropIndexCommand : ICommand
    {
        public string Name => "dropIndex";

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

            if (!state.Indexies.TryGetValue(collectionName, out var collectionIndexies))
                return Result.Error.NotFound;

            if (!collectionIndexies.ContainsKey(key))
                return Result.Error.NotFound;

            collectionIndexies.TryRemove(key, out _);

            return Result.Ok.Empty;
        }
    }
}
