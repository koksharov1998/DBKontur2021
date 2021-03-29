using System;
using System.Collections.Concurrent;
using System.Linq;
using DB.Core.Helpers;
using DB.Core.Validation;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Restore
{
    public class RestoreCommandParser : IRestoreCommandParser
    {
        private readonly IDocumentValidator validator;

        public RestoreCommandParser(IDocumentValidator validator)
            => this.validator = validator;

        public (bool Ok, ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>> Backup) Parse(JObject parameters)
        {
            var backup = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>>();

            foreach (var parameter in parameters)
            {
                var collection = backup.GetOrAdd(parameter.Key, _ => new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>());
                if (parameter.Value == null || !parameter.Value.Any())
                    return default;

                foreach (var token in parameter.Value)
                {
                    if (token is not JObject idAndDocument)
                        return default;

                    var idProperty = idAndDocument.Properties().First();
                    var id = idProperty.Name;

                    if (idProperty.First() is not JObject document)
                        return default;

                    if (!validator.IsValid(document))
                        return default;
    
                    collection[id] = document.ToObject<ConcurrentDictionary<string, string>>();
                }
            }

            return (true, backup);
        }
    }
}
