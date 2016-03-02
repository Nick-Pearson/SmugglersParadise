using System.Collections.Generic;

public class CargoDef {
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

    public bool Contains(CargoType cargo)
    {
        return mCargoDef.ContainsKey(cargo);
    }

    public void Add(CargoType cargo, int amount = 1)
    {
        if (!Contains(cargo))
            mCargoDef.Add(cargo, amount);
        else
            mCargoDef[cargo] += amount;
    }

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

    public int GetAmount(CargoType cargo)
    {
        if (!Contains(cargo))
            return 0;

        return mCargoDef[cargo];
    }

    public float GetAmount()
    {
        float amount = 0;

        foreach(CargoType t in mCargoDef.Keys)
        {
            amount += mCargoDef[t];
        }

        return amount;
    }
}
