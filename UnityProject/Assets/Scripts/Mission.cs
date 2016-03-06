//base class for all missions
using System;

public abstract class Mission {

    private int mCount = 0;
    private int mTarget = 1;

    public Planet TargetPlanet { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int ReputationValue { get; private set; }
    public int Reward { get; private set; }

    public Mission(string name, string description, Planet targetPlanet, int reward=100, int reputationValue=100, int reqCount=1)
    {
        Name = name;
        Description = description;
        ReputationValue = reputationValue;
        Reward = reward;
        TargetPlanet = targetPlanet;
        mTarget = reqCount;
    }

    //are we in the right state to complete this mission
    public virtual bool isMissionCompletable()
    {
        if (GameState.CurrentPlanet != TargetPlanet || mCount < mTarget)
            return false;

        return true;
    }

    //we have done the task of the mission one time
    private void addMissionCount(int amount = 1)
    {
        mCount += amount;
    }

    public abstract void TakeMission();

    public abstract void Complete();

    public override string ToString()
    {
        string s = "";
        s += Name + ",";
        s += Description + ",";
        s += ReputationValue + ",";
        s += Reward + ",";
        s += TargetPlanet.Name + ",";
        s += mTarget;
        return s;
    }

    public static Mission ParseMission(string s)
    {
        string[] parts = s.Split(',');

        if (parts[0] == "1")
            return CargoMission.ParseMission(s);

        return null;
    }

    public virtual bool isMissionTakeable() { return true; }
}

//cargo missions
public class CargoMission : Mission
{
    private CargoDef.CargoType cType;
    private int cAmount;

    public CargoMission(string name, string description, Planet targetPlanet, CargoDef.CargoType cargo, int amount=1, int reward=100, int reputationValue=100) : base(name, description, targetPlanet, reward, reputationValue, 0)
    {
        cType = cargo;
        cAmount = amount;
    }

    public override bool isMissionCompletable()
    {
        return (GameState.PlayerCargo.GetAmount(cType) >= cAmount) && (base.isMissionCompletable());
    }

    public override void TakeMission()
    {
        GameState.PlayerCargo.Add(cType, cAmount);
        UIManager.UISystem.ChangeCargoValue(GameState.PlayerCargoPercentage);
    }

    public override void Complete()
    {
        GameState.PlayerCargo.Remove(cType, cAmount);
        UIManager.UISystem.ChangeCargoValue(GameState.PlayerCargoPercentage);
    }

    public override string ToString()
    {
        string s = "1,";
        s += (int)cType + ":" + cAmount + ",";
        s += base.ToString();
        return s;
    }

    public static new Mission ParseMission(string s)
    {
        string[] parts = s.Split(',');

        if (parts[0] != "1")
            throw new Exception("Invalid mission type for parse");

        //parse out our cargo target
        string[] cargoParts = parts[1].Split(':');
        CargoDef.CargoType cType = (CargoDef.CargoType)int.Parse(cargoParts[0]);
        int cAmount = int.Parse(cargoParts[1]);

        string name = parts[2];
        string description = parts[3];
        int reputation = int.Parse(parts[4]);
        int reward = int.Parse(parts[5]);
        Planet target = World.getPlanetFromName(parts[6]);
        //ignore count as it is not required here

        return new CargoMission(name, description, target, cType, cAmount, reward, reputation);
    }

    public override bool isMissionTakeable()
    {
        if (GameState.PlayerMaxCargo - GameState.PlayerCargo.GetAmount() >= cAmount)
            return true;

        return false;
    }
}