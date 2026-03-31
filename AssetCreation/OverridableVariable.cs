using Sirenix.OdinInspector;
using UnityEngine;

namespace YellowPanda.AssetCreation
{
    [System.Serializable]
    public class OverridableVariable<DataClass, ScriptableObjectClass> where DataClass : class where ScriptableObjectClass : OverridableVariableSO<DataClass>
    {
        [SerializeField] bool useOverride = false;
        [SerializeField, ShowIf("@useOverride")] DataClass overrideValue = null;
        [SerializeField, ShowIf("@!useOverride"), InlineEditor] ScriptableObjectClass defaultValue = null;

#if UNITY_EDITOR
        [Button]
        void CreateAsset()
        {
            string path = ScriptableObjectFactory.GetEditorAssetPath();
            defaultValue = ScriptableObjectFactory.Create<ScriptableObjectClass>(path);
        }
#endif
        public DataClass Value
        {
            get => useOverride ? overrideValue : defaultValue.Value;
        }
    }
}


public class OverridableVariableSO<DataClass> : ScriptableObject where DataClass : class
{
    public DataClass Value;
}

//Exemple usage:

[System.Serializable]
public class OverridableFloat
{
    public float value;
}

public class FloatOverridableSO : OverridableVariableSO<OverridableFloat> { }
