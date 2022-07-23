
using UnityEngine;

namespace W {
	public class FontFixer : MonoBehaviour
	{
		public Font FontToFix;

		[ContextMenu("Fix")]
		private void Fix() {
			FontToFix.material.mainTexture.filterMode = FilterMode.Point;
			Debug.Log("fixed");
        }
	}
}
