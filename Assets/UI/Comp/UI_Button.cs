
using System;
using UnityEngine;
using UnityEngine.UI;

namespace W {
	public class UI_Button : MonoBehaviour
	{
		public Action OnTap;
		public void _Tap() {
			OnTap?.Invoke();
        }

		[SerializeField]
		public Image Image;

		[SerializeField]
		public Button Button;

		[SerializeField]
		public Text Text;


		[SerializeField]
		public Slider Slider;

		[NonSerialized]
		public Color ItemColor;


        public void UpdateWith(Func<float> progress) {
			float p = progress == null ? 1 : progress.Invoke();
			bool inCooldown = p < 1;

			Slider.value = 1 - p;
			Slider.gameObject.SetActive(inCooldown);

			Button.interactable = !inCooldown && OnTap != null;
			Button.image.color = ItemColor;

			const float greyscale = 0.5f;
			Text.color = Button.interactable ? ItemColor
							: new Color(greyscale * ItemColor.r, greyscale * ItemColor.g, greyscale * ItemColor.b);
		}
    }
}

