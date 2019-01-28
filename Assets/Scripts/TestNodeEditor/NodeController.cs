using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TestNodeEditor
{
    public class NodeController
    {
        [MenuItem("Assets/Create/Node Controller")]
        public static NodeList Create()
        {
            NodeList asset = ScriptableObject.CreateInstance<NodeList>();

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(GetPath() + "/New Node Controller.asset");
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