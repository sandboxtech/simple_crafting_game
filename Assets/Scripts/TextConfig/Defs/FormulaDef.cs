
using System.Collections.Generic;
using System.Text;

namespace W
{
    public class FormulaDef
    {
        private static Dictionary<string, List<FormulaDef>> formulasDictKey;
        private static Dictionary<string, List<FormulaDef>> formulasDictCondition;
        private static Dictionary<string, List<FormulaDef>> formulasDictInput;
        private static Dictionary<string, List<FormulaDef>> formulasDictOutput;

        public static IReadOnlyList<FormulaDef> Of(string key) => formulasDictKey.GetOrNull(key);
        public static IReadOnlyList<FormulaDef> OfCondition(string key) => formulasDictCondition.GetOrNull(key);
        public static IReadOnlyList<FormulaDef> OfInput(string key) => formulasDictInput.GetOrNull(key);
        public static IReadOnlyList<FormulaDef> OfOutput(string key) => formulasDictOutput.GetOrNull(key);

        public static (
            Dictionary<string, List<FormulaDef>>, 
            Dictionary<string, List<FormulaDef>>, 
            Dictionary<string, List<FormulaDef>>, 
            Dictionary<string, List<FormulaDef>>
            ) Build() {
            A.Assert(formulasDictKey == null);

            formulasDictKey = new Dictionary<string, List<FormulaDef>>();
            formulasDictCondition = new Dictionary<string, List<FormulaDef>>();
            formulasDictInput = new Dictionary<string, List<FormulaDef>>();
            formulasDictOutput = new Dictionary<string, List<FormulaDef>>();

            return (formulasDictKey, formulasDictCondition, formulasDictInput, formulasDictOutput);
        }


        private readonly IReadOnlyList<string> Content;
        public readonly string ContentRaw;
        public readonly string ContentLocalized;

        public readonly int ConditionCount;
        public readonly int InputCount;
        public readonly int OutputCount;
        public readonly int GotoCount;

        public readonly int ConditionIndex;
        public readonly int ApplyIndex;
        public readonly int GotoIndex;

        public const char ConditionOp = ':';
        public const char ApplyOp = '=';
        public const char GotoOp = '>';


        public const int Invalid = -1;

        // 人 杯 : 酸 碱 = 水 盐 > 地点
        // 工具 : 木板 木板 = 车轮
        public FormulaDef(string raw, string[] array) {
            ContentRaw = raw;
            List<string> content = new List<string>(array.Length);
            Content = content;

            ConditionCount = 0;
            InputCount = 0;
            OutputCount = 0;
            GotoCount = 0;

            ConditionIndex = Invalid;
            ApplyIndex = Invalid;
            GotoIndex = Invalid;
            for (int i = 0; i < array.Length; i++) {
                string item = array[i];
                content.Add(item);
                if (item.Length == 1) {
                    switch (item[0]) {
                        case ConditionOp:
                            ConditionIndex = i;
                            break;
                        case ApplyOp:
                            ApplyIndex = i;
                            break;
                        case GotoOp:
                            GotoIndex = i;
                            break;
                        default:
                            break;
                    }
                }
            }


            StringBuilder sb = new StringBuilder();

            if (ConditionIndex != Invalid) {
                ConditionCount = ConditionIndex;
                A.Assert(ConditionCount > 0);

                for (int i = 0; i < ConditionCount; i++) {
                    sb.Append(ConditionOf(i));
                    if (i == ConditionCount - 1) {
                        sb.Append(' ');
                        sb.Append(':');
                    }
                    sb.Append('\n');
                }
            }
            if (ApplyIndex != Invalid) {
                InputCount = ApplyIndex - ConditionIndex - 1;
                OutputCount = GotoIndex == Invalid ? Content.Count - ApplyIndex - 1 : GotoIndex - ApplyIndex - 1;

                for (int i = 0; i < InputCount; i++) {
                    sb.Append(InputOf(i));
                    sb.Append('\n');
                }
                sb.Append('=');
                sb.Append('\n');

                for (int i = 0; i < OutputCount; i++) {
                    sb.Append(OutputOf(i));
                    sb.Append('\n');
                }
            }
            if (OutputCount > 0) {
                sb.Remove(sb.Length - 1, 1);
            }

            if (GotoIndex != Invalid) {
                GotoCount = Content.Count - GotoIndex - 1;
            }

            A.Assert(InputCount + OutputCount + GotoCount > 0, raw);

            ContentLocalized = sb.ToString();
        }

        public string Key => Content[0];

        public string ConditionOf(int i) => Content[i];
        public string InputOf(int i) => Content[i + ConditionIndex + 1];
        public string OutputOf(int i) => Content[i + ApplyIndex + 1];
        public string Goto => GotoIndex == Invalid ? null : Content[GotoIndex + 1];
    }
}
