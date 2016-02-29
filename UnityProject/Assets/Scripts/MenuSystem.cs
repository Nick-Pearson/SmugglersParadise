using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour {
    public RectTransform Canvas;
    public RectTransform MissionListContainer;
    public RectTransform MissionUIPrefab;

    private const int MISSION_UI_PADDING = 5;
    private const int MISSION_SIZE_GROW = 30; //how big to make the mission when we select it
    private RectTransform[] mMissionUIObjects;
    private int mCurrentSelectedMission = -1;

    void Start()
    {
        GenerateMissionList(GetRandomMissions(50));
        PopulateUI();
    }

    void PopulateUI()
    {
        UIManager.UISystem.ChangeCargoValue(GameState.PlayerCargoPercentage);
        UIManager.UISystem.ChangeFuelValue(GameState.PlayerFuelPercentage);
    }

    public void GenerateMissionList(Mission[] missions)
    {
        mMissionUIObjects = new RectTransform[missions.Length];

        //for each mission create a ui element to display it's data
        for(int i = 0; i < missions.Length; i++)
        {
            RectTransform missionUI = Instantiate(MissionUIPrefab) as RectTransform;
            missionUI.SetParent(MissionListContainer, false);
            missionUI.anchoredPosition3D = new Vector3(missionUI.anchoredPosition3D.x, (-(i+.5f) * missionUI.rect.height) - ((i+1) * MISSION_UI_PADDING), missionUI.anchoredPosition3D.z);
            missionUI.gameObject.name = i + "";
            mMissionUIObjects[i] = missionUI;

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { OnMissionClicked(missionUI); });

            missionUI.GetComponent<EventTrigger>().triggers.Add(entry);

            UIFromMission(missions[i], missionUI);
        }

        //set the container height so that scrollbars work correctly
        MissionListContainer.offsetMin = new Vector2(MissionListContainer.offsetMin.x, -(missions.Length * (MissionUIPrefab.rect.height + MISSION_UI_PADDING)) - MISSION_UI_PADDING);
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

    void OnMissionClicked(RectTransform missionObject)
    {
        //parse the mission ID
        int missionID = int.Parse(missionObject.gameObject.name);

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
