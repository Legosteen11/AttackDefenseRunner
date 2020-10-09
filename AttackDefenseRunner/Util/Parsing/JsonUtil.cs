using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace AttackDefenseRunner.Util.Parsing
{
    public static class JsonUtil
    {
        public static async Task<T> FromStream<T>(Stream stream)
        {
            string content = await new StreamReader(stream).ReadToEndAsync();

            return FromString<T>(content);
        }

        public static T FromString<T>(string content)
        {
            Log.Information("Received {content}", content);

            var result = JsonConvert.DeserializeObject<T>(content);

            if (result is null)
            {
                throw new ParseFailedException();
            }

            Log.Information("Parsed to {@result}", result);

            return result;
        }
    }
}