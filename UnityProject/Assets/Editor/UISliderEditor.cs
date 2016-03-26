using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UISlider))]
public class UISliderEditor : Editor {
    public override void OnInspectorGUI()
    {
        UISlider myTarget = (UISlider)target;

        myTarget.SType = (UISlider.SliderType)EditorGUILayout.EnumPopup("Slider Type", myTarget.SType);

        if(myTarget.SType == UISlider.SliderType.Discrete)
        {
            EditorGUILayout.Separator();

            myTarget.SpriteInstances = EditorGUILayout.IntSlider("Instance Count", myTarget.SpriteInstances, 1, 30);

            EditorGUILayout.LabelField("On & Off Sprites");

            EditorGUILayout.BeginHorizontal();
            myTarget.OnSprite = (Sprite)EditorGUILayout.ObjectField(myTarget.OnSprite, typeof(Sprite), false);
            myTarget.OffSprite = (Sprite)EditorGUILayout.ObjectField(myTarget.OffSprite, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();
        }
    }
}
