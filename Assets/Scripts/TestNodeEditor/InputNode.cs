using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TestNodeEditor
{
    [Serializable]
    public class InputNode : BaseInputNode
    {
        public enum InputType { Number, Randomization }

        public InputType inputType;

        public string randomFrom = "";
        public string randomTo = "";
        public string inputValue = "";

        public InputNode()
        {
            windowTitle = "Input Node";
        }

        public override void DrawWindow()
        {
            base.DrawWindow();
            inputType = (InputType)EditorGUILayout.EnumPopup("Input type: ", inputType);

            if (inputType == InputType.Number)
            {
                inputValue = EditorGUILayout.TextField("Value", inputValue);
            }
            else
            {
                randomFrom = EditorGUILayout.TextField("From", randomFrom);
                randomTo = EditorGUILayout.TextField("To", randomTo);

                if (GUILayout.Button("Calculate Random"))
                {
                    CalculateRandom();
                }
            }
        }

        public override void DrawCurves()
        {

        }

        private void CalculateRandom()
        {
            float rFrom = 0;
            float rTo = 0;
            float.TryParse(randomFrom, out rFrom);
            float.TryParse(randomTo, out rTo);

            int randFrom = (int)(rFrom * 10);
            int randTo = (int)(rTo * 10);

            int selected = UnityEngine.Random.Range(randFrom, randTo + 1);
            float selectedValue = selected / 10;

            inputValue = selectedValue.ToString();
        }

        public override string GetResult()
        {
            return inputValue.ToString();
        }
    }
}