
using System.Collections.Generic;
using UnityEngine;

namespace W
{
    public class NarrationConfig : MonoBehaviour
    {
        [SerializeField]
        private TextAsset[] texts;

        public static NarrationConfig I { get; private set; }
        private void Awake() {
            A.Assert(I == null);
            I = this;

            spriteDict = new Dictionary<string, string>();

            for (int i = 0; i < texts.Length; i++) {
                
            }
        }
        private void ProcessText(string text) {

        }

        private Dictionary<string, string> spriteDict;

        public string Of(string key) {
            if (key == null) return null;
            if (!spriteDict.TryGetValue(key, out string sprite)) {
                return null;
            }
            return sprite;
        }
    }
}
