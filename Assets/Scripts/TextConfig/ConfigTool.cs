
namespace W
{
    public static class ConfigTool
	{
        // 某个工具
        public static void ReverseFunctorFormulas(string text, string path) {
            string[] lines = text.Split('\n');

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i].Trim();

                if (line.Length == 0) { sb.Append('\n'); continue; }
                if (line.StartsWith('#')) { sb.Append($"{line}\n"); continue; }
                sb.Append(ReversedLine(line));
            }
            System.IO.File.WriteAllText(path, sb.ToString());
        }
        private static string ReversedLine(string line) {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            string[] tokens = line.Split(' ');
            for (int i = tokens.Length - 1; i >= 0; i--) {
                sb.Append(tokens[i]);
                sb.Append(' ');
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append('\n');
            return sb.ToString();
        }

    }
}
