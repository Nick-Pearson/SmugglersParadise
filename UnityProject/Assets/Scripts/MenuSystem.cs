using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour {
    public RectTransform MissionListContainer;
    public RectTransform CompleteListContainer;
    public RectTransform AddonListContainer;
    public RectTransform ShopListContainer;

    public RectTransform MissionUIPrefab;
    public RectTransform MissionCompleteUIPrefab;
    public RectTransform AddonPrefab;

    public Text PlanetWelcomeText;
    public Text ShipNameText;
    public Text MoneyText;
    public Text DamageText;
    public Text FuelText;

    private const int MISSION_UI_PADDING = 5;
    private const int MISSION_SIZE_GROW = 30; //how big to make the mission when we select it
    private RectTransform[] mMissionUIObjects;
    private Mission[] mAvailibleMissions;
    private RectTransform[] mCompletedMissionUIObjects;
    private int mCurrentSelectedMission = -1;

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

    public RectTransform[] GenerateMissionList(Mission[] missions, RectTransform container)
    {
        RectTransform[] array = new RectTransform[missions.Length];

        //for each mission create a ui element to display it's data
        for(int i = 0; i < missions.Length; i++)
        {
            RectTransform missionUI = Instantiate(MissionUIPrefab) as RectTransform;
            missionUI.SetParent(container, false);
            missionUI.anchoredPosition3D = new Vector3(missionUI.anchoredPosition3D.x, (-(i+.5f) * missionUI.rect.height) - ((i+1) * MISSION_UI_PADDING), missionUI.anchoredPosition3D.z);
            missionUI.gameObject.name = i + "";

            UIFromMission(missions[i], missionUI);
            array[i] = missionUI;
        }

        //set the container height so that scrollbars work correctly
        container.offsetMin = new Vector2(container.offsetMin.x, -(missions.Length * (MissionUIPrefab.rect.height + MISSION_UI_PADDING)) - MISSION_UI_PADDING);

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
            mMissionUIObjects[i].anchoredPosition3D = new Vector3(mMissionUIObjects[i].anchoredPosition3D.x, mMissionUIObjects[i].anchoredPosition3D.y + mMissionUIObjects[missionID].rect.height + MISSION_UI_PADDING, mMissionUIObjects[i].anchoredPosition3D.z);
        }

        mCurrentSelectedMission = -1;
    }

    void OnCompletedMissionClicked(int missionID)
    {
        GameState.CompleteMission(missionID);
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
}
