
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace W
{

    [JsonObject(MemberSerialization.Fields)]
    public class PlayerStatus
    {
        public PlayerStatus() {
            GachaCooldown = new Idle(0, 1, C.Second * 1);
        }

        public long DiscardCount;

        public Idle GachaCooldown;

        public uint GachaHash = 19971012;

        public uint Next {
            get {
                GachaHash = H.Hash(GachaHash);
                return GachaHash;
            }
        }
        public uint NextRange(uint max) => Next % max;
    }


    public class Player : Card
    {
        public static PlayerStatus Status => GameData.I.PlayerStatus;
        public override string Name => ColorPlayer(PlayerNameText);
        public string PlayerNameText => "玩家";

        public override void DoubleClick() => PlayerPage();
        public override void Click() => PlayerPage();

        public override Color Color => new Color32(0xff, 0xcc, 0xff, 0xff);

        private void PlayerPage() {
            string homeKey = ResourceDef.ByTransportOf(Key.ROOT).Location;
            UI.Show(
                UI.Text($"{Name}\n--------"),

                Hand.Cards.Count <= 1 ? UI.Empty : UI.Button("清空手牌", DiscardHand, ColorOfPlayer),
                // Place.Cards.Count <= 1 ? UI.Empty : UI.Button("清空右边", DiscardPlace, ColorOfPlayer),

                Place.Key == homeKey ? UI.Button("回家 (已经在家)", null, ColorOfPlayer) : UI.Button("回家", () => Place.Key = homeKey, ColorOfPlayer),
                Status.DiscardCount == 0 ? UI.Empty : UI.Text($"弃牌总数 {Status.DiscardCount}", ColorOfPlayer),

                UI.Space,
                UI.Button("教程", () => UISystem.I.ShowPixelArt(SpriteConfig.I.Of("图解")), ColorOfPlayer),
                UI.Button("帮助", () => UI.ShowReadonlyLongText(Guide), ColorOfPlayer),
                UI.Button("存档", SavePage.Click, ColorOfPlayer),
                UI.Button("模组", ModPage.Click, ColorOfPlayer),
                UI.Button("音乐", MusicPage, ColorOfPlayer),

                UI.Empty
            );
        }

        private void MusicPage() {
            string name = AudioSystem.I.MusicName;
            UI.Show(
                UI.Button("返回", PlayerPage),
                UI.Button(GameData.I.MusicEnabled ? "关闭音乐" : "开启音乐", () => {
                    GameData.I.MusicEnabled = !GameData.I.MusicEnabled;
                    if (GameData.I.MusicEnabled) {
                        AudioSystem.I.StopMusic();
                    }
                    else {
                        AudioSystem.I.PlayMusic();
                    }
                    MusicPage();
                }),
                name == null ? UI.Empty : UI.Text($"播放中音乐： \n{name}")
            );

        }

        public virtual string Guide => @$"
游戏教程帮助

{ColorAction("拖拽")}左侧滚动条，可滚动此界面

点击{ColorLocation("橙色")}卡牌，随机抽取卡牌
点击{ColorTransport("黄色")}卡牌，解锁新的卡包

点击{Name}，点击{ColorPlayer("弃牌")}，丢弃左侧的所有牌
点击{Name}，点击{ColorPlayer("存档")}，点击{ColorPlayer("退出")}，可以保存并退出游戏

{ColorAction("单击")}卡牌，{ColorAction("查看")}卡牌的{ColorDescription("属性")}、{ColorDescription("用途")}、{ColorDescription("来源")}
{ColorAction("双击")}卡牌，{ColorAction("移动")}卡牌至另一竖排的{ColorDescription("最上边")}

{ColorAction("双击")}在{ColorDescription("最上边")}的卡牌，可以尝试合成

例如：从上往下第一张牌为木头，第二张牌为石头，
双击'木头'，可激活配方'木头 石头 = 石器'

例如：若已知配方 '石器 : 粘土 石砖 = 火炉'，
且从上往下的卡牌分为 '石器 粘土 石砖'
双击'石器'可以进行合成


";
        private string a = $@"
--- 220531版本目标 --- 

1. {ColorAction("汽车")}
2. {ColorAction("电力")}
3. {ColorAction("计算机")}


--- 图片 ---

树林 建材 湖区 矿区 工坊 学院  油田

";

        /// <summary>
        /// 
        /// </summary>
        private void DiscardHand() {
            Discard(Hand.Cards);
        }

        private void DiscardPlace() {
            Discard(Place.Cards);
        }

        private void Discard(List<Card> cards) {

            List<UIItem> items = new List<UIItem>();

            int goldCollected = 0;
            int discardCount = 0;
            for (int i = cards.Count - 1; i >= 0; i--) {
                Card card = cards[i];
                if (card != this && !(card is Gold)) {
                    if (items.Count == 0) {
                        items.Add(UI.Text("丢弃卡牌：", ColorOfPlayer));
                    }
                    int goldForCard = 1;
                    goldCollected += goldForCard;

                    discardCount++;

                    items.Add(UI.Text(card.Type));

                    cards.RemoveAt(i);
                }
            }
            if (discardCount > 0) {

                Status.DiscardCount += discardCount;

                AudioSystem.I.Play(AudioSystem.I.Discard);

                UI.Show(items);
            }
            UISystem.I.Rerender = true;
        }
    }
}



