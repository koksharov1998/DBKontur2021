using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DB.Core.Commands.Update
{
    public class UpdateCommand : ICommand
    {
        public string Name => "update";

        public JObject Execute(IDbState state, JObject parameters)
        {
            if (parameters.Count != 1)
                return Result.Error.InvalidRequest;

            var collectionProperty = parameters.Properties().First();
            var collectionName = collectionProperty.Name;

            if (collectionProperty.Count != 1)
                return Result.Error.InvalidRequest;

            if (collectionProperty.First() is not JObject idAndOperations)
                return Result.Error.InvalidRequest;

            var idProperty = idAndOperations.Properties().First();
            var id = idProperty.Name;

            var collection = state.Collections.GetOrAdd(collectionName, _ => new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>());


            if (idProperty.First() is not JArray operations)
                return Result.Error.InvalidRequest;

            //if (!collection.ContainsKey(id))
              //  return Result.Error.NotFound;

            //if (operations.Children<JObject>().Properties().Select(x => x.Name != "set" && x.Name != "unset").Count() != 0)
            //  throw new Exception("dsf");
            //return Result.Error.InvalidRequest;




            foreach (JObject obj in operations.Children<JObject>())
            {
                if (obj.Properties().First().First() is not JObject objProp)
                    return Result.Error.InvalidRequest;
                // var objProp = obj.Properties().First();
                switch (objProp.Name)
                {
                    case "set":
                        if (!collection.ContainsKey(id))
                            return Result.Error.NotFound;
                        break;
                    case "unset":
                        if (!collection.ContainsKey(id))
                            return Result.Error.NotFound;
                        break;
                    default:
                        return Result.Error.InvalidRequest;
                        break;
                }
            }

            foreach (JObject obj in operations.Children<JObject>())
            {
                var objProp = obj.Properties().First();
                switch (objProp.Name)
                {
                    case "set":
                        var kvp = objProp.Value.ToObject<JObject>().Properties().First();
                        collection[id][kvp.Name] = kvp.Value.ToObject<string>();
                        break;
                    case "unset":
                        collection[id].TryRemove(objProp.Value.ToObject<string>(), out _);
                        break;
                    default:
                        return Result.Error.InvalidRequest;
                        break;
                }
            }

            return Result.Ok.Empty;
        }
    }
}
