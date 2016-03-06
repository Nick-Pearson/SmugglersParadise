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
        Planet[] planets = getAllPlanets();

        foreach(Planet p in planets)
        {
            if (p.Name == name)
                return p;
        }
        throw new System.Exception("Referenced planet (" + name + ") could not be found");
    }

    //get a random planet
    public static Planet getRandomPlanet(Planet notInclude)
    {
        Planet selection;

        do
        {
            selection = getRandomPlanet();
        } while (selection == notInclude);

        return selection;
    }

    public static Planet getRandomPlanet()
    {
        Planet[] planets = getAllPlanets();

        return planets[Random.Range(0, planets.Length)];
    }

    public static Planet[] getAllPlanets()
    {
        return new Planet[6] { Egoras, Hellzine, TreasureIsland, Moocombe, Limpet, Rubnub };
    }

    //defined planets in our world
    public static Planet Egoras = new Planet("Egoras", new Vector2(0, 0), 100, 1);
    public static Planet Hellzine = new Planet("Hellzine", new Vector2(500, 1550), 200, 1);
    public static Planet TreasureIsland = new Planet("Treasure Island", new Vector2(10000, -440), 150, 1.3f);
    public static Planet Moocombe = new Planet("Moocombe", new Vector2(1500, 1500), 100, 0.4f);
    public static Planet Limpet = new Planet("Limpet", new Vector2(-15000, 0), 400, 1.1f);
    public static Planet Rubnub = new Planet("Rubnub", new Vector2(-10000, 750), 100, 0.8f);
}
