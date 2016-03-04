//base class for all missions
using System;

public abstract class Mission {

    private Planet mTargetPlanet;
    private int mCount = 0;
    private int mTarget = 1;

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
        mTargetPlanet = targetPlanet;
        mTarget = reqCount;
    }

    //are we in the right state to complete this mission
    public virtual bool isMissionCompletable()
    {
        if (GameState.CurrentPlanet != mTargetPlanet || mCount < mTarget)
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
}