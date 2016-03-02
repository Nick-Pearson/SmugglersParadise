using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuSystem : MonoBehaviour {
    //Object References to UI Elements
    [SerializeField] private RectTransform MissionListContainer;
    [SerializeField] private RectTransform CompleteListContainer;
    [SerializeField] private RectTransform AddonListContainer;
    [SerializeField] private RectTransform ShopListContainer;

    [SerializeField] private RectTransform MissionUIPrefab;
    [SerializeField] private RectTransform MissionCompleteUIPrefab;
    [SerializeField] private RectTransform AddonPrefab;

    [SerializeField] private Text PlanetWelcomeText;
    [SerializeField] private Text ShipNameText;
    [SerializeField] private Text MoneyText;
    [SerializeField] private Text DamageText;
    [SerializeField] private Text FuelText;

    [SerializeField] private Button SellAddonButton;
    [SerializeField] private Button BuyAddonButton;
    [SerializeField] private Button UpAddonButton;
    [SerializeField] private Button DownAddonButton;
    [SerializeField] private Button LeftAddonButton;
    [SerializeField] private Button RightAddonButton;

    [SerializeField] private Color NormalColor;
    [SerializeField] private Color SelectedColor;

    //private variables
    private const int UI_PADDING = 5;
    private const int MISSION_SIZE_GROW = 30; //how big to make the mission when we select it

    private RectTransform[] mMissionUIObjects;
    private Mission[] mAvailibleMissions;
    private RectTransform[] mCompletedMissionUIObjects;

    private int mCurrentSelectedMission = -1;
    private Button mCurrentlySelectedShipAddon = null;
    private int mCurrentlySelectedStoreAddon = -1;
    private bool mIsBuying = false;

    private Addon[] mAvailibleAddons;
    private RectTransform[] mAddonUIElements;
    private List<RectTransform> mShipLayoutUIElements = new List<RectTransform>();
    private List<RectTransform> mShipLayoutEmptySlots = new List<RectTransform>();

    void Start()
    {
        mAvailibleMissions = GetRandomMissions(Random.Range(5,25));
        mMissionUIObjects = GenerateMissionList(mAvailibleMissions, MissionListContainer);
        mCompletedMissionUIObjects = GenerateMissionList(GameState.GetCompleteableMissions(), CompleteListContainer);

        //hook events for availible missions
        for(int i = 0; i < mMissionUIObjects.Length; i++)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            int _i = i; //this gets around the parameter being stored in a closure for event calling purposes
            entry.callback.AddListener(delegate { OnMissionClicked(_i); });

            mMissionUIObjects[i].GetComponent<EventTrigger>().triggers.Add(entry);

            //deal with button
            Button.ButtonClickedEvent selectEvent = new Button.ButtonClickedEvent();
            selectEvent.AddListener(delegate { OnMissionSelected(_i); });

            mMissionUIObjects[i].FindChild("Take_Mission").GetComponent<Button>().onClick = selectEvent;
        }

        //hook events for completed missions
        for (int i = 0; i < mCompletedMissionUIObjects.Length; i++)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            int _i = i;
            entry.callback.AddListener((eventData) => { OnCompletedMissionClicked(_i); });

            mCompletedMissionUIObjects[i].GetComponent<EventTrigger>().triggers.Add(entry);
        }

        //TODO: Use this to resize container?
        GenerateShipLayout(GameState.PlayerAddons, AddonListContainer);

        mAvailibleAddons = GetRandomAddons(Random.Range(3, 8));
        mAddonUIElements = GenerateAddonList(mAvailibleAddons, ShopListContainer);

        PopulateUI();
    }

    void PopulateUI()
    {
        UIManager.UISystem.ChangeCargoValue(GameState.PlayerCargoPercentage);
        UIManager.UISystem.ChangeFuelValue(GameState.PlayerFuelPercentage);

        PlanetWelcomeText.text = "Welcome to " + GameState.CurrentPlanet.Name;
        FuelText.text = "Re-Fuel (cr. " + Mathf.Round((GameState.PlayerMaxFuel - GameState.PlayerFuel) * GameState.FUEL_COST) + ")";
        ShipNameText.text = GameState.ShipName;
        MoneyText.text = "cr. " + GameState.PlayerMoney;
    }

    //creates the list of addons for the store
    public RectTransform[] GenerateAddonList(Addon[] addons, RectTransform container)
    {
        RectTransform[] uiElements = new RectTransform[addons.Length];

        for(int i = 0; i < addons.Length; i++)
        {
            RectTransform addonUI = Instantiate(AddonPrefab) as RectTransform;
            addonUI.SetParent(container);
            addonUI.anchoredPosition3D = new Vector3(0, -(addonUI.rect.height / 2) - UI_PADDING - (addonUI.rect.height * i), 0);
            addonUI.localScale = new Vector3(1, 1, 1);
            addonUI.gameObject.name = addons[i].getName();
            addonUI.FindChild("Text").GetComponent<Text>().text = addons[i].getName();

            Button.ButtonClickedEvent selectEvent = new Button.ButtonClickedEvent();

            //variables to be captured in the event closure
            int _i = i;
            selectEvent.AddListener(delegate { OnClickedStoreAddon(_i); });

            addonUI.GetComponent<Button>().onClick = selectEvent;
            addonUI.GetComponent<Button>().colors = buildColorBlock(NormalColor);

            uiElements[i] = addonUI;
        }

        return uiElements;
    }

    //creates ui elements for our ship layout
    //returns the number of columns this ship uses
    public int GenerateShipLayout(Addon baseAddon, RectTransform container, float offsetX = 0, float offsetY = 0, Addon parent = null, Addon.AttachPosition pos = Addon.AttachPosition.Bottom)
    {
        RectTransform addonUI = Instantiate(AddonPrefab) as RectTransform;
        addonUI.SetParent(container);
        addonUI.anchoredPosition3D = new Vector3(offsetX, -(addonUI.rect.height / 2) - UI_PADDING + offsetY, 0);
        addonUI.localScale = new Vector3(1, 1, 1);

        //if we have no addon make this an empty slot
        if (baseAddon == null)
        {
            addonUI.gameObject.name = "Empty";
            addonUI.FindChild("Text").GetComponent<Text>().text = "Empty Slot";
            addonUI.GetComponent<Button>().interactable = false;

            //add hooks to the button event
            Button.ButtonClickedEvent selectEvent = new Button.ButtonClickedEvent();
            selectEvent.AddListener(delegate { OnClickedEmpty(parent, pos, addonUI.gameObject); });

            addonUI.GetComponent<Button>().onClick = selectEvent;
            addonUI.GetComponent<Button>().colors = buildColorBlock(NormalColor);

            mShipLayoutEmptySlots.Add(addonUI);

            return 1;
        }
        else
        {
            addonUI.gameObject.name = baseAddon.getName();
            addonUI.FindChild("Text").GetComponent<Text>().text = baseAddon.getName();

            //add hooks to the button event
            Button.ButtonClickedEvent selectEvent = new Button.ButtonClickedEvent();
            selectEvent.AddListener(delegate { OnAddonSelected(addonUI, baseAddon, parent, pos); });

            addonUI.GetComponent<Button>().onClick = selectEvent;
            addonUI.GetComponent<Button>().colors = buildColorBlock(NormalColor);

            mShipLayoutUIElements.Add(addonUI);
        }

        float width = addonUI.rect.width;
        float height = addonUI.rect.height;
        int rowsLeft = 0, rowsRight = 0, rowsBottom = 0;

        if (baseAddon.canAttach(Addon.AttachPosition.Bottom))
            rowsBottom = GenerateShipLayout(baseAddon.getAttachment(Addon.AttachPosition.Bottom), container, offsetX, offsetY - height, baseAddon);

        if (baseAddon.canAttach(Addon.AttachPosition.Left) && pos != Addon.AttachPosition.Right)
            rowsLeft = GenerateShipLayout(baseAddon.getAttachment(Addon.AttachPosition.Left), container, offsetX + width, offsetY, baseAddon, Addon.AttachPosition.Left);

        if (baseAddon.canAttach(Addon.AttachPosition.Right) && pos != Addon.AttachPosition.Left)
            rowsRight = GenerateShipLayout(baseAddon.getAttachment(Addon.AttachPosition.Right), container, offsetX - width, offsetY, baseAddon, Addon.AttachPosition.Right);
        
        //sum of the other objects rows
        return rowsLeft + rowsRight + rowsBottom;
    }

    public RectTransform[] GenerateMissionList(Mission[] missions, RectTransform container)
    {
        RectTransform[] array = new RectTransform[missions.Length];

        //for each mission create a ui element to display it's data
        for(int i = 0; i < missions.Length; i++)
        {
            RectTransform missionUI = Instantiate(MissionUIPrefab) as RectTransform;
            missionUI.SetParent(container, false);
            missionUI.anchoredPosition3D = new Vector3(missionUI.anchoredPosition3D.x, (-(i+.5f) * missionUI.rect.height) - ((i+1) * UI_PADDING), missionUI.anchoredPosition3D.z);
            missionUI.gameObject.name = i + "";

            UIFromMission(missions[i], missionUI);
            array[i] = missionUI;
        }

        //set the container height so that scrollbars work correctly
        container.offsetMin = new Vector2(container.offsetMin.x, -(missions.Length * (MissionUIPrefab.rect.height + UI_PADDING)) - UI_PADDING);

        //return links to the ui objects
        return array;
    }

    void UIFromMission(Mission m, Transform UIelement)
    {
        UIelement.FindChild("Mission_Title").GetComponent<Text>().text = m.Name;
        UIelement.FindChild("Mission_Text").GetComponent<Text>().text = m.Description;
        UIelement.FindChild("Take_Mission").gameObject.SetActive(false);
        UIelement.FindChild("Mission_Text").gameObject.SetActive(false);
    }

    void ToggleMissionMode(RectTransform mission, bool mode = true)
    {
        if(mode)
        {
            mission.offsetMin = new Vector2(mission.offsetMin.x, mission.offsetMin.y - MISSION_SIZE_GROW);
            mission.FindChild("Take_Mission").gameObject.SetActive(true);
            mission.FindChild("Mission_Text").gameObject.SetActive(true);
        }
        else
        {
            mission.offsetMin = new Vector2(mission.offsetMin.x, mission.offsetMin.y + MISSION_SIZE_GROW);
            mission.FindChild("Take_Mission").gameObject.SetActive(false);
            mission.FindChild("Mission_Text").gameObject.SetActive(false);
        }
    }

    void OnMissionClicked(int missionID)
    {
        if(mCurrentSelectedMission == -1)
        {
            ToggleMissionMode(mMissionUIObjects[missionID]);
            //offset the ui elements bellow us
            for (int i = missionID+1; i < mMissionUIObjects.Length; i++)
            {
                mMissionUIObjects[i].anchoredPosition3D = new Vector3(mMissionUIObjects[i].anchoredPosition3D.x, mMissionUIObjects[i].anchoredPosition3D.y - MISSION_SIZE_GROW, mMissionUIObjects[i].anchoredPosition3D.z);
            }
        }
        else if(mCurrentSelectedMission != missionID)
        {
            //toggle off the currently selected mission
            ToggleMissionMode(mMissionUIObjects[mCurrentSelectedMission], false);

            if (mCurrentSelectedMission < missionID)
            {
                //offset all the missions above us back to normal
                for(int i = mCurrentSelectedMission+1; i < missionID+1; i++)
                {
                    mMissionUIObjects[i].anchoredPosition3D = new Vector3(mMissionUIObjects[i].anchoredPosition3D.x, mMissionUIObjects[i].anchoredPosition3D.y + MISSION_SIZE_GROW, mMissionUIObjects[i].anchoredPosition3D.z);
                }
            }
            else
            {
                //offset the extra missions between us and the previous selection
                for(int i = missionID + 1; i < mCurrentSelectedMission + 1; i++)
                {
                    mMissionUIObjects[i].anchoredPosition3D = new Vector3(mMissionUIObjects[i].anchoredPosition3D.x, mMissionUIObjects[i].anchoredPosition3D.y - MISSION_SIZE_GROW, mMissionUIObjects[i].anchoredPosition3D.z);
                }
            }

            //activate us
            ToggleMissionMode(mMissionUIObjects[missionID]);
        }

        mCurrentSelectedMission = missionID;
    }

    void OnMissionSelected(int missionID)
    {
        GameState.AddMission(mAvailibleMissions[missionID]);
        //do not remove from availible missions as this messes up indexing

        //remove from UI
        mMissionUIObjects[missionID].gameObject.SetActive(false);
        
        for(int i = missionID+1; i<mMissionUIObjects.Length; i++)
        {
            mMissionUIObjects[i].anchoredPosition3D = new Vector3(mMissionUIObjects[i].anchoredPosition3D.x, mMissionUIObjects[i].anchoredPosition3D.y + mMissionUIObjects[missionID].rect.height + UI_PADDING, mMissionUIObjects[i].anchoredPosition3D.z);
        }

        mCurrentSelectedMission = -1;
    }

    void OnCompletedMissionClicked(int missionID)
    {
        GameState.CompleteMission(missionID);
    }

    void OnAddonSelected(RectTransform uiElm, Addon addon, Addon parent, Addon.AttachPosition pos)
    {
        Button butt = uiElm.GetComponent<Button>();

        if (mCurrentlySelectedShipAddon != null)
            mCurrentlySelectedShipAddon.colors = buildColorBlock(NormalColor);

        butt.colors = buildColorBlock(SelectedColor);
        mCurrentlySelectedShipAddon = butt;

        //activate the required buttons
        SellAddonButton.interactable = true;
        SellAddonButton.GetComponentInChildren<Text>().text = "Sell (cr. " + addon.getValue() + ")";

        /* we can only move to a place if
        1. there is an addon to replace there
        2. the posiioning is compatible */
        Addon bottom = addon.getAttachment(Addon.AttachPosition.Bottom);
        if (bottom != null && bottom.canAttach(Addon.AttachPosition.Bottom) && addon.canAttach(Addon.AttachPosition.Top))
            DownAddonButton.interactable = true;
        else
            DownAddonButton.interactable = false;

        //is our parent above us??
        if(pos == Addon.AttachPosition.Bottom && parent != null)
        {
            if (parent.canAttach(Addon.AttachPosition.Top) && addon.canAttach(Addon.AttachPosition.Bottom))
                UpAddonButton.interactable = true;
            else
                UpAddonButton.interactable = false;

            if (parent.canAttach(Addon.AttachPosition.Left) && addon.canAttach(Addon.AttachPosition.Right) && addon.getAttachment(Addon.AttachPosition.Left) == null)
                LeftAddonButton.interactable = true;
            else
                LeftAddonButton.interactable = false;

            if (parent.canAttach(Addon.AttachPosition.Right) && addon.canAttach(Addon.AttachPosition.Left) && addon.getAttachment(Addon.AttachPosition.Right) == null)
                RightAddonButton.interactable = true;
            else
                RightAddonButton.interactable = false;
        }
    }

    void OnMovePart(int direction)
    {
        throw new System.NotImplementedException();
    }

    void OnClickedEmpty(Addon parent, Addon.AttachPosition pos, GameObject self)
    {
        parent.attach(pos, mAvailibleAddons[mCurrentlySelectedStoreAddon]);

        RectTransform emptyTransform = self.GetComponent<RectTransform>();
        GenerateShipLayout(mAvailibleAddons[mCurrentlySelectedStoreAddon], AddonListContainer, emptyTransform.anchoredPosition3D.x, emptyTransform.anchoredPosition3D.y + (emptyTransform.rect.height / 2) + UI_PADDING, parent, pos);
        mAddonUIElements[mCurrentlySelectedStoreAddon].gameObject.SetActive(false);

        mCurrentlySelectedStoreAddon = -1;

        //TODO: add to lists of ship addons

        //disable buying mode
        mIsBuying = false;
        ToggleEmptys(false);
        self.SetActive(false);
        BuyAddonButton.interactable = false;
    }

    void OnClickedStoreAddon(int index)
    {
        if(mCurrentlySelectedStoreAddon != -1)
            mAddonUIElements[mCurrentlySelectedStoreAddon].GetComponent<Button>().colors = buildColorBlock(NormalColor);

        mCurrentlySelectedStoreAddon = index;
        mAddonUIElements[index].GetComponent<Button>().colors = buildColorBlock(SelectedColor);

        BuyAddonButton.GetComponentInChildren<Text>().text = "Buy (cr. " + mAvailibleAddons[index].getPrice() + ")";
        BuyAddonButton.interactable = true;
    }

    public void OnClickedBuyButton()
    {
        if(mIsBuying)
        {
            mIsBuying = false;
            BuyAddonButton.GetComponentInChildren<Text>().text = "Buy";
            BuyAddonButton.interactable = false;

            mAddonUIElements[mCurrentlySelectedStoreAddon].GetComponent<Button>().colors = buildColorBlock(NormalColor);
            mCurrentlySelectedStoreAddon = -1;

            ToggleEmptys(false);
        }
        else
        {
            mIsBuying = true;
            BuyAddonButton.GetComponentInChildren<Text>().text = "Cancel";

            ToggleEmptys(true);
        }
    }

    void ToggleEmptys(bool show)
    {
        foreach (RectTransform rt in mShipLayoutUIElements)
            rt.GetComponent<Button>().interactable = !show;

        foreach (RectTransform rt in mShipLayoutEmptySlots)
            rt.GetComponent<Button>().interactable = show;
    }

    ColorBlock buildColorBlock(Color baseColor)
    {
        ColorBlock colors = new ColorBlock();
        colors.normalColor = baseColor;
        colors.highlightedColor = baseColor * 1.1f;
        colors.pressedColor = baseColor * 0.9f;
        colors.disabledColor = baseColor * 0.2f;

        colors.colorMultiplier = 1;

        return colors;
    }
    
    Mission[] GetRandomMissions(int amount)
    {
        Mission[] missions = new Mission[amount];

        for (int i = 0; i < amount; i++)
        {
            missions[i] = new Mission("M" + i, "A randomised mission", Mission.MissionType.Cargo, Mission.Character.Steve_the_Rifleman, World.Egoras);
        }

        return missions;
    }

    Addon[] GetRandomAddons(int amount)
    {
        Addon[] addons = new Addon[amount];

        for (int i = 0; i < amount; i++)
        {
            addons[i] = GetRandomAddon(Random.Range(0, 4));
        }

        return addons;
    }

    Addon GetRandomAddon(int index)
    {
        switch (index)
        {
            case 0:
                return new Mk1Cargo();
            case 1:
                return new Mk1Engine();
            case 2:
                return new Mk1EngineMount();
            case 3:
                return new Mk1FlightDeck();
            default:
                return new Mk2Engine();
        }
    }
}
