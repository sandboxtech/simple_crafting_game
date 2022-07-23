
using UnityEngine;
using UnityEngine.UI;

namespace W
{
    public class UI_TextMulti : MonoBehaviour
    {
        [SerializeField]
        public Text Text;

        [ContextMenu("Resize")]
        private void Start() {
            RectTransform rect = GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, EvenNumber(Text.preferredHeight));
        }
        private float EvenNumber(float x) {
            return Mathf.Floor(x / 2) * 2;
        }
    }
}
