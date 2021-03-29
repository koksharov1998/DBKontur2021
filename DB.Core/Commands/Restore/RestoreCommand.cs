using DB.Core.State;
using Newtonsoft.Json.Linq;
using DB.Core.Helpers;

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
                return Result.Error.InvalidRequest;

            state.Collections.Clear();
            foreach (var col in backup)
                state.Collections[col.Key] = col.Value;

            return Result.Ok.Empty;
        }
    }
}
