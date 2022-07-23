
using System.Collections.Generic;

namespace W
{
    public class AttributeDef
    {
        public readonly string ContentRaw;
        public readonly IReadOnlyDictionary<string, object> Content;
        public readonly string Key;

        public const char Separator = '@';
        public const int StartIndex = 2;

        public AttributeDef(string raw, string[] array) {

            A.Assert(array.Length > StartIndex && array[1][0] == Separator);

            ContentRaw = raw;

            Dictionary<string, object> content = new Dictionary<string, object>();
            Content = content;

            Key = array[0];
            for (int i = StartIndex; i < array.Length; i++) {
                content.Add(array[i], null);
            }
        }



        private static Dictionary<string, AttributeDef> attrs;
        public static Dictionary<string, AttributeDef> Build() {
            A.Assert(attrs == null);

            attrs = new Dictionary<string, AttributeDef>();
            return attrs;
        }
        public static IReadOnlyDictionary<string, object> Of(string key) {
            if (key == null) {
                return null;
            }
            if (!attrs.TryGetValue(key, out AttributeDef attrDef)) {
                return null;
            }
            return attrDef.Content;
        }
        public static bool Has(string key, string attr) {
            if (!attrs.TryGetValue(key, out AttributeDef def)) {
                return false;
            }
            return def.Content.ContainsKey(attr);
        }
    }
}
