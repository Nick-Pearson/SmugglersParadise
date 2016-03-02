using UnityEngine;

public class ShipGraphics : MonoBehaviour {
    //place to store references to our addons
    //TODO: fill with default addons for now
    public Addon BaseAddon { get; private set; }

    private GameObject[] mEngineEffects = new GameObject[0];
    private PlayerCharacter mPlayer;

    void Start()
    {
        BaseAddon = GameState.PlayerAddons;
        GenerateShipGraphics();
        mPlayer = GetComponent<PlayerCharacter>();
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
        if (a is Empty)
            return 0;

        float width, height;
        GameObject instance;
        if (!(a is Base))
        {
            //cache the properties we will need
            width = a.getWidth();
            height = a.getHeight();

            GameObject art = Resources.Load<GameObject>("Player/" + a.getName());
            instance = Instantiate(art, new Vector3(offsetX, offsetY, 0), Quaternion.identity) as GameObject;
            instance.transform.parent = parent.transform;
        }
        else
        {
            instance = parent;
            width = 0;
            height = 0;
        }


        Addon[] attachments = a.getAttachments();

        if (attachments.Length == 0)
            return height;

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
