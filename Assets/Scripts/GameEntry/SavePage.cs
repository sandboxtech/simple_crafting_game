using System;
using System.Collections.Generic;
using UnityEngine;

namespace W
{
    public static class ModPage
    {
        public static void Click() {
            UI.Show(
                UI.Button("查看 mods", AllModsPage),
                UI.Button("mod 教程", () => UI.ShowReadonlyLongText(mod_tutorial)),
                UI.Space,
                UI.Button("重置 mods", () => {
                    SavePage.DeleteAll(GameEntry.I.ModFolder);
                    Config.TryCopyFromTextConfigToModPath();
                }, new Color32(0xff, 0xcc, 0xcc, 0xff)),
                UI.Empty
            );
        }

        private static string mod_tutorial => @$"

# mod 存放路径
# {GameEntry.I.ModFolder}

#号后面是注释

# A 和 B 变成 C 和 D 的反应可以写成
A B = C D

# 如果加工过程中 A 不消耗，可以写成
A : B = C D
# 上面这一条配方，会被更上面的一条覆盖

# 这里定义一个变化，可以和 官方.txt 互动
木头 木头 木头 = A
石头 石头 石头 = B


# 如果一个卡牌a可以前往其他地方，可以写成
# 卡牌 ~ 地点 ~
# 例：
# 木船 ~ 湖区 ~

# 如果这个地点可以抽卡，则加上若干条目：物品1 权重1 物品2 权重2
# 例：
# 矿车 ~ 矿区 ~ 矿石 1
# 玻璃厂蓝图 ~ 玻璃厂 ~ 望远镜 1 显微镜 1 三棱柱 1 试管 1
";

        private static void AllModsPage() {
            List<string> mods = GameEntry.I.GetAllSaveFilenames(GameEntry.I.ModFolder);

            if (mods == null || mods.Count == 0) {
                UI.Show(
                    UI.Text("没有任何mod！"),
                    UI.Button("返回", Click),
                    UI.Empty
                );
                return;
            }

            List<UIItem> items = new List<UIItem>(mods.Count + 3);
            items.Add(UI.Text($"mod 位置", Color.Lerp(Color.white, Color.blue, 0.5f)));
            items.Add(UI.Text(GameEntry.I.ModFolder));
            items.Add(UI.Text($"mod 列表", Color.Lerp(Color.white, Color.blue, 0.5f)));

            foreach (string mod in mods) {
                if (mod == null) break;
                items.Add(UI.Text(mod));
            }

            UI.Show(items);
        }
    }
    public static class SavePage
    {
        public static void Click() {
            UI.Show(
                UI.Button("保存", Save),
                UI.Space,
                UI.Button("快速读档", LoadQuickSave),
                UI.Button("查看存档", AllSavePage),
                UI.Space,
                UI.Button("清空存档", () => DeleteAll(GameEntry.I.SaveFolder), new Color32(0xff, 0xcc, 0xcc, 0xff)),
                UI.Empty
            );
        }
        public static void DeleteAll(string path) {
            List<string> filenames = new List<string>();
            int count = GameEntry.I.DeleteAll(path, filenames);
            if (count == 0) {
                UI.TextContent = "没有文件需要删除";
                return;
            }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("删除以下文件: \n\n");
            foreach (string filename in filenames) {
                sb.Append(filename);
                sb.Append('\n');
            }
            UI.ShowReadonlyLongText(sb.ToString());
        }

        private static void LoadQuickSave() {
            GameEntry.I.LoadQuickSave();
            AfterLoadQuickSave();
        }

        public static void AfterLoadQuickSave() {
            UI.Show(
                UI.Text("读档成功！"),
                UI.Button("查看存档", AllSavePage),
                UI.Button("关闭", () => UI.Show()),
                UI.Empty
            );
        }

        public static void SaveAndQuit() {
            Save();
            GameEntry.I.QuitGame();
        }
        private static void Save() {
            GameEntry.I.Save();
            UI.Show(
                UI.Text("保存成功！"),
                UI.Button("返回", Click),
                UI.Button("退出", GameEntry.I.QuitGame),
                UI.Button("查看存档", AllSavePage),
                UI.Empty
            );
        }



        private static void AllSavePage() {
            List<string> saves = GameEntry.I.GetAllSaveFilenames(GameEntry.I.SaveFolder);

            if (saves == null || saves.Count == 0) {
                UI.Show(
                    UI.Text("没有任何存档！"),
                    UI.Button("返回", Click),
                    UI.Empty
                );
                return;
            }

            List<UIItem> items = new List<UIItem>(saves.Count + 1);
            items.Add(UI.Text($"存档路径: \n{GameEntry.I.SaveFolder}\n"));

            int i = 0;
            foreach (string save in saves) {
                if (save == null) break;

                bool validTime = long.TryParse(save, out long time);
                if (validTime) {
                    items.Add(
                        UI.Button(
                            $"存档 {(DateTime.FromBinary(time).Ticks / 1_000_000) % 1_000_000}",
                            () => OnTapSave(save, time)
                        )
                    );
                } else {
                    items.Add(UI.Button($"不明存档 {save}", null));
                }

                i++;
            }

            UI.Show(items);
        }
        private static void OnTapSave(string save, long time) {
            if (!GameEntry.I.HasFile(save)) {
                UI.Show(
                    UI.Text("此存档不存在！"),
                    UI.Button("返回", Click),
                    UI.Button("查看存档", AllSavePage),
                    UI.Empty
                );
            } else {
                DateTime date = DateTime.FromBinary(time);
                TimeSpan span = DateTime.UtcNow - date;
                UI.Show(
                    UI.Text($"存档时间"),
                    UI.Text($"{date}"),
                    UI.Text($"存档积灰"),
                    UI.Text($"{span}"),
                    UI.Space,
                    UI.Button("删除", () => {
                        GameEntry.I.DeleteFile(save);
                        UI.Show(
                            UI.Text("存档已被删除！"),
                            UI.Button("查看其他存档", AllSavePage)
                        );
                    }),
                    UI.Button("读取", () => {
                        GameEntry.I.LoadSave(save);
                        UI.TextContent = "成功读取存档！";
                    }),
                    UI.Empty
                );
            }
        }
    }
}
