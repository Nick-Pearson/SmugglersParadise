using System.Collections.Generic;

//this object captures the slots for addons and what they are used for
//To be used for saving the game
public class Ship
{
    public enum Licence
    {
        Cargo_1,
        Cargo_2,
        Cargo_3,
        Cargo_4,
        Passenger_1,
        Passenger_2,
        Passenger_3
    }

    private Addon[] mAddons;
    private List<Licence> mLicences = new List<Licence>();

    public Ship(params Addon[] addons)
    {
        mAddons = addons;
        mLicences.Add(Licence.Cargo_1);
    }

    public void addLicence(Licence l)
    {
        if (!mLicences.Contains(l))
            mLicences.Add(l);
    }

    public bool hasLicence(Licence query)
    {
        foreach (Licence l in mLicences)
        {
            if (l == query)
                return true;
        }

        return false;
    }

    public void ApplyChanges(PlayerCharacter player)
    {
        foreach (Addon a in mAddons)
        {
            a.ApplyChanges(player);
        }
    }

    public int getMass()
    {
        int mass = 0;
        foreach (Addon a in mAddons)
        {
            mass += a.getMass();
        }

        return mass;
    }
}
