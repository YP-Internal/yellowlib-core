using YellowPanda.Attributes;
using UnityEditor;
using UnityEngine;

namespace YellowPanda.Editor {
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class SceneAttributeDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            property.stringValue = SceneInspectorSelector.SceneSelector(position, property.displayName, property.stringValue);
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}