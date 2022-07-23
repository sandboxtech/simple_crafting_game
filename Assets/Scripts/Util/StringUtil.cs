
namespace W
{
    public static class StringUtil
	{
        public static bool IsCommentOrEmpty(string line) {
            if (line.Length == 0) return true; // skip empty lines
            if (line.StartsWith('#')) return true; // skip comments
            return false;
        }
    }
}
