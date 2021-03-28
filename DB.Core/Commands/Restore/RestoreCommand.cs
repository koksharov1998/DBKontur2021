using DB.Core.State;
using Newtonsoft.Json.Linq;
using System;
using DB.Core.Helpers;
using DB.Core.Validation;

namespace DB.Core.Commands.Restore
{
    public class RestoreCommand : ICommand
    {
        private readonly IDocumentValidator validator;

        public string Name => "restore";

        public JObject Execute(IDbState state, JObject parameters)
        {

            if (parameters.Count != 1)
            {
                return Result.Error.InvalidRequest;
            }

            var content = JObject.Parse();

            throw new NotImplementedException();
        }
    }
}
