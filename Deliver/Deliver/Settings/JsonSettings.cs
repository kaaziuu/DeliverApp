using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Deliver.Settings
{
    public static class JsonSettings
    {
        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
                Formatting = Formatting.Indented,
            };
        }
    }
}
