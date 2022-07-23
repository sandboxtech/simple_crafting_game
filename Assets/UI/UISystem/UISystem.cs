
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace W
{
    public class UISystem : MonoBehaviour
    {
        public static UISystem I;

        private void Awake() {
            A.Assert(I == null);
            I = this;

            Cam = CameraComponent.Cam;
            A.Assert(Cam != null);

            Canvas.worldCamera = Cam;
        }


        [Header("Ref")]

        [SerializeField]
        private Camera Cam;
        [SerializeField]
        private Canvas Canvas;

        [Space]

        [SerializeField]
        private Slider Progress;
        [SerializeField]
        private Text ProgressText;
        public string Notice { get => ProgressText.text; set => ProgressText.text = value; }


        [Space]

        [SerializeField]
        private ScrollRect Place;
        [SerializeField]
        private RectTransform PlaceContent;

        [Space]

        [SerializeField]
        private ScrollRect Hand;
        [SerializeField]
        private RectTransform HandContent;

        [Space]

        [SerializeField]
        private ScrollRect UI;
        [SerializeField]
        private RectTransform UIContent;


        [Header("Comp")]

        [SerializeField]
        private UI_Card UI_Card;

        [Space]

        [SerializeField]
        private UI_Space UI_Space;

        [SerializeField]
        private UI_TextSingle UI_TextSingle;

        [SerializeField]
        private UI_TextMulti UI_TextMulti;

        [SerializeField]
        private UI_Button UI_Button;



        [Header("Code")]

        [SerializeField]
        private ScrollRect CodeScroll;

        public bool InputScrollRichText { get => CodeInputFieldText.supportRichText; set => CodeInputFieldText.supportRichText = value; }
        public bool InputScrollVisible { get => CodeScroll.gameObject.activeSelf; set { CodeScroll.gameObject.SetActive(value); lastCard = null; } }
        public bool InputScrollEditable { get => CodeInputField.interactable; set => CodeInputField.interactable = value; }
        public string InputScrollContent { get => CodeInputField.text; set => CodeInputField.text = value; }
        public string InputScrollPlaceHolderContent { get => CodeInputFieldPlaceHolder.text; set => CodeInputFieldPlaceHolder.text = value; }


        [SerializeField]
        private InputField CodeInputField;
        [SerializeField]
        private Text CodeInputFieldPlaceHolder;
        [SerializeField]
        private Text CodeInputFieldText;
        [SerializeField]
        private RectTransform CodeInputFieldRectTransform;

        [Header("Code")]

        public Image PixelArt;
        public GameObject PixelArtRoot;

        public void HasPixelArt() {

        }
        public void ShowPixelArt(Sprite sprite) {
            if (sprite == null) return;
            PixelArtRoot.SetActive(true);
            PixelArt.sprite = sprite;
            PixelArt.SetNativeSize();
        }

        public void HidePixelArt() {
            PixelArtRoot.SetActive(false);
        }


        [Header("Temporary Effects")]
        [SerializeField]
        public Sprite[] SpawnEffect;


        [NonSerialized]
        public bool Rerender = true;

        public void Frame() {
            if (Rerender) {
                Rerender = false;
                Sync();
            }
        }

        public void Sync() {
            // ioc
            ShowCards(HandContent, GameData.I.PlaceOf(GameData.I.HandKey));
            ShowCards(PlaceContent, GameData.I.PlaceOf(GameData.I.PlaceKey));
        }

        private void ClearChildren(RectTransform root) {
            int length = root.childCount;
            for (int i = 0; i < length; i++) {
                Destroy(root.GetChild(i).gameObject);
            }
        }

        private int showCardCheckCount = 0;
        private void ShowCardCheck() {
            A.Assert(Time.frameCount != showCardCheckCount);
        }
        private void ShowCards(RectTransform content, List<Card> cards) {
            ShowCardCheck();
            ClearChildren(content);

            long currentTicks = C.Now;
            int currentFrameCount = Time.frameCount;
            for (int i = cards.Count - 1; i >= 0; i--) {
            // for (int i = 0; i < cards.Count; i++) {
                Card card = cards[i];

                UI_Card ui_card = Instantiate(UI_Card, content);

                ui_card.Color = card.Color;

                ui_card.ReadyTicks = card.ReadyTicks;
                ui_card.BirthTicks = card.StartTicks;
                ui_card.HasProgress = card.ReadyTicks > currentTicks && card.ReadyTicks > card.StartTicks;
                ui_card.UpdateProgress();

                ui_card.Text.text = card.Name;

                ui_card.OnTap = () => OnTapCard(card);

                if (!GameEntry.Loading && Time.frameCount > 1) {
                    ui_card.CardEffect.HasSpawnEffect = currentFrameCount == card.BirthFrame;
                }
            }
        }

        private Card lastCard = null;
        private void OnTapCard(Card card) {
            AudioSystem.I.Play(AudioSystem.I.Click);

            if (lastCard == card) {
                card.DoubleClick();
                lastCard = null;
            } else {

                card.Click();
                lastCard = card;
            }
        }


        private int showCheckTimeFrameCount = 0;
        private void ShowCheck() {
            A.Assert(Time.frameCount != showCheckTimeFrameCount);
        }
        public void Show(UIItem[] items) {
            ShowCheck();
            ClearChildren(UIContent);
            ClearDynamicStuff();
            if (items != null && items.Length > 0) {
                foreach (UIItem item in items) {
                    ShowItem(item);
                }
            }
        }
        public void Show(List<UIItem> items) {
            ShowCheck();
            ClearChildren(UIContent);
            ClearDynamicStuff();
            if (items != null && items.Count > 0) {
                foreach (UIItem item in items) {
                    ShowItem(item);
                }
            }
        }

        private void ShowItem(UIItem item) {
            switch (item.Type) {
                case UIItemType.None:
                default:
                    A.Assert(false);
                    break;

                case UIItemType.Empty:
                    break;

                case UIItemType.Space:
                    Instantiate(UI_Space, UIContent);
                    break;

                case UIItemType.TextSingle:
                    UI_TextSingle textSingle = Instantiate(UI_TextSingle, UIContent);

                    if (item.TextGetter != null) {
                        textSingle.Text.text = item.TextGetter();
                        dynamicTexts.Add(textSingle.Text, item.TextGetter);
                    }
                    else {
                        textSingle.Text.text = item.Text;
                    }

                    if (item.ColorCustomized) {
                        textSingle.Text.color = item.Color;
                    }

                    break;

                case UIItemType.TextMulti:
                    UI_TextMulti textMulti = Instantiate(UI_TextMulti, UIContent);
                    textMulti.Text.text = item.Text;
                    if (item.ColorCustomized) {
                        textMulti.Text.color = item.Color;
                    }
                    break;

                case UIItemType.Button:
                    UI_Button button = Instantiate(UI_Button, UIContent);

                    if (item.TextGetter != null) {
                        button.Text.text = item.Text;
                        dynamicTexts.Add(button.Text, item.TextGetter);
                    } else {
                        button.Text.text = item.Text;
                    }

                    button.OnTap = item.OnTap == null ? null : () => {
                        lastCard = null;
                        AudioSystem.I.Play(AudioSystem.I.Click);
                        item.OnTap?.Invoke();
                    };

                    button.ItemColor = item.ColorCustomized ? item.Color : Color.white;
                    button.UpdateWith(item.Progress);


                    if (item.Progress != null) {
                        buttonWithProgress.Add(button, item.Progress);
                    }
                    break;
            }
        }

        private void ClearDynamicStuff() {
            dynamicTexts.Clear();
            buttonWithProgress.Clear();
        }

        private Dictionary<Text, Func<string>> dynamicTexts = new Dictionary<Text, Func<string>>();
        private Dictionary<UI_Button, Func<float>> buttonWithProgress = new Dictionary<UI_Button, Func<float>>();
        private void Update() {
            foreach (var pair in dynamicTexts) {
                pair.Key.text = pair.Value?.Invoke();
            }
            foreach (var pair in buttonWithProgress) {
                pair.Key.UpdateWith(pair.Value);
            }
        }
    }
}




