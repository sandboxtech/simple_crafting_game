
namespace W
{
    /// <summary>
    /// A for Assertion
    /// </summary>
    public static class A
	{
        public static void Assert(bool b) {
            if (!b) {
                string stack = System.Environment.StackTrace;
                UI.ShowReadonlyLongText(stack);
                throw new System.Exception(stack);
            }
        }
        public static void Assert(bool b, string message) {
            if (!b) {
                string stack = System.Environment.StackTrace;
                UI.ShowReadonlyLongText($"{message}\n\n{stack}");
                throw new System.Exception(message);
            }
        }
    }
}
