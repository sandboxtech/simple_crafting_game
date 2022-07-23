using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace W
{
    [MoonSharp.Interpreter.MoonSharpUserData]
    [JsonObject(MemberSerialization.Fields)]
    public class Card
    {
        [JsonIgnore]
        public int BirthFrame { get; set; }


        #region ready
        public long StartTicks { get; set; }
        public long ReadyTicks { get; set; }

        public bool Ready => Progress >= 1;
        public float Progress {
            get {
                if (ReadyTicks <= StartTicks) return 1;
                long ticks = C.Now;
                return M.Progress(ticks, StartTicks, ReadyTicks - StartTicks);
            }
        }

        private void Constructor(string type, long second) {
            Type = type;

            BirthFrame = Time.frameCount;

            SetReadyState(second);
        }
        public void SetReadyState(long second = -1) {
            StartTicks = C.Now;
            ReadyTicks = StartTicks + ((second >= 0 ? second : ReadyTimeInSecondForType(Type)) * 10_000_000);

            if (ReadyTicks < 0) ReadyTicks = StartTicks;
        }

        public Card(string type = null, long second = -1) {
            Constructor(type, second);
        }

        public static long ReadyTimeInSecondForType(string type) {
            if (GameEntry.Loading) return 0;

            IReadOnlyDictionary<string, object> dict = AttributeDef.Of(type);
            if (dict == null) {
                return 1;
            }
            return 0;
        }
        #endregion




        #region view

        /// <summary>
        /// 卡牌名字富文本
        /// </summary>
        public string Type;
        public virtual string Name => Type ?? GetType().Name;
        public virtual Color Color => ColorForType(Type);
        public static Color ColorForType(string type) {
            if (ResourceDef.HasTransport(type)) {
                return ColorOfTransport;
            } else if (ResourceDef.HasLocation(type)) {
                return ColorOfLocation;
            } else if (AttributeDef.Has(type, "科技")) {
                return ColorOfTechnology;
            }
            return ColorOfItem;
        }

        public static string ColorTransport(string s) => $"<color=#ffff99>{s}</color>";
        public static Color ColorOfTransport => new Color32(0xff, 0xff, 0x99, 0xff);
        public static string ColorLocation(string s) => $"<color=#ffcc99>{s}</color>";
        public static Color ColorOfLocation => new Color32(0xff, 0xcc, 0x99, 0xff);
        public static string ColorAction(string s) => $"<color=#9999ff>{s}</color>";
        public static string ColorDescription(string s) => $"<color=#99ffff>{s}</color>";
        public static string ColorItem(string s) => $"<color=#eeeeee>{s}</color>";
        public static Color ColorOfItem => new Color32(0xee, 0xee, 0xee, 0xff);
        public static string ColorPlayer(string s) => $"<color=#ffccff>{s}</color>";
        public static Color ColorOfPlayer => new Color32(0xff, 0xcc, 0xff, 0xff);


        public static Color ColorOfTechnology => new Color32(0xaa, 0xaa, 0xff, 0xff);

        #endregion view


        #region behaviour

        /// <summary>
        /// 单击行为。若空，则构造Click(int n)
        /// </summary>
        /// 
        public virtual void Click() {
            Show();
        }

        private void Show() {

            if (Type == null) {
                UI.TextContent = $"未定义物品\n{GetType().Name}\n";
                return;
            }

            List<UIItem> items = new List<UIItem>();

            items.Add(UI.Text(Type, ColorForType(Type)));


            if (!Ready) {
                items.Add(UI.Button(() => $"合成进度 {UI.ProgressString(Progress)}", Show, () => Progress));
                // items.Add(UI.Text(() => UI.ProgressString(Progress)));
                // items.Add(UI.Text(() => UI.TimeString(ReadyTicks)));
            }


            ResourceDef byLocation = ResourceDef.ByLocationOf(Type);

            if (byLocation != null && byLocation.Content.Count > 0) {
                items.Add(UI.Space);

                if (Place.Cards.Count + Hand.Cards.Count < 22) {
                    items.Add(UI.Button("抽卡", () => DoGacha(byLocation), ColorOfLocation, () => Player.Status.GachaCooldown.Progress));
                } else {
                    items.Add(UI.Button("抽卡 (卡牌过多)", null, ColorOfLocation));
                }

                items.Add(UI.Button("卡池", () => ShowPoolOf(byLocation), ColorOfLocation));
            }

            ResourceDef byTransport = ResourceDef.ByTransportOf(Type);
            if (byTransport != null) {
                items.Add(UI.Space);
                items.Add(UI.Text("-- 前往 --", ColorOfLocation));
                items.Add(UI.Button(byTransport.Location, () => Place.Key = byTransport.Location, ColorOfLocation));
                items.Add(UI.Space);
            }


            IReadOnlyDictionary<string, object> attribute = AttributeDef.Of(Type);
            if (attribute != null && attribute.Count > 0) {
                items.Add(UI.Button("属性", () => ShowAttributesOf(attribute)));
            }



            IReadOnlyList<FormulaDef> formulasCondition = FormulaDef.OfCondition(Type);
            if (formulasCondition != null && formulasCondition.Count > 0) {
                items.Add(UI.Button("功能", () => ShowConditionOf(formulasCondition)));
            }

            IReadOnlyList<FormulaDef> formulasInput = FormulaDef.OfInput(Type);
            if (formulasInput != null && formulasInput.Count > 0) {
                items.Add(UI.Button("去向", () => ShowInputOf(formulasInput)));
            }

            IReadOnlyList<FormulaDef> formulasReverse = FormulaDef.OfOutput(Type);
            if (formulasReverse != null) {
                items.Add(UI.Button("来源", () => ShowOutputOf(formulasReverse)));
            }


            Sprite sprite = SpriteConfig.I.Of(Type);
            if (sprite != null) {
                items.Add(UI.Space);
                items.Add(UI.Button("插画", () => UISystem.I.ShowPixelArt(sprite)));
            }

            UI.Show(items);
        }






        private void ShowPoolOf(ResourceDef resourceDef) {
            List<UIItem> items = new List<UIItem>();
            items.Add(UI.Button(Type, Show, ColorOfLocation));
            AddPoolOf(resourceDef, items);
            UI.Show(items);
        }

        private void AddPoolOf(ResourceDef byLocation, List<UIItem> items) {
            items.Add(UI.Text("-- 卡池 --"));
            foreach (var pair in byLocation.Content) {
                items.Add(UI.Text($"{pair.Key} {pair.Value}"));
            }
        }


        private void DoGacha(ResourceDef byLocation) {
            OpenPack(byLocation.Content);
            Player.Status.GachaCooldown.Clear();
            AudioSystem.I.Play(AudioSystem.I.Gacha);
        }
        private void ShowGachaPool(ResourceDef byLocation) {

        }
        private void OpenPack(IReadOnlyDictionary<string, string> property) {
            string cardType = SelectCard(property);
            Card card = new Card(cardType);
            Place.Add(card);

            Notice.Content = ColorLocation($"获得 {card.Type}");
        }
        private string SelectCard(IReadOnlyDictionary<string, string> property) {
            uint sum = 0;
            foreach (var pair in property) {
                uint v = uint.Parse(pair.Value);
                sum += v;
            }

            //UnityEngine.Random.InitState((int)C.Now);
            //int random = UnityEngine.Random.Range(0, sum);

            uint random = Player.Status.NextRange(sum);
            int left = 0;

            foreach (var pair in property) {
                int v = int.Parse(pair.Value);
                int right = left + v;
                if (random >= left && random < right) {
                    return pair.Key;
                }
                left = right;
            }
            return null;
        }



        private void ShowAttributesOf(IReadOnlyDictionary<string, object> attribute) {
            List<UIItem> items = new List<UIItem>();
            items.Add(UI.Button(Type, Show));
            AddAttributeOf(attribute, items);
            UI.Show(items);
        }
        private void AddAttributeOf(IReadOnlyDictionary<string, object> attribute, List<UIItem> items) {
            items.Add(UI.Text("-- 属性 --", ColorOfTechnology));
            foreach (var pair in attribute) {
                items.Add(UI.Text(pair.Key));
            }
        }


        public static HashSet<object> sameContent { get; private set; } = new HashSet<object>();

        private void ShowConditionOf(IReadOnlyList<FormulaDef> formulas) {
            List<UIItem> items = new List<UIItem>();
            items.Add(UI.Button(Type, Show));
            AddConditionOf(formulas, items);
            UI.Show(items);
        }
        private void AddConditionOf(IReadOnlyList<FormulaDef> formulas, List<UIItem> items) {
            items.Add(UI.Space);
            items.Add(UI.Text("-- 功能 --\n以下配方需要此物", ColorOfItem));
            foreach (var formula in formulas) {
                items.Add(UI.Space);
                items.Add(UI.Text(formula.ContentLocalized));
            }
        }

        private void ShowInputOf(IReadOnlyList<FormulaDef> formulas) {
            List<UIItem> items = new List<UIItem>();
            items.Add(UI.Button(Type, Show));
            AddInputOf(formulas, items);
            UI.Show(items);
        }
        private void AddInputOf(IReadOnlyList<FormulaDef> formulas, List<UIItem> items) {
            items.Add(UI.Space);
            items.Add(UI.Text("-- 去向 --\n以下配方消耗此物", ColorOfItem));
            foreach (var formula in formulas) {
                if (!sameContent.Contains(formula)) {
                    sameContent.Add(formula);
                    items.Add(UI.Space);
                    items.Add(UI.Text(formula.ContentLocalized));
                }
            }
            sameContent.Clear();
        }

        private void ShowOutputOf(IReadOnlyList<FormulaDef> formulas) {
            List<UIItem> items = new List<UIItem>();
            items.Add(UI.Button(Type, Show));
            AddOutputOf(formulas, items);
            UI.Show(items);
        }
        private void AddOutputOf(IReadOnlyList<FormulaDef> formulas, List<UIItem> items) {
            items.Add(UI.Space);
            items.Add(UI.Text("-- 来源 --\n以下配方产生此物", ColorOfItem));
            foreach (var formula in formulas) {
                if (!sameContent.Contains(formula)) {
                    sameContent.Add(formula);
                    items.Add(UI.Space);
                    items.Add(UI.Text(formula.ContentLocalized));
                }
            }
            sameContent.Clear();
        }




        /// <summary>
        /// 双击行为
        /// 常为：移动、变换
        /// </summary>
        public virtual void DoubleClick() {

            if (Type == null) {
                return;
            }

            if (Place.Cards.Count > 0 && Place.Cards[^1] == this
                && Ready && TryApplyFunctorPatternMatch(Place.Cards)) {
                // 变换
            } else if (Hand.Cards.Count > 0 && Hand.Cards[^1] == this
                  && Ready && TryApplyFunctorPatternMatch(Hand.Cards)) {
                // 变换
            } else if (ResourceDef.HasLocation(Type)) {
                // 卡包
            } else {
                AudioSystem.I.Play(AudioSystem.I.Move);
                Move();
            }
        }

        public void TryApply() {
            TryApplyFunctorPatternMatch(Place.Cards);
        }
        private bool TryApplyFunctorPatternMatch(List<Card> cards) {
            // 如果能变换则变换
            // 如果不能变换，则移动
            IReadOnlyList<FormulaDef> formulas = FormulaDef.Of(Type);
            if (formulas == null) {
                return false;
            }
            foreach (FormulaDef formula in formulas) {
                if (TryApplyFunctor(formula, cards)) {
                    return true;
                }
            }
            return false;
        }

        private static List<Card> tempBuffer = new List<Card>(8);
        private bool TryApplyFunctor(FormulaDef formula, List<Card> cards) {
            int cardsCount = cards.Count;

            int formulaMatchCount = formula.ConditionCount + formula.InputCount;
            if (formulaMatchCount > cards.Count) {
                // 数量不可能匹配
                return false;
            }

            for (int i = 0; i < formulaMatchCount; i++) {
                Card card = cards[cardsCount - 1 - i];
                if (card.Type == null) {
                    return false;
                }
                if (!card.Ready) {
                    return false;
                }

                string type;
                if (i < formula.ConditionCount) {
                    type = formula.ConditionOf(i);
                } else {
                    type = formula.InputOf(i - formula.ConditionCount);
                }

                // 符合条件判定，在此
                if (card.Type != type) {
                    return false;
                }
            }

            // 成功匹配，进行替换
            AudioSystem.I.Play(AudioSystem.I.Craft);

            Notice.Content = formula.ContentRaw;
            List<UIItem> items = new List<UIItem>() {
                UI.Text("成功合成"),
                UI.Space,
                UI.Text(formula.ContentRaw),

            };
            if (formula.ConditionCount > 0) {
                items.Add(UI.Space);
                items.Add(UI.Text("-- 条件 --"));
                for (int i = 0; i < formula.ConditionCount; i++) {
                    items.Add(UI.Text(formula.ConditionOf(i)));
                }
            }
            if (formula.InputCount > 0) {
                items.Add(UI.Space);
                items.Add(UI.Text("-- 原料 --"));
                for (int i = 0; i < formula.InputCount; i++) {
                    items.Add(UI.Text(formula.InputOf(i)));
                }
            }
            if (formula.OutputCount > 0) {
                items.Add(UI.Space);
                items.Add(UI.Text("-- 产品 --"));
                for (int i = 0; i < formula.OutputCount; i++) {
                    items.Add(UI.Text(formula.OutputOf(i)));
                }
            }


            UI.Show(items);


            long time = ReadyTimeInSecondForType(formula.Key);

            // 保存条件
            if (formula.ConditionCount > 0) {
                for (int i = 0; i < formula.ConditionCount; i++) {
                    tempBuffer.Add(cards[cardsCount - 1 - i]);

                    cards.RemoveAt(cards.Count - 1);
                }
            }

            // 移除输入
            if (formula.InputCount > 0) {
                for (int i = 0; i < formula.InputCount; i++) {

                    cards.RemoveAt(cards.Count - 1);
                }
            }

            // 恢复条件
            if (formula.ConditionCount > 0) {
                for (int i = tempBuffer.Count - 1; i >= 0; i--) {
                    cards.Add(tempBuffer[i]);
                    tempBuffer[i].SetReadyState(time);
                }
                tempBuffer.Clear();
            }


            // 获取输出
            if (formula.OutputCount > 0) {
                for (int i = 0; i < formula.OutputCount; i++) {
                    string key = formula.OutputOf(formula.OutputCount - 1 - i);
                    cards.Add(new Card(key, time));
                }
            }


            // deprecated
            //// 移动
            //if (formula.GotoCount > 0) {
            //    Place.Key = formula.Goto;
            //}

            UISystem.I.Rerender = true;
            return true;
        }

        #endregion

        #region util

        public void Move() {
            if (Hand.Remove(this)) {
                Place.Add(this);
                Notice.Content = $"打出 {Name}";
            } else if (Place.Remove(this)) {
                Hand.Add(this);
                Notice.Content = $"收回 {Name}";
            }
        }

        public void Remove() {
            if (Hand.Remove(this)) {
            } else if (Place.Remove(this)) {
            }
        }
        #endregion
    }
}


