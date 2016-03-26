using UnityEngine;
using UnityEngine.UI;

public class UISlider : MonoBehaviour {
    public enum SliderType
    {
        Continous,
        Discrete
    }
    public SliderType SType;

    public Sprite OnSprite;
    public Sprite OffSprite;

    public int SpriteInstances = 5;

    public void UpdateSlide(float val)
    {
        if (SType == SliderType.Continous)
        {
            RectTransform trans = GetComponent<RectTransform>();
            float width = transform.parent.GetComponent<RectTransform>().rect.width;

            trans.offsetMax = new Vector2(-width + (val * width), trans.offsetMax.y);
        }
        else if (SType == SliderType.Discrete) {
            for (int i = 0; i < SpriteInstances; i++)
            {
                if((float)i / SpriteInstances > val)
                {
                    transform.GetChild(i).GetComponent<Image>().sprite = OffSprite;
                } else
                {
                    transform.GetChild(i).GetComponent<Image>().sprite = OnSprite;
                }
            }
        }
    }

    public void Awake()
    {
        if(SType == SliderType.Discrete)
        {
            for (int i = 0; i < SpriteInstances; i++)
            {
                GameObject go = new GameObject("Instance_" + i);
                go.layer = LayerMask.NameToLayer("UI");

                RectTransform rt = go.AddComponent<RectTransform>();
                rt.SetParent(transform);

                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.localScale = new Vector3(1, 1, 1);

                rt.offsetMin = new Vector2(0, -transform.GetComponent<RectTransform>().rect.height);
                rt.offsetMax = new Vector2(rt.offsetMin.y * -1, 0);

                rt.anchoredPosition = new Vector2((i * 0.6f * rt.rect.width) + (0.5f * rt.rect.width), -0.5f * rt.rect.height);

                Image im = go.AddComponent<Image>();
                im.sprite = OnSprite;
            }
        }
    }
}
