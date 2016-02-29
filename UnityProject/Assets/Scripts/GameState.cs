using UnityEngine;
using System.Collections.Generic;

//this class captures all information that should be saved and loaded with the game
//it must inherit from mono behaviour to survive a level reload...
public class GameState : MonoBehaviour {
    private static string mGameName;

    private static List<Mission> mActiveMissions;

    //publically accessable properties
    public static Planet CurrentPlanet;
    public static string ShipName;
    public static float PlayerFuelPercentage { get { return mPlayerMaxFuel == 0 ? 0 : mPlayerFuel / mPlayerMaxFuel; } }
    public static float PlayerCargoPercentage { get { return mPlayerMaxCargo == 0 ? 0 : mPlayerCargo / mPlayerMaxCargo; } }

    //private properties
    private static float mPlayerFuel;
    private static float mPlayerMaxFuel;
    private static float mPlayerCargo;
    private static float mPlayerMaxCargo;

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

    public static void AddMission(Mission m)
    {
        mActiveMissions.Add(m);
    }

    //capture the properties from the player itself
    public static void UpdatePlayerProperties(PlayerCharacter pc)
    {
        mPlayerFuel = pc.PlayerFuelAmount;
        mPlayerMaxFuel = pc.PlayerMaxFuel;
        mPlayerCargo = pc.ShipCargoMass;
        mPlayerMaxCargo = pc.ShipMaxCargo;
    }
}
