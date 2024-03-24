using UnityEngine;
using UnityEditor;

public class NormalMapImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter importer = assetImporter as TextureImporter;

        // Check if the asset's filename indicates it's a normal map
        if (assetPath.ToLowerInvariant().Contains("_norm"))
        {
            // Set the texture to be imported as a normal map
            importer.textureType = TextureImporterType.NormalMap;

            // Ensure the alpha channel is not treated as transparency
            importer.alphaIsTransparency = false;

            // Set texture format to one that preserves the alpha channel
            // Note: This step may vary depending on the target platform
            // and whether you're working in a linear or gamma color space.
            // Here, we're setting a generic format as an example.
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings
            {
                name = "Standalone",
                format = TextureImporterFormat.RGBA32,
                textureCompression = TextureImporterCompression.Uncompressed
            });

            // Additional settings adjustments can be made here
            // For example:
            // importer.filterMode = FilterMode.Trilinear;
            // importer.anisoLevel = 16;
        }
    }
}
