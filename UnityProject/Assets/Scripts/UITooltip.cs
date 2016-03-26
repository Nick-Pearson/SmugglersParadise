using UnityEngine;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour {
    private bool mShowTooltip = false;

    private float mTooltipShowTime = -1;

    private const float TOOLTIP_SHOW_TIME = 2;

    private RectTransform mTooltip;
    private Text mTooltipText;

    private string mShowText;

    void Awake()
    {
        mTooltip = GetComponent<RectTransform>();
        mTooltipText = GetComponentInChildren<Text>();
        mTooltip.GetComponent<CanvasRenderer>().SetAlpha(0);
    }

    void Update()
    {
        if (mTooltipShowTime != -1 && mTooltipShowTime < Time.time)
        {
            mTooltipShowTime = -1;
            mShowTooltip = true;
            mTooltip.GetComponent<CanvasRenderer>().SetAlpha(1);
            mTooltipText.text = mShowText;
        }

        if (mShowTooltip)
        {
           mTooltip.anchoredPosition = new Vector2(Input.mousePosition.x + (mTooltip.rect.width * 0.5f * (Input.mousePosition.x > Screen.width * 0.5f ? -1 : 1)), Input.mousePosition.y + (0.5f * mTooltip.rect.height) - Screen.height);
        }
    }
    public void SetTooltip(string text)
    {
        mShowText = text;
        mTooltipShowTime = Time.time + TOOLTIP_SHOW_TIME;

        Update();
    }

    public void HideTooltip()
    {
        mShowTooltip = false;
        mTooltip.GetComponent<CanvasRenderer>().SetAlpha(0);
        mTooltipText.text = "";
        mTooltipShowTime = -1;
    }
}
