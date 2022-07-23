
using System.Collections.Generic;
using System.IO;

namespace W
{


    public static class Config
    {
        private static Dictionary<string, AttributeDef> attributes;
        private static Dictionary<string, List<FormulaDef>> formulasKey;
        private static Dictionary<string, List<FormulaDef>> formulasCondition;
        private static Dictionary<string, List<FormulaDef>> formulasInput;
        private static Dictionary<string, List<FormulaDef>> formulasOutput;
        private static Dictionary<string, ResourceDef> byTransport;
        private static Dictionary<string, ResourceDef> byLocation;

        public static string ConfigFilename;
        public static int ConfigLineNumber;

        public static void LogConfigLineWarning() {
            UnityEngine.Debug.LogWarning($"warning at: line {ConfigLineNumber} in {ConfigFilename}");
        }
        public static void Build() {
            if (formulasKey != null) return;

            (formulasKey, formulasCondition, formulasInput, formulasOutput) = FormulaDef.Build();
            attributes = AttributeDef.Build();
            (byTransport, byLocation) = ResourceDef.Build();

            // UnityEngine.TextAsset textAsset in TextConfig.I.Texts

            switch (UnityEngine.Application.platform) {
                //case UnityEngine.RuntimePlatform.WindowsEditor:
                //    LoadFromTextConfig();
                //    break;
                default:
                    try {
                        TryCopyFromTextConfigToModPath();
                        LoadFromPath(GameEntry.I.ModFolder);
                    } catch (System.Exception) {
                        TryCopyFromTextConfigToModPath(true);
                        LoadFromPath(GameEntry.I.ModFolder);
                    }
                    break;
            }
        }


        private static void ProcessFormulaDef(string line, string[] array) {

            FormulaDef formula = new FormulaDef(line, array);

            // 用法
            formulasKey.GetOrCreate(formula.Key).Add(formula);


            // 条件
            for (int i = 0; i < formula.ConditionCount; i++) {
                formulasCondition.GetOrCreate(formula.ConditionOf(i)).Add(formula);
            }

            // 输入
            for (int i = 0; i < formula.InputCount; i++) {
                formulasInput.GetOrCreate(formula.InputOf(i)).Add(formula);
            }

            // 输出
            for (int i = 0; i < formula.OutputCount; i++) {
                formulasOutput.GetOrCreate(formula.OutputOf(i)).Add(formula);
            }

            // duplication check?
        }
        private static void ProcessAttributeDef(string line, string[] array) {
            AttributeDef attrDef = new AttributeDef(line, array);
            attributes.Add(attrDef.Key, attrDef);
        }

        private static void ProcessPropertyDef(string line, string[] array) {
            ResourceDef resourceDef = new ResourceDef(line, array);
            byTransport.Add(resourceDef.Transport, resourceDef);
            byLocation.Add(resourceDef.Location, resourceDef);
        }


        private static void LoadLines(string[] lines) {
            for (int i = 0; i < lines.Length; i++) {
                ConfigLineNumber = i;

                string line = lines[i].Trim();

                if (StringUtil.IsCommentOrEmpty(line)) continue;

                // split
                string[] array = line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

                try {
                    if (array[1].Length == 1 && array[1][0] == AttributeDef.Separator) {
                        ProcessAttributeDef(line, array);
                    } else if (
                        array.Length >= 4
                        && array[1].Length == 1 && array[1][0] == ResourceDef.Separator
                        && array[3].Length == 1 && array[3][0] == ResourceDef.Separator
                        ) {
                        ProcessPropertyDef(line, array);
                    } else {
                        ProcessFormulaDef(line, array);
                    }
                } catch (System.Exception e) {
                    UI.ShowReadonlyLongText($"<color=#ff6666>mod解析错误</color>:\n文件 <color=#ff9999>{ConfigFilename}</color>\n行数 <color=#ff9999>{i + 1}</color>\n\n内容:\n<color=#ff9999>{line}</color>\n\n其他: \n{e.Message}");
                    throw new System.Exception("mod读取失败", e);
                }
            }
        }


        private static void LoadFromPath(string path) {
            foreach (string file in Directory.GetFileSystemEntries(path)) {
                if (file.EndsWith(".meta")) continue;
                if (File.Exists(file)) {
                    ConfigFilename = Path.GetFileName(file);
                    string[] lines = File.ReadAllLines(file);
                    LoadLines(lines);

                }
            }
        }

        //private static void LoadFromTextConfig() {
        //    foreach (UnityEngine.TextAsset textAsset in TextConfig.I.Texts) {
        //        ConfigFilename = textAsset.name;
        //        string[] lines = textAsset.text.Split('\n');
        //        LoadLines(lines);
        //    }
        //}

        public static void TryCopyFromTextConfigToModPath(bool overwrite = false) {
            string path = GameEntry.I.ModFolder;
            foreach (UnityEngine.TextAsset textAsset in TextConfig.I.Texts) {
                string file = Path.Combine(path, textAsset.name);
                if (!File.Exists(file) || overwrite) {
                    File.WriteAllText(file, textAsset.text);
                }
            }
        }

    }
}
