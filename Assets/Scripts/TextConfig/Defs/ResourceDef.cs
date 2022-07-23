
using System.Collections.Generic;

namespace W
{

    public class ResourceDef
    {
        public readonly string ContentRaw;
        public readonly IReadOnlyDictionary<string, string> Content;
        public readonly string Transport;
        public readonly string Location;

        public const char Separator = '~';
        public const int StartIndex = 4;
        public const int PairCount = 2;

        // 木船 ~ 河流 ~ 粘土 1 沙砾 1
        public ResourceDef(string raw, string[] array) {
            A.Assert(array.Length % 2 == 0, raw);

            ContentRaw = raw;

            Transport = array[0];
            Location = array[2];

            Dictionary<string, string> content = new Dictionary<string, string>();
            Content = content;

            for (int i = StartIndex; i < array.Length; i += PairCount) {
                content.Add(array[i], array[i + 1]);
            }
        }



        private static Dictionary<string, ResourceDef> byTransport;
        private static Dictionary<string, ResourceDef> byLocation;

        public static (Dictionary<string, ResourceDef>, Dictionary<string, ResourceDef>) Build() {
            A.Assert(byTransport == null && byLocation == null);

            byTransport = new Dictionary<string, ResourceDef>();
            byLocation = new Dictionary<string, ResourceDef>();

            return (byTransport, byLocation);
        }

        public static bool HasTransport(string key) {
            if (key == null) {
                return false;
            }
            return byTransport.ContainsKey(key);
        }
        public static ResourceDef ByTransportOf(string key) {
            if (key == null) {
                return null;
            }
            if (!byTransport.TryGetValue(key, out ResourceDef def)) {
                return null;
            }
            return def;
        }

        public static bool HasLocation(string key) {
            if (key == null) {
                return false;
            }
            return byLocation.ContainsKey(key);
        }
        public static ResourceDef ByLocationOf(string key) {
            if (key == null) {
                return null;
            }
            if (!byLocation.TryGetValue(key, out ResourceDef def)) {
                return null;
            }
            return def;
        }
    }

}
