using UnityEngine;
using UnityEngine.Events;

public class UIModulateColour : MonoBehaviour {
    [System.Serializable]
    public class OnColourChangedEvent : UnityEvent<Color> { }
    //all these variables are fully exposed in case scripts want to mess with them
    public Color BaseColor;

    public float LuminosityFactor = 1.0f;
    public float Cycle = 1.0f;

    [SerializeField]
    public OnColourChangedEvent OnColourChanged;

    void Update()
    {
        float lumaFactor = ( Mathf.Sin(Time.time % Cycle * Mathf.PI/Cycle) * LuminosityFactor )  + 0.5f;
        Color newColour = new Color(BaseColor.r * lumaFactor, BaseColor.g * lumaFactor, BaseColor.b * lumaFactor, BaseColor.a);
        OnColourChanged.Invoke(newColour);
    }
}
