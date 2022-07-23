
using System.Collections.Generic;

namespace W
{
    //public class Pack : Card
    //{

    //    public override string Name => $"{Place.Name}\n卡包";
    //    public override void Click() {

    //        List<UIItem> items = new List<UIItem>();

    //        items.Add(UI.Text(Name));

    //        int index = Index;
    //        if (index >= PossibleCard.Count) {
    //            items.Add(UI.Text("未发行"));
    //            UI.Show(items);
    //            return;
    //        }

    //        List<(string, int)> array = PossibleCard[index];
    //        if (array.Count > 0) {
    //            items.Add(UI.Text("可能开出以下物品"));
    //            for (int i = 0; i < array.Count; i++) {
    //                items.Add(UI.Text($"{array[i].Item1} {array[i].Item2}"));
    //            }
    //        }

    //        UI.Show(items);
    //    }



    //    public override void DoubleClick() {

    //        if (!Ready) {
    //            return;
    //        }

    //        int index = Index;
    //        if (index >= PossibleCard.Count) {
    //            return;
    //        }

    //        for (int i = 0; i < 2; i++) {
    //            string result = ResultOfWeightedRandomChoice(PossibleCard[index]);
    //            Place.Add(result, 0);
    //        }
    //        Remove();
    //    }


    //    /// <summary>
    //    /// 加权随机算法
    //    /// </summary>
    //    public string ResultOfWeightedRandomChoice(List<(string, int)> array) {
    //        int sum = 0;
    //        for (int i = 0; i < array.Count; i++) {
    //            sum += array[i].Item2;
    //        }

    //        int random = UnityEngine.Random.Range(0, sum);
    //        int left = 0;
    //        for (int i = 0; i < array.Count; i++) {
    //            int right = left + array[i].Item2;
    //            if (random >= left && random < right) {
    //                return array[i].Item1;
    //            }
    //            left = right;
    //        }
    //        return null;
    //    }
    //}
}
