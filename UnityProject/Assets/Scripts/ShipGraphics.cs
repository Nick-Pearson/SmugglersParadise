using UnityEngine;

public class ShipGraphics : MonoBehaviour {
    //place to store references to our addons
    //TODO: fill with default addons for now
    public Addon BaseAddon { get; private set; }

    private GameObject[] mEngineEffects = new GameObject[0];
    private PlayerCharacter mPlayer;

    void Awake()
    {
        BaseAddon = buildBasicShip();
        GenerateShipGraphics();
        mPlayer = GetComponent<PlayerCharacter>();
    }

    Addon buildBasicShip()
    {
        Addon fd = new Mk1FlightDeck();
        Addon cg = new Mk1Cargo();
        Addon cg2 = new Mk1Cargo();
        Addon em = new Mk1EngineMount();
        Addon e1 = new Mk1Engine();
        Addon e2 = new Mk1Engine();
        Addon em2 = new Mk1EngineMount();
        Addon e3 = new Mk1Engine();
        Addon e4 = new Mk1Engine();

        Addon e5 = new Mk2Engine();

        em.attach(Addon.AttachPosition.Left, e1);
        em.attach(Addon.AttachPosition.Right, e2);
        em.attach(Addon.AttachPosition.Bottom, e5);

        cg.attach(Addon.AttachPosition.Bottom, em);

        cg2.attach(Addon.AttachPosition.Bottom, cg);

        em2.attach(Addon.AttachPosition.Left, e3);
        em2.attach(Addon.AttachPosition.Right, e4);

        em2.attach(Addon.AttachPosition.Bottom, cg2);

        fd.attach(Addon.AttachPosition.Bottom, em2);

        return fd;
    }

    void GenerateShipGraphics()
    {
        GameObject baseObject = new GameObject("ShipGraphics");
        baseObject.transform.parent = transform;
        float height = BuildAddonGraphics(0, 0, BaseAddon, baseObject);
        baseObject.transform.Translate(0, height, 0);

        //find all our engine effects
        mEngineEffects = GameObject.FindGameObjectsWithTag("Effect");
    }

    float BuildAddonGraphics(float offsetX, float offsetY, Addon a, GameObject parent)
    {
        if (a == null)
            return 0;

        //cache the properties we will need
        float width = a.getWidth();
        float height = a.getHeight();
        
        GameObject art = Resources.Load<GameObject>("Player/" + a.getName());
        GameObject instance = Instantiate(art, new Vector3(offsetX, offsetY, 0), Quaternion.identity) as GameObject;
        instance.transform.parent = parent.transform;

        Addon[] attachments = a.getAttachments();

        if (attachments.Length == 0)
            return height;

        BuildAddonGraphics(offsetX, offsetY + height, attachments[0], instance);
        float h1 = BuildAddonGraphics(offsetX, offsetY - height, attachments[1], instance);
        float h2 = BuildAddonGraphics(offsetX + width, offsetY, attachments[2], instance);
        float h3 = BuildAddonGraphics(offsetX - width, offsetY, attachments[3], instance);

        //the total height of the unit plus addons
        float normHeight = height + h1;
        //the height of the edge cases extending bellow the addon
        float edgeHeight = h2 > h3 ? h2 : h3;

        //are our edge addons taller than us?
        return normHeight > edgeHeight ? normHeight : edgeHeight;
    }

    public void UpdateThrottle(float val)
    {
        if (mPlayer.PlayerFuelAmount == 0)
            val = 0;

        foreach (GameObject go in mEngineEffects)
        {
            go.transform.localScale = new Vector3(go.transform.localScale.x, val, go.transform.localScale.z);
        }
    }
}
