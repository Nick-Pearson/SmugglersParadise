﻿using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour {
    [System.Serializable]
    public class OnTextChangedEvent : UnityEvent<string> { }
    [System.Serializable]
    public class OnValueChangedEvent : UnityEvent<float> { }

    public static UIManager UISystem;

    [SerializeField] public OnTextChangedEvent StartText;

    [SerializeField] public OnTextChangedEvent FuelText;
    [SerializeField] public OnValueChangedEvent FuelValue;

    [SerializeField] public OnTextChangedEvent ThrottleText;
    [SerializeField] public OnValueChangedEvent ThrottleValue;

    [SerializeField] public OnValueChangedEvent SpeedValue;
    [SerializeField] public OnTextChangedEvent SpeedText;

    void Awake()
    {
        if (UISystem == null)
            UISystem = this;
        else
            throw new System.Exception("Multiple Instances of UI Manager have been detected in the scene");
    }

    public void ChangeFuelValue(float val)
    {
        FuelValue.Invoke(val);
        FuelText.Invoke(string.Format("Fuel {0}%", Mathf.FloorToInt(val * 100)));
    }

    public void ChangeThrottleValue(float val)
    {
        val = val / 20;
        ThrottleValue.Invoke(val);
        ThrottleText.Invoke(string.Format("{0}%", Mathf.FloorToInt(val * 100)));
    }

    public void ChangeStartText(string val)
    {
        StartText.Invoke(val);
    }

    public void ChangeSpeedValue(float val)
    {
        SpeedValue.Invoke(val);
        SpeedText.Invoke((Mathf.FloorToInt(val * 10) / 10) + " m/s");
    }
}