
using System;
using System.Collections.Generic;
using UnityEngine;

namespace W
{

    public static class Notice
    {
        public static string Content {
            set {
                UISystem.I.Notice = value;
                GameData.I.NoticeContent = value;
            }
        }

    }
    public static class UI
    {
        private static UISystem I => UISystem.I;
        public static void ShowReadonlyLongText(string text) {
            I.InputScrollEditable = false;
            I.InputScrollVisible = true;

            I.InputScrollContent = text;

            Show(
                Button("关闭", () => {
                    I.InputScrollVisible = false;
                    I.InputScrollContent = null;
                    Show();
                }),
                Empty
            );
        }

        public static void ShowEditableText(Action<string> callback, string placeHolderText = null) {
            I.InputScrollEditable = true;
            I.InputScrollVisible = true;

            Show(
                Button("不保存", () => {
                    I.InputScrollVisible = false;
                    I.InputScrollContent = null;
                    Show();
                }),
                Button("保存", () => {
                    callback?.Invoke(I.InputScrollContent);
                    I.InputScrollVisible = false;
                    I.InputScrollContent = null;
                }),
                Empty
            );
        }


        public static void Show(params UIItem[] items) {
            UISystem.I.Show(items);
        }
        public static void Show(List<UIItem> items) {
            UISystem.I.Show(items);
        }

        public static string TextContent { set => Show(Text(value)); }


        public static UIItem Empty => new UIItem {
            Type = UIItemType.Empty,
        };

        public static UIItem Space => new UIItem {
            Type = UIItemType.Space,
        };

        public static UIItem Button(string text, Action onTap) => new UIItem {
            Type = UIItemType.Button,
            Text = text,
            OnTap = onTap,
        };
        public static UIItem Button(string text, Action onTap, Func<float> progress) => new UIItem {
            Type = UIItemType.Button,
            Text = text,
            OnTap = onTap,

            Progress = progress,
        };
        public static UIItem Button(Func<string> getter, Action onTap) => new UIItem {
            Type = UIItemType.Button,
            TextGetter = getter,
            OnTap = onTap,
        };
        public static UIItem Button(Func<string> getter, Action onTap, Func<float> progress) => new UIItem {
            Type = UIItemType.Button,
            TextGetter = getter,
            OnTap = onTap,

            Progress = progress,
        };



        public static UIItem Button(string text, Action onTap, Color color, Func<float> progress=null) => new UIItem {
            Type = UIItemType.Button,
            Text = text,
            OnTap = onTap,
            ColorCustomized = true,
            Color = color,

            Progress = progress,
        };

        public static UIItem TextSingle(string text) => new UIItem {
            Type = UIItemType.TextSingle,
            Text = text,
        };

        public static UIItem TextSingle(string text, Color color) => new UIItem {
            Type = UIItemType.TextSingle,
            Text = text,

            ColorCustomized = true,
            Color = color,
        };

        public static UIItem TextSingle(Func<string> getter) => new UIItem {
            Type = UIItemType.TextSingle,
            TextGetter = getter,
        };

        public static UIItem Text(string text) => new UIItem {
            Type = UIItemType.TextMulti,
            Text = text,
        };

        public static UIItem Text(string text, Color color) => new UIItem {
            Type = UIItemType.TextMulti,
            Text = text,

            ColorCustomized = true,
            Color = color,
        };

        public static string ProgressString(float progress) => $"{Clamp((int)(progress * 100), 100)}%";
        private static int Clamp(int x, int max) => x > max ? max : x;

        public static string TimeString(long ticks) {

            ticks -= C.Now;

            long hour = ticks / C.Hour;
            long minute = ticks / C.Minute;
            long second = ticks / C.Second;


            if (hour > 0) {
                return $"{hour}时 {minute % C.MinutePerHour}分";
            }
            else if (minute > 0) {
                return $"{minute}分 {second % C.SecondPerMinute}秒";
            }
            else if (second > 0) {
                return $"{second}秒";
            }
            return $"1秒";
        }
    }

    public enum UIItemType
    {
        None,
        Empty,
        Space,
        TextSingle,
        TextMulti,
        TextDynamic,
        Button,
    }

    public struct UIItem
    {
        public UIItemType Type;

        public string Text;
        public Func<string> TextGetter;

        public Action OnTap;
        public Func<float> Progress;

        public bool ColorCustomized;
        public Color Color;
    }
}
