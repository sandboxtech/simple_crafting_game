
using UnityEngine;

namespace W
{
    /// <summary>
    /// attention to script execution order
    /// </summary>
    public class CameraComponent : MonoBehaviour
	{
        public static Camera Cam;

        private void Awake() {
            Cam = GetComponent<Camera>();
        }
    }
}
