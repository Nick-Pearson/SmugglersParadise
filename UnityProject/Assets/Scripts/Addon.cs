//base class for all ship upgrades
public abstract class Addon {
    public void ApplyChanges(PlayerCharacter player)
    {
        player.ShipAddonMass += getMass();
        applyBuffs(player);
    }
    //the mass of this addon in tons
    public abstract int getMass();

    //the resale value of this addon
    public abstract int getValue();

    //the price of this addon
    public abstract int getPrice();

    //apply any addional property changes this addon requires
    public abstract void applyBuffs(PlayerCharacter player);
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
        player.PlayerMaxFuel += 800;
        player.PlayerMaxFuelBurn += 200f;
    }

    public override int getMass() {  return 80; }

    public override int getValue() { return 10; }

    public override int getPrice() { return 50; }
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

    public override int getValue() { return 15; }

    public override int getPrice() { return 100; }
}

//           FLIGHT DECKS
//======================================

//I made this data up
public class Mk1FlightDeck : Addon
{
    public override void applyBuffs(PlayerCharacter player) { return; }

    public override int getMass() { return 30; }

    public override int getValue() { return 100; }

    public override int getPrice() { return 250; }
}

//            STRUCTURE
//======================================

//I made this data up
public class RearEngineMount : Addon
{
    private Engine mEngineLeft;
    private Engine mEngineRight;

    public RearEngineMount(Engine eL, Engine eR)
    {
        mEngineLeft = eL;
        mEngineRight = eR;
    }

    public override void applyBuffs(PlayerCharacter player)
    {
        mEngineLeft.applyBuffs(player);
        mEngineRight.applyBuffs(player);
    }

    public override int getMass() { return 20 + mEngineLeft.getMass() + mEngineRight.getMass(); }

    public override int getValue() { return 10; }

    public override int getPrice() { return 45; }
}