using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace WeightedObjects
{

    [CustomPropertyDrawer(typeof(WeightedObject<>), true)]
    public class WeightedObjectPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var contentsProp = property.FindPropertyRelative("contents");

            var o_color = GUI.backgroundColor;

            Rect leftSection = position;
            leftSection.width = WeightedObjectCollectionDrawer.WEIGHT_COL_WIDTH;
            leftSection.height = EditorGUIUtility.singleLineHeight;

            Rect rightSection = position;
            rightSection.x += WeightedObjectCollectionDrawer.WEIGHT_COL_WIDTH;
            rightSection.width -= (WeightedObjectCollectionDrawer.WEIGHT_COL_WIDTH);
            rightSection.height = EditorGUIUtility.singleLineHeight;
            if (contentsProp.propertyType == SerializedPropertyType.Generic)
            {
                rightSection.x += 12;
                rightSection.width -= 12;
            }
            rightSection.x += 4;
            rightSection.width -= 4;


            var labelContent = new GUIContent("");
            if (contentsProp.propertyType == SerializedPropertyType.Generic)
            {
                labelContent = new GUIContent(contentsProp.type.Replace("PPtr<$", "").Replace(">", "").ToString());
            }

            EditorGUI.PropertyField(rightSection, contentsProp, labelContent);

            var weightProp = property.FindPropertyRelative("weight");

            {
                Color o_ContentColor = GUI.contentColor;
                if(weightProp.floatValue < 0)
                {
                    weightProp.floatValue = 0;
                }
                GUI.contentColor = Color.white;
                if(weightProp.floatValue == 0)
                {
                    GUI.contentColor = Color.red;
                }
                weightProp.floatValue = EditorGUI.FloatField(leftSection, weightProp.floatValue, EditorStyles.centeredGreyMiniLabel);
                GUI.contentColor = o_ContentColor;
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var arrData = GetArrayData(property);

            var contentsProp = property.FindPropertyRelative("contents");
            var height = 0f;
            height += EditorGUI.GetPropertyHeight(contentsProp);

            height += EditorGUIUtility.standardVerticalSpacing;
            return height;
        }

        private (SerializedProperty arrayProp, int positionInArray) GetArrayData(SerializedProperty property)
        {
            //Traverse backwards one
            var split = property.propertyPath.Split('.');
            string parentArrayPath = split[0];
            for (int i = 1; i < split.Length - 1; i++)
            {
                parentArrayPath = $"{parentArrayPath}.{split[i]}";
            }
            //Get the parent array
            var parentArrayProp = property.serializedObject.FindProperty(parentArrayPath);
            //Sort Array
            
            int positionInArray = int.Parse(split[split.Length - 1].Split('[', ']')[1]);

            return (parentArrayProp, positionInArray);
        }
    }
}
