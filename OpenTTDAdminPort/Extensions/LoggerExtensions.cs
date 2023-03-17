using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OpenTTDAdminPort.Extensions
{
    internal static class LoggerExtensions
    {
        internal static void LogErrorJson(this ILogger logger, params object[] objs)
        {
            StringBuilder sb = new();
            foreach(var obj in objs)
            {
                sb.AppendLine(JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                }));
                sb.AppendLine();
                sb.AppendLine("---");
                sb.AppendLine();
            }

            logger.LogError(sb.ToString());
        }
    }
}
