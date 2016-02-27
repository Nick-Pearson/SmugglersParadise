﻿using UnityEngine;
using System.Collections.Generic;

//this class captures all information that should be saved and loaded with the game
public static class GameState {
    private static string mGameName;

    private static List<Mission> mActiveMissions;

    //publically accessable properties
    public static Planet CurrentPlanet;
    public static string ShipName;

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
}
