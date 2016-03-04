using System.Collections.Generic;

public class CargoDef {
    //enum is order of value Low to High
    public enum CargoType
    {
        Livestock,
        Oil,
        Rubbish,
        Aid,
        Coffee,
        Gold,
        Lemonade,
        Pirate_Flags,
        Booty,
        Cannon
    }

    private Dictionary<CargoType, int> mCargoDef = new Dictionary<CargoType, int>();

    //do we have this cargo?s
    public bool Contains(CargoType cargo)
    {
        return mCargoDef.ContainsKey(cargo);
    }

    //add new cargo to our def
    public void Add(CargoType cargo, int amount = 1)
    {
        if (!Contains(cargo))
            mCargoDef.Add(cargo, amount);
        else
            mCargoDef[cargo] += amount;
    }

    //take away some cargo from our def
    public void Remove(CargoType cargo, int amount = 1)
    {
        if(Contains(cargo))
        {
            if(mCargoDef[cargo] > amount)
                mCargoDef[cargo] -= amount;
            else
                mCargoDef[cargo] = 0;
        }
    }

    //amount of one type of cargo
    public int GetAmount(CargoType cargo)
    {
        if (!Contains(cargo))
            return 0;

        return mCargoDef[cargo];
    }

    //what is our total cargo weight
    public float GetAmount()
    {
        float amount = 0;

        foreach(CargoType t in mCargoDef.Keys)
        {
            amount += mCargoDef[t];
        }

        return amount;
    }

    //utility to get the raw cargo storage
    public Dictionary<CargoType, int> GetCargo()
    {
        return mCargoDef;
    }
}
