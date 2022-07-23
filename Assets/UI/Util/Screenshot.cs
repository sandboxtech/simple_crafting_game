
using System;
using System.Collections.Generic;
using UnityEngine;

namespace W
{
    public class Screenshot : MonoBehaviour
    {
        [ContextMenu("截图")]
        public void TakeScreenshot() {
            // Debug.Log(Application.dataPath);
            ScreenCapture.CaptureScreenshot(Application.dataPath + "/_Screenshot/__screenshot.png");
        }
    }
}
