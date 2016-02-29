using UnityEngine;

public class UISlider : MonoBehaviour {
    public void UpdateSlide(float val)
    {
        RectTransform trans = GetComponent<RectTransform>();
        float width = trans.rect.width;

        trans.offsetMax = new Vector2(-width + (val* width), trans.offsetMax.y);
    }
}
