using UnityEngine;
using UnityEditor;
using System.IO;

namespace SocialPoint.Tools.FlyThrough
{
    public class FlyThroughGenerator
    {
        [MenuItem("Assets/Create/Fly Through Controller")]
        public static FlyThrough Create()
        {
            FlyThrough asset = ScriptableObject.CreateInstance<FlyThrough>();

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(GetPath() + "/New Fly-Through Controller.asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            return asset;
        }

        private static string GetPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (path == "")
                path = "Assets";
            else if (Path.GetExtension(path) != "")
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");

            return path;
        }
    }
}