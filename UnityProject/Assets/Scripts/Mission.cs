public class Mission {
    //TODO: Perhaps move into a class?
    public enum Character
    {
        Player,

        //friendlies
        Trader_Joe,
        Trader_Pete,
        Steve_the_Rifleman,

        //pirates
        Bluebeard,
        Davoom
    }

    public enum MissionType
    {
        Cargo,
        Passenger,
        Attack
    }

    private Planet mTargetPlanet;
    private int mCount = 0;
    private int mTarget = 1;

    public string Name { get; private set; }
    public string Description { get; private set; }
    public int ReputationValue { get; private set; }
    public int Reward { get; private set; }
    public Character Client { get; private set; }
    public MissionType Type { get; private set; }

    public Mission(string name, string description, MissionType mtype, Character client, Planet targetPlanet, int reward=100, int reputationValue=100, int reqCount=1)
    {
        Name = name;
        Description = description;
        Type = mtype;
        Client = client;
        ReputationValue = reputationValue;
        Reward = reward;
        mTargetPlanet = targetPlanet;
        mTarget = reqCount;
    }

    //are we in the right state to complete this mission
    public bool isMissionCompletable()
    {
        if (GameState.CurrentPlanet != mTargetPlanet || mCount < mTarget)
            return false;

        return true;
    }

    //we have done the task of the mission one time
    public void addMissionCount(int amount = 1)
    {
        mCount += amount;
    }
}
