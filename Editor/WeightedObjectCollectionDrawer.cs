using UnityEditor;
using UnityEngine;

namespace WeightedObjects
{
    [CustomPropertyDrawer(typeof(WeightedObjectCollection<>), true)]
    public class WeightedObjectCollectionDrawer : PropertyDrawer
    {
#if UNITY_2020_2_OR_NEWER
        public const float MOVE_COL_WIDTH = 28;
        public const float WEIGHT_COL_WIDTH = 42;
#else
        public const float MOVE_COL_WIDTH = 12;
        public const float WEIGHT_COL_WIDTH = 42;
#endif
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;
            var arrProp = property.FindPropertyRelative("weightedObjects");
            float arrHeight = EditorGUI.GetPropertyHeight(arrProp);
            height += arrHeight;

            if (arrProp.isExpanded)
            {
                //Footer
                height += ExtraEditorGUIUtility.SingleLineHeight();
                height += ExtraEditorGUIUtility.SingleLineHeight();
#if UNITY_2020_1_1
                height += ExtraEditorGUIUtility.SingleLineHeight();
#endif
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect containerRect = EditorGUI.IndentedRect(position);
            containerRect.height = GetPropertyHeight(property, label);
            GUI.Box(containerRect, "");

            EditorGUI.BeginProperty(position, label, property);

            position.height = EditorGUIUtility.singleLineHeight;

            if (!property.isExpanded) { property.isExpanded = true; }
            if(property.isExpanded)
            {
                SerializedProperty randomTypeProp = property.FindPropertyRelative("randomType");

                var arrProp = property.FindPropertyRelative("weightedObjects");

                //Draw Array
                {
                    float arrHeight = EditorGUI.GetPropertyHeight(arrProp);

                    Rect arrRect = position;
                    arrRect.height = arrHeight;

                    if(arrProp.hasMultipleDifferentValues)
                    {
                        EditorGUI.HelpBox(arrRect, "Multiple Array Editing is not supported.", MessageType.Warning);
                    }
                    else
                    {
                        EditorGUI.PropertyField(arrRect, arrProp, label, true);
                    }
                    position.y += EditorGUI.GetPropertyHeight(arrProp);
                }

#if UNITY_2020_1_1
                position.y += ExtraEditorGUIUtility.SingleLineHeight();
#endif
                if (arrProp.isExpanded)
                {
                    //float colTitleLeftOffset = 48; 
                    float colTitleLeftOffset = 0;
                    float colTitleRightOffset = 68;
                    float colTitleHeight = 18;
                    position.y -= ExtraEditorGUIUtility.SingleLineHeight();

                    Rect indentedColPos = EditorGUI.IndentedRect(position);

                    Rect moveColRect = indentedColPos;
                    moveColRect.x += colTitleLeftOffset;
                    moveColRect.height = colTitleHeight;
                    moveColRect.width = MOVE_COL_WIDTH;
                    GUI.Box(moveColRect, new GUIContent(""), EditorStyles.helpBox);

                    Rect weightColRect = indentedColPos;
                    weightColRect.x += colTitleLeftOffset + MOVE_COL_WIDTH;
                    weightColRect.height = colTitleHeight;
                    weightColRect.width = WEIGHT_COL_WIDTH;
                    GUIContent numberGUIContent = new GUIContent("");
                    RandomType randomType = (RandomType)randomTypeProp.enumValueIndex;
                    switch (randomType)
                    {
                        case RandomType.WeightedRandom:
                            numberGUIContent = new GUIContent("Wt  ▲", "The larger, the more likely it will be selected.");
                            break;
                        case RandomType.RandomExhaustive:
                            numberGUIContent = new GUIContent("Amt ▲", "The amount of times it will appear before the pool is exhausted.");
                            break;
                        case RandomType.Ordered:
                            numberGUIContent = new GUIContent("Ord ▲", "Will play in the visual order. Use Sort button to sort by this value.");
                            break;
                        default:
                            break;
                    }
                    GUI.Box(weightColRect, numberGUIContent, EditorStyles.helpBox);

                    Rect contentColRect = indentedColPos;
                    contentColRect.height = colTitleHeight;
                    contentColRect.x += colTitleLeftOffset;
                    contentColRect.x += WEIGHT_COL_WIDTH + MOVE_COL_WIDTH;
                    contentColRect.width -= (WEIGHT_COL_WIDTH + MOVE_COL_WIDTH + colTitleRightOffset);
                    GUI.Box(contentColRect, new GUIContent("Content ▲"), EditorStyles.helpBox);

                    position.y += ExtraEditorGUIUtility.SingleLineHeight();

                    //SUM
                    Rect sumLabelRect = position;
                    sumLabelRect.x += 0;
                    EditorGUI.LabelField(sumLabelRect, new GUIContent("Sum:"), EditorStyles.miniBoldLabel);
                    if (!property.hasMultipleDifferentValues)
                    {
                        float sum = 0;
                        for (int i = 0; i < arrProp.arraySize; i++)
                        {
                            sum += arrProp.GetArrayElementAtIndex(i).FindPropertyRelative("weight").floatValue;
                        }
                        Rect sumValRect = weightColRect;
                        sumValRect.y = position.y;
                        sumValRect.x += colTitleLeftOffset;
                        GUI.Box(sumValRect, new GUIContent($"{sum}"), EditorStyles.centeredGreyMiniLabel);
                    }

                    //UTILITIES
                    Rect toolBarRect = position;
                    toolBarRect.width = position.width - colTitleLeftOffset - WEIGHT_COL_WIDTH - 10;
                    toolBarRect.x += colTitleLeftOffset + WEIGHT_COL_WIDTH;
                    Color o_color = GUI.backgroundColor;
                    //GUI.backgroundColor = Color.red;
                    GUI.Box(toolBarRect, "");
                    //GUI.backgroundColor = o_color;

                    EditorGUI.BeginDisabledGroup(property.hasMultipleDifferentValues);
                    if (toolBarRect.width > 40)
                    {
                        Rect sortRect = DrawSortRect(toolBarRect, arrProp);
                        if (toolBarRect.width > (40 + 70))
                        {
                            DrawNormalizeButton(arrProp, sortRect);
                        }
                    }
                    EditorGUI.EndDisabledGroup();

                    position.y += ExtraEditorGUIUtility.SingleLineHeight();

                    EditorGUI.PropertyField(position, randomTypeProp);
                    position.y += ExtraEditorGUIUtility.SingleLineHeight();

                    position.y += ExtraEditorGUIUtility.SingleLineHeight();
                }
            }
            EditorGUI.EndProperty();
        }

        private static Rect DrawSortRect(Rect position, SerializedProperty arrProp)
        {
            Rect sortRect = position;
            sortRect.width = 40;
            sortRect.x = (position.x + position.width - sortRect.width);

            if (GUI.Button(sortRect, "Sort", EditorStyles.miniButton))
            {
                while(true)
                {
                    bool changed = false;
                    float prevWeight = -1;
                    for (int i = 0; i < arrProp.arraySize; i++)
                    {
                        var weight = arrProp.GetArrayElementAtIndex(i).FindPropertyRelative("weight").floatValue;
                        if(i > 0)
                        {
                            if(weight > prevWeight)
                            {
                                changed = true;
                                arrProp.MoveArrayElement(i, i - 1);
                            }
                        }
                        prevWeight = weight;
                    }
                    if (!changed) { break; }
                }
            }

            return sortRect;
        }

        private static void DrawNormalizeButton(SerializedProperty arrProp, Rect sortRect)
        {
            Rect normaliseRect = sortRect;
            normaliseRect.width = 70;
            normaliseRect.x -= (normaliseRect.width);
            if (GUI.Button(normaliseRect, "Normalize", EditorStyles.miniButton))
            {
                float sum = 0;
                for (int i = 0; i < arrProp.arraySize; i++)
                {
                    sum += arrProp.GetArrayElementAtIndex(i).FindPropertyRelative("weight").floatValue;
                }
                float norm = 1f / sum;
                for (int i = 0; i < arrProp.arraySize; i++)
                {
                    var weightProp = arrProp.GetArrayElementAtIndex(i).FindPropertyRelative("weight");
                    weightProp.floatValue = (float)System.Math.Round(norm * weightProp.floatValue, 3);
                }
            }
        }
    }
}
