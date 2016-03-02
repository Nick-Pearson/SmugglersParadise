using UnityEngine;
using System.Collections.Generic;
using System;

//this class captures all information that should be saved and loaded with the game
//it must inherit from mono behaviour to survive a level reload...
public class GameState : MonoBehaviour {
    private static string mGameName;

    private static List<Mission> mActiveMissions;
    private static List<Mission> mCompleteableMissions;

    //publically accessable properties
    public static Planet CurrentPlanet;
    public static string ShipName;
    public static float PlayerFuelPercentage { get { return PlayerMaxFuel == 0 ? 0 : PlayerFuel / PlayerMaxFuel; } }
    public static float PlayerCargoPercentage { get { return PlayerMaxCargo == 0 ? 0 : PlayerCargo / PlayerMaxCargo; } }

    public static float PlayerFuel;
    public static float PlayerMaxFuel;
    public static float PlayerCargo;
    public static float PlayerMaxCargo;
    public static Addon PlayerAddons;

    public static int PlayerMoney { get; private set; }
    public static int PlayerReputaion { get; private set; }

    //game constants
    public const float FUEL_COST = 1.5f;

    //if there is no data create a new game
    void Awake()
    {
        if (PlayerAddons == null)
        {
            Debug.Log("No data detected, creating a new game");
            CreateNewGame();
        }
    }

    //global game properties
    public enum Column { One=0, Two, Three, NumColumns }

    public static void LoadGame(string GameName)
    {
        mGameName = GameName;
    }

    public static void SaveGame(string GameName)
    {

    }

    //if no name is passed assume we are overwriting the current savegame file
    public static void SaveGame()
    {
        SaveGame(mGameName);
    }

    //accessor to missions
    public static Mission[] GetActiveMissions()
    {
        return mActiveMissions.ToArray();
    }

    public static Mission[] GetCompleteableMissions()
    {
        mCompleteableMissions = new List<Mission>();

        foreach(Mission m in mActiveMissions)
        {
            if (m.isMissionCompletable())
                mCompleteableMissions.Add(m);
        }

        return mCompleteableMissions.ToArray();
    }

    public static void AddMission(Mission m)
    {
        mActiveMissions.Add(m);
    }

    public static void CompleteMission(int index)
    {
        Mission m = mCompleteableMissions[index];

        //apply that missions buffs
        PlayerMoney += m.Reward;
        PlayerReputaion += m.ReputationValue;

        //for now we keep that mission in the list of completeables
        //otherwise the indexing will be skewed
        mActiveMissions.Remove(m);
    }

    //capture the properties from the player itself
    public static void UpdatePlayerProperties(PlayerCharacter pc)
    {
        PlayerFuel = pc.PlayerFuelAmount;
        PlayerMaxFuel = pc.PlayerMaxFuel;
        PlayerCargo = pc.ShipCargoMass;
        PlayerMaxCargo = pc.ShipMaxCargo;
        PlayerAddons = pc.GetComponent<ShipGraphics>().BaseAddon;
    }

    //create a new profile
    public static void CreateNewGame()
    {
        mActiveMissions = new List<Mission>();
        CurrentPlanet = World.Egoras;

        ShipName = "The Jolly Hamburger";
        PlayerFuel = 800;
        PlayerMaxFuel = 800;
        PlayerCargo = 0;
        PlayerMaxCargo = 50;
        PlayerMoney = 1000;

        PlayerAddons = buildBasicShip();
    }

    static Addon buildBasicShip()
    {
        Addon fd = new Mk1FlightDeck();
        Addon cg = new Mk1Cargo();
        Addon em = new Mk1EngineMount();
        Addon e1 = new Mk1Engine();
        Addon e2 = new Mk1Engine();

        em.attach(Addon.AttachPosition.Left, e1);
        em.attach(Addon.AttachPosition.Right, e2);

        cg.attach(Addon.AttachPosition.Bottom, em);

        fd.attach(Addon.AttachPosition.Bottom, cg);

        return fd;
    }
}
