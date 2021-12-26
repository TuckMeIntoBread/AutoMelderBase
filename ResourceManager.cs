using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AutoMelder
{
    public static class ResourceManager
    {
        public static readonly Lazy<Dictionary<int, Dictionary<int, float>>> OvermeldChances;

        static ResourceManager()
        {
            OvermeldChances = new Lazy<Dictionary<int, Dictionary<int, float>>>(() => LoadResource<Dictionary<int, Dictionary<int, float>>>(Resources.MateriaJoinRate));
        }

        public static T LoadResource<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}