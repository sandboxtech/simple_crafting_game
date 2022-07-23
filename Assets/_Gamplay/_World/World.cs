
using System.Text;

namespace W
{
    public static class World
    {
        public static void Tick() {
            // todo
            // 世界动态行为
        }
        public static void Build() {

            BuildWorld();
            BuildHome();
        }


        private static void BuildWorld() {
            // todo
        }
        private static void BuildHome() {
            // 家
            if (Hand.Key == null) Hand.Key = Key.HAND;

            Place.Key = ResourceDef.ByTransportOf(Key.ROOT).Location;

            Notice.Content = "一片黑暗";

            Hand.Add<Player>();

        }
    }
}



