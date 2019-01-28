using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SocialPoint.Tools
{
    public class BoxLayoutInspector : Editor
    {
        private GUIStyle GetGUIStyle(Color color)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = color;
            style.fontStyle = FontStyle.Bold;

            return style;
        }

        public virtual void Header(string title)
        {
            GUILayout.Space(3);
            GUI.color = new Color(0.75f, 0.75f, 0.75f);
            EditorGUILayout.BeginVertical("Box");
            //GUIStyle font = new GUIStyle { fontSize = 12, fontStyle = FontStyle.Bold };
            GUILayout.Space(1);
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            GUI.color = Color.white;
            GUILayout.Space(3);
            EditorGUI.indentLevel++;
        }

        public virtual void Footer()
        {
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
    }
}