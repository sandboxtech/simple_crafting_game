
using System;
using System.Collections.Generic;
using UnityEngine;

namespace W
{

    public class SpriteConfig : MonoBehaviour
    {

        [SerializeField]
        private Sprite[] sprites;

        public static SpriteConfig I { get; private set; }
        private void Awake() {
            A.Assert(I == null);
            I = this;

            spriteDict = new Dictionary<string, Sprite>();

            for (int i = 0; i < sprites.Length; i++) {
                spriteDict.Add(sprites[i].name, sprites[i]);
            }
        }

        private Dictionary<string, Sprite> spriteDict;

        public Sprite Of(string key) {
            if (key == null) return null;
            if (!spriteDict.TryGetValue(key, out Sprite sprite)) {
                return null;
            }
            return sprite;
        }

    }
}
