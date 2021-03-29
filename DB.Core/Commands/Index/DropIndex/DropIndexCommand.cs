using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DB.Core.Commands.DropIndex
{
    public class DropIndexCommand : ICommand
    {
        public string Name => "dropIndex";

        public JObject Execute(IDbState state, JObject parameters)
        {
            if (false)
                return Result.Error.NotFound;

            return Result.Ok.Empty;
        }
    }
}
