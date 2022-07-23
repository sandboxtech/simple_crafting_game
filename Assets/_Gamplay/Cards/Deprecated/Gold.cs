
using UnityEngine;

namespace W
{
    public class Gold : Card
    {
        public Gold(long quantity) {
            this.quantity = quantity;
        }
        private long quantity;
        public long Quantity => quantity;
        public override string Name => $"金币 {Quantity}";
        public override Color Color => Color.yellow;

        public override void DoubleClick() {
            UISystem.I.Rerender = true;
            if (Hand.Cards.Contains(this)) {
                for (int i = 0; i < Place.Cards.Count; i++) {
                    if (Place.Cards[i] is Gold gold) {
                        gold.quantity += Quantity;
                        quantity = 0;
                        break;
                    }
                }
                Hand.Cards.Remove(this);
                if (Quantity > 0) {
                    Place.Cards.Add(this);
                }
            } else if (Place.Cards.Contains(this)) {
                for (int i = 0; i < Hand.Cards.Count; i++) {
                    if (Hand.Cards[i] is Gold gold) {
                        gold.quantity += Quantity;
                        quantity = 0;
                        break;
                    }
                }
                Place.Cards.Remove(this);
                if (Quantity > 0) {
                    Hand.Cards.Add(this);
                }
            }
        }
    }
}
