using DB.Core.State;
using Newtonsoft.Json.Linq;
using System;
using DB.Core.Helpers;
using DB.Core.Validation;

namespace DB.Core.Commands.Restore
{
    public class RestoreCommand : ICommand
    {
        private readonly IRestoreCommandParser parser;

        public RestoreCommand(IRestoreCommandParser parser)
            => this.parser = parser;

        public string Name => "restore";

        public JObject Execute(IDbState state, JObject parameters)
        {
            var (ok, backup) = parser.Parse(parameters);

            if (!ok)
            {
                return Result.Error.InvalidRequest;
            }

            state.Collections.Clear();

            // state.Collections

            // внести backup в бд

            return Result.Ok.Empty;
        }
    }
}
