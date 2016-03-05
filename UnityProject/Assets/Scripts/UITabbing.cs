using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UITabbing : MonoBehaviour {
    [System.Serializable]
    public struct Tab
    {
        public GameObject panel;
        public Image ui;
    }
    [SerializeField] private Tab[] Tabs;
    [SerializeField] private Color ActiveColor;
    [SerializeField] private Color DisabledColor;

    private int mActiveTab = 0;

    public void OnTabClicked(int id)
    {
        if (id > Tabs.Length)
            throw new System.Exception("That tab does not exist");

        //special case for the takeoff tab
        if(id == 3 && mActiveTab == 3)
        {
            SceneManager.LoadScene("Game");
        }

        Tabs[mActiveTab].panel.SetActive(false);
        Tabs[mActiveTab].ui.color = DisabledColor;

        Tabs[id].panel.SetActive(true);
        Tabs[id].ui.color = ActiveColor;

        mActiveTab = id;
    }
}
