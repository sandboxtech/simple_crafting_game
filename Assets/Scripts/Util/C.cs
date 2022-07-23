
namespace W
{
    /// <summary>
    /// C for Constant
    /// </summary>
    public static class C
    {
        public static long Now => System.DateTime.UtcNow.Ticks;

        public const long SecondPerMinute = 60;
        public const long MinutePerHour = 60;
        public const long HourPerDay = 24;

        public const long NanoSecond = 10;
        public const long MiniSecond = NanoSecond * 1000;
        public const long Second = MiniSecond * 1000;
        public const long Minute = Second * SecondPerMinute;
        public const long Hour = Minute * MinutePerHour;
    }
}
