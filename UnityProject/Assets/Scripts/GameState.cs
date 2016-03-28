using UnityEngine;
using System.Collections.Generic;
using System;

//this class captures all information that should be saved and loaded with the game
//it must inherit from mono behaviour to survive a level reload...
public class GameState : MonoBehaviour {
    //version refers to the version of the savegame
    public const string VERSION = "1.2";

    private static string mGameName;

    private static List<Mission> mActiveMissions;
    private static List<Mission> mCompleteableMissions;

    //publically accessable properties
    public static Planet CurrentPlanet;
    public static Planet Destination;
    public static string ShipName;
    public static float PlayerFuelPercentage { get { return PlayerMaxFuel == 0 ? 0 : PlayerFuel / PlayerMaxFuel; } }
    public static float RefuelCost { get { return Mathf.Round((PlayerMaxFuel - PlayerFuel) * FUEL_COST); } }
    public static float RepairCost { get { return Mathf.Round(PlayerDamage * REPAIR_COST); } }
    public static float PlayerCargoPercentage { get { return PlayerMaxCargo == 0 ? 0 : PlayerCargo.GetAmount() / PlayerMaxCargo; } }

    public static float PlayerFuel;
    public static float PlayerMaxFuel;
    public static CargoDef PlayerCargo;
    public static float PlayerMaxCargo;
    public static Addon PlayerAddons;
    public static float PlayerDamage;

    public static int PlayerMoney;
    public static int PlayerReputaion { get; private set; }

    //game constants
    public const float FUEL_COST = 1.5f;
    public const float REPAIR_COST = 500;
    public const float BULLET_SPEED = 15.0f;
    public const float BULLET_DAMAGE = 10;

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

        mCompleteableMissions[index].Complete();
        
        //for now we keep that mission in the list of completeables
        //otherwise the indexing will be skewed
        mActiveMissions.Remove(m);
    }

    //create a new profile
    public static void CreateNewGame()
    {
        mActiveMissions = new List<Mission>();
        CurrentPlanet = World.Egoras;
        Destination = World.Hellzine;

        ShipName = "The Jolly Hamburger";
        PlayerFuel = 800;
        PlayerMaxFuel = 800;
        PlayerCargo = new CargoDef();
        PlayerMaxCargo = 50;
        PlayerMoney = 1000;
        PlayerDamage = 0.0f;

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

        Addon b = new Base();

        b.attach(Addon.AttachPosition.Bottom, fd);

        return b;
    }

    private static string createGameString()
    {
        string s = "";

        //basic game details
        s += VERSION + ";";
        s += CurrentPlanet.Name + ";";

        //player details
        s += ShipName + ";";
        s += PlayerMoney + ";";
        s += PlayerReputaion + ";";
        s += PlayerFuel + ";";
        s += PlayerMaxFuel + ";";
        s += PlayerMaxCargo + ";";
        s += createCargoString(PlayerCargo);
        s += createAddonString(PlayerAddons);

        //missions
        s += mActiveMissions.Count + ";";
        foreach(Mission m in mActiveMissions)
        {
            s += m.ToString() + ";";
        }

        return s;
    }

    private static string createCargoString(CargoDef c)
    {
        string s = "";
        Dictionary<CargoDef.CargoType, int> def = c.GetCargo();

        foreach(CargoDef.CargoType t in def.Keys)
        {
            s += (int)t + ":";
            s += def[t] + ",";
        }
        
        return s + ";";
    }

    private static string createAddonString(Addon b)
    {
        if (b is Base)
        {
            return "Base(" + createAddonString(b.getAttachment(Addon.AttachPosition.Bottom)) + ");";
        }
        else if(b is Empty)
        {
            return "Empty,";
        }
        else
        {
            return b.getName() + "(" + createAddonString(b.getAttachment(Addon.AttachPosition.Left)) + ":" + createAddonString(b.getAttachment(Addon.AttachPosition.Right)) + ":" + createAddonString(b.getAttachment(Addon.AttachPosition.Bottom)) + "),";
        }
    }
}
