
using System.Collections.Generic;

namespace W
{

    public class Code : Card
    {
        public override string Name => "代码";
        public override void DoubleClick() => UI.ShowEditableText(RunCode);
        private void RunCode(string code) {
            GameData.I.Code = code;
            UI.TextContent = "文本已写入";
        }
    }
}
