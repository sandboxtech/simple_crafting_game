
using UnityEditor;

namespace W
{
    public class Importer : AssetPostprocessor
    {
        void OnPreprocessTexture() {
            TextureImporter importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.filterMode = UnityEngine.FilterMode.Point;
            importer.spritePixelsPerUnit = 16;
            // importer.spritePivot = new UnityEngine.Vector2(0, 0);

            TextureImporterSettings textureSettings = new TextureImporterSettings();
            importer.ReadTextureSettings(textureSettings);
            textureSettings.spriteMeshType = UnityEngine.SpriteMeshType.FullRect;
            textureSettings.spriteExtrude = 0;
            textureSettings.spriteGenerateFallbackPhysicsShape = false;
            importer.SetTextureSettings(textureSettings);
        }
    }
}
