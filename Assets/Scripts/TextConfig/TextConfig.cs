
using UnityEngine;

namespace W
{
    public class TextConfig : MonoBehaviour
	{
        [SerializeField]
        private TextAsset[] texts;
        public TextAsset[] Texts => texts;

        public static TextConfig I { get; private set; }
        private void Awake() {
            A.Assert(I == null);
            I = this;
        }
    }
}
