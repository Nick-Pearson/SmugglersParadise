using UnityEngine;

public struct Planet
{
    public Planet(string name, Vector2 location, float atmosphereSize = 100.0f, float gravityScale = 1.0f)
    {
        Name = name;
        AtmosphereSize = atmosphereSize;
        GravityScale = gravityScale;
        Location = location;
    }

    //properties
    public float AtmosphereSize {  get; private set; }
    public float GravityScale { get; private set; }
    public Vector2 Location { get; private set; }
    public string Name { get; private set; }


    //these are a paint but we have to override them to make the planet struct useable
    public static bool operator !=(Planet a, Planet b)
    {
        return !(a == b);
    }

    public static bool operator ==(Planet a, Planet b)
    {
        return a.Name == b.Name;
    }
    
    public override bool Equals(object o)
    {
        return o is Planet && this == (Planet)o;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();

    }
}
