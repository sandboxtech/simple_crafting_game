
using System;
using UnityEngine;
using UnityEngine.UI;

namespace W
{
    public class UI_Card : MonoBehaviour
    {
        public Action OnTap;

        public void _Tap() {
            OnTap?.Invoke();
        }

        [SerializeField]
        public Text Text;

        [SerializeField]
        public Image Image;

        [SerializeField]
        public UI_CardEffect CardEffect;

        public Color Color {
            set {
                Text.color = value;
                Image.color = value;
                CardEffect.EffectImage.color = value;

                ProgressFill.color = value;
                ProgressForeground.color = value;
            }
        }


        [Header("Progress")]

        [SerializeField]
        public Slider Progress;
        [SerializeField]
        public Image ProgressFill;
        [SerializeField]
        public Image ProgressForeground;

        public bool HasProgress { get => Progress.gameObject.activeSelf; set => Progress.gameObject.SetActive(value); }

        [NonSerialized]
        public long BirthTicks;
        [NonSerialized]
        public long ReadyTicks;

        private void Update() {
            UpdateProgress();
        }

        public void UpdateProgress() {
            if (HasProgress) {
                long ticks = C.Now;
                if (ticks > ReadyTicks) {
                    HasProgress = false;
                    return;
                }
                double td = (double)(ticks - BirthTicks) / (ReadyTicks - BirthTicks);
                float t = (float)td;

                if (t > 1) {
                    HasProgress = false;
                } else if (t < 0) {
                    HasProgress = true;
                    Progress.value = 0;
                } else {
                    HasProgress = true;
                    Progress.value = t;
                }

            }
        }
    }
}
