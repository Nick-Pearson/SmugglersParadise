//base class for all ship upgrades

public abstract class Addon {
    //nice enum to shield indexing the bool/Addon array ourselves
    public enum AttachPosition
    {
        Top=0,
        Bottom,
        Left,
        Right
    }

    private Addon[] mAttachments = new Addon[0] {};

    public void ApplyChanges(PlayerCharacter player)
    {
        player.ShipAddonMass += getMass();
        applyBuffs(player);

        foreach(Addon a in mAttachments)
        {
            //TODO: null comparisons are expensive, consider replacing
            if(a != null)
            {
                a.ApplyChanges(player);
            }
        }
    }
    //the mass of this addon in tons
    public abstract int getMass();

    //the resale value of this addon
    public abstract int getValue();

    //the price of this addon
    public abstract int getPrice();

    //apply any addional property changes this addon requires
    public abstract void applyBuffs(PlayerCharacter player);

    //name of addon for graphc fetching purposes
    public abstract string getName();

    //the height of the object (world units, not pixels)
    public abstract float getHeight();
    //the width of the object
    public abstract float getWidth();

    //where can things be attached
    public virtual bool[] canAttach() { return new bool[4] { true, true, true, true }; }
    public virtual bool canAttach(AttachPosition pos) { return canAttach()[(int)pos]; }

    //what is attached (one addon per slot)
    public Addon[] getAttachments() { return mAttachments; }
    public Addon getAttachment(AttachPosition pos) { return mAttachments.Length == 0 ? null : mAttachments[(int)pos]; }

    //attach an addon to us
    public void attach(int pos, Addon a)
    {
        //check if we are allowed to attach here
        if(canAttach()[pos])
        {
            if (mAttachments.Length == 0)
                mAttachments = new Addon[4] { null, null, null, null };

            mAttachments[pos] = a;
        }
        else
        {
            throw new System.Exception("You cannot attach an addon here");
        }
    }

    public void attach(AttachPosition pos, Addon a)
    {
        attach((int)pos, a);
    }
}

//base class for all engines
public abstract class Engine : Addon {}

//              ENGINES
//======================================

//This is based (roughly) on the Saturn V Rocket First and Second Stage
//Data from Wikipedia
public class Mk1Engine : Engine
{
    public override void applyBuffs(PlayerCharacter player)
    {
        player.MaxThrustForce += 8500;
        player.PlayerMaxFuel += 400;
        player.PlayerMaxFuelBurn += 50f;
    }

    public override int getMass() {  return 80; }
    public override int getValue() { return 100; }
    public override int getPrice() { return 500; }
    public override string getName() { return "Mk1 Engine"; }
    public override float getHeight() { return 1.15f; }
    public override float getWidth() { return 1; }
    public override bool[] canAttach() { return new bool[4] { false, false, true, true }; }
}

public class Mk2Engine : Engine
{
    public override void applyBuffs(PlayerCharacter player)
    {
        player.MaxThrustForce += 20000;
        player.PlayerMaxFuel += 0;
        player.PlayerMaxFuelBurn += 100f;
    }

    public override int getMass() { return 80; }
    public override int getValue() { return 500; }
    public override int getPrice() { return 2500; }
    public override string getName() { return "Mk2 Engine"; }
    public override float getHeight() { return 1.15f; }
    public override float getWidth() { return 1; }
    public override bool[] canAttach() { return new bool[4] { true, false, false, false }; }
}

//            CARGO BAYS
//======================================

//I made this data up
public class Mk1Cargo : Addon
{
    public override void applyBuffs(PlayerCharacter player)
    {
        player.ShipMaxCargo += 50;
    }

    public override int getMass() { return 30; }
    public override int getValue() { return 150; }
    public override int getPrice() { return 1000; }
    public override string getName() { return "Mk1 Cargo"; }
    public override float getHeight() { return 2.3f; }
    public override float getWidth() { return 1; }
    public override bool[] canAttach() { return new bool[4] { true, true, false, false }; }
}

//           FLIGHT DECKS
//======================================

//I made this data up
public class Mk1FlightDeck : Addon
{
    public override void applyBuffs(PlayerCharacter player) { return; }
    public override int getMass() { return 30; }
    public override int getValue() { return 1000; }
    public override int getPrice() { return 2500; }
    public override string getName() { return "Mk1 Flight Deck"; }
    public override float getHeight() { return 1.3f; }
    public override float getWidth() { return 1; }
    public override bool[] canAttach() { return new bool[4] { false, true, false, false }; }
}

//            STRUCTURE
//======================================

//I made this data up
public class Mk1EngineMount : Addon
{
    public override void applyBuffs(PlayerCharacter player) { }
    public override int getMass() { return 20; }
    public override int getValue() { return 100; }
    public override int getPrice() { return 450; }
    public override string getName() { return "Mk1 Engine Mount"; }
    public override float getHeight() { return 0.517f; }
    public override float getWidth() { return 1; }
}