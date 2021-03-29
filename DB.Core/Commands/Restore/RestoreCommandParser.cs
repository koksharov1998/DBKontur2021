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
            if (parameters.Count != 1)
            {
                return default;
            }

            var backup = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>>();
            var ok = true;

            var collectionProperty = parameters.Properties().First();
            // throw new Exception(collectionProperty.ToString());

            var collectionName = collectionProperty.Name;
            // throw new Exception(collectionName.ToString());

            try
            {
                foreach (var collection in parameters.Properties())
                {
                    foreach (var idAndDocument in collection)
                    {
                        // throw new Exception(idAndDocument.ToString());
                        // throw new Exception(idAndDocument.First().ToString());
                        // throw new Exception(idAndDocument.First().First().ToString());   

                        if (!validator.IsValid((JObject)idAndDocument.First().First()))
                        {
                            ok = false;
                            break;
                        }
                        // throw new Exception(ok.ToString());

                    }
                }
            }
            catch
            {
                ok = false;
            }



            if (!ok)
                return default;


            return (true, backup);
        }
    }
}
