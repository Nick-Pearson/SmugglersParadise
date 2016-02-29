using UnityEngine;

public class World {
    //get the distance between two planets
    public static float getDistance(Planet p1, Planet p2)
    {
        return (p1.Location - p2.Location).magnitude;
    }

    //find a specific planet (perhaps to use when loading from savegame??)
    public static Planet getPlanetFromName(string name)
    {
        switch(name)
        {
            case "Any":
                return Any;
            case "Space":
                return Space;
            case "Egoras":
                return Egoras;
            case "Hellzine":
                return Hellzine;
            default:
                throw new System.Exception("Referenced planet (" + name + ") could not be found");
        }
    }

    //an 'any' planet wildcard object for use in missions
    public static Planet Any = new Planet("Any", new Vector2(0, 0));

    //special case 'planet' where we are in space
    public static Planet Space = new Planet("Space", new Vector2(0, 0));

    //defined planets in our world
    public static Planet Egoras = new Planet("Egoras", new Vector2(0, 0), 100, 1);
    public static Planet Hellzine = new Planet("Hellzine", new Vector2(500, 0), 200, 1);
}
