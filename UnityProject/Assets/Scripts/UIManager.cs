using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour {
    [System.Serializable]
    public class OnTextChangedEvent : UnityEvent<string> { }
    [System.Serializable]
    public class OnValueChangedEvent : UnityEvent<float> { }

    public static UIManager UISystem;

    [SerializeField] public OnTextChangedEvent StartText;
    [SerializeField] public OnTextChangedEvent EndText;

    [SerializeField] public OnTextChangedEvent FuelText;
    [SerializeField] public OnValueChangedEvent FuelValue;

    [SerializeField] public OnTextChangedEvent ThrottleText;
    [SerializeField] public OnValueChangedEvent ThrottleValue;

    [SerializeField] public OnValueChangedEvent SpeedValue;
    [SerializeField] public OnTextChangedEvent SpeedText;

    [SerializeField] public OnTextChangedEvent CargoText;
    [SerializeField] public OnValueChangedEvent CargoValue;

    [SerializeField] public OnTextChangedEvent PassengerText;
    [SerializeField] public OnValueChangedEvent PassengerValue;

    [SerializeField] public OnValueChangedEvent DistanceValue;

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
        FuelText.Invoke(string.Format("{0}% Fuel", Mathf.FloorToInt(val * 100)));
    }

    public void ChangeCargoValue(float val)
    {
        CargoValue.Invoke(val);
        CargoText.Invoke(string.Format("{0}% Cargo", Mathf.FloorToInt(val * 100)));
    }

    public void ChangePassengerValue(int val)
    {
        PassengerValue.Invoke(val);
        PassengerText.Invoke(string.Format("{0} Passengers", Mathf.FloorToInt(val * 100)));
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

    public void ChangeEndText(string val)
    {
        EndText.Invoke(val);
    }

    public void ChangeSpeedValue(float val)
    {
        SpeedValue.Invoke(val);
        SpeedText.Invoke((Mathf.FloorToInt(val * 10) / 10) + " m/s");
    }

    public void ChangeDistanceValue(float val)
    {
        DistanceValue.Invoke(val);
    }
}
