
using System;
using UnityEngine;
using UnityEngine.UI;

namespace W
{
    public class UI_CardEffect : MonoBehaviour
    {

        [SerializeField]
        public Image EffectImage;

        public bool HasSpawnEffect { get; set; }

        private const long Second = 10_000_000;
        private const long Duration = (long)(0.5 * Second);
        private long start = 0;
        private void Start() {
            if (HasSpawnEffect) {
                start = C.Now;
                EffectImage.enabled = true;
                EffectImage.sprite = UISystem.I.SpawnEffect[0];
            }
        }

        private void Update() {
            if (HasSpawnEffect) {
                Sprite[] effects = UISystem.I.SpawnEffect;

                long now = C.Now;
                double t = (double)(now - start) / Duration;
                if (t > 1) {
                    EffectImage.sprite = null;
                    EffectImage.enabled = false;
                } else {
                    int index = (int)(t * effects.Length) % effects.Length;
                    EffectImage.sprite = effects[index];
                }
            }
        }
    }
}
