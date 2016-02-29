using UnityEngine;
using System.Collections;

public class PlanetFactory : MonoBehaviour {
    [SerializeField] private GameObject PlanetPrefab;
    [SerializeField] private Sprite[] Clouds;
    [Range(1, 100)]
    [SerializeField] private int CloudPoolSize;
    [SerializeField] private float CloudCeiling;
    [SerializeField] private float CloudFloor;


    private GameObject[] mCloudObjects;

    private PlayerCharacter mPlayer;

    void Start()
    {
        mPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();

        GameObject p0 = Instantiate(PlanetPrefab);
        p0.name = "Origin";
        GameObject p1 = Instantiate(PlanetPrefab);
        p1.name = "Destination";
        p1.transform.Translate(0, GameLogic.TargetDistance + GameLogic.Destination.AtmosphereSize, 0);
        p1.transform.localScale = new Vector3(1,-1,1);

        SetupPlanet(p0, GameLogic.Origin);
        SetupPlanet(p1, GameLogic.Destination);

        mCloudObjects = new GameObject[CloudPoolSize];

        for(int i = 0; i < CloudPoolSize; i++)
        {
            mCloudObjects[i] = new GameObject("Cloud_" + i);
            mCloudObjects[i].transform.parent = p0.transform;
            mCloudObjects[i].transform.position = new Vector3(Random.Range(-GameLogic.ScreenBounds, GameLogic.ScreenBounds), Random.Range(CloudFloor, CloudCeiling), 0);
            mCloudObjects[i].AddComponent<SpriteRenderer>();
            SetupCloud(mCloudObjects[i]);
        }
    }

    void SetupPlanet(GameObject go, Planet p)
    {
        Transform atmos_a = go.transform.FindChild("atmosphere_a");
        Transform atmos_w = go.transform.FindChild("atmosphere_w");
        float scale = atmos_a.localScale.y * p.AtmosphereSize;
        atmos_a.localScale = new Vector3(1,scale,1);
        atmos_w.localScale = new Vector3(1, scale, 1);
    }

    void SetupCloud(GameObject go)
    {
        float scale = Random.Range(0.2f, 1.0f);
        go.transform.localScale = new Vector3(scale, scale, scale);

        SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();

        if (Random.value > 0.5f)
            renderer.sortingLayerName = "Pre-Player";
        else
            renderer.sortingLayerName = "Post Player";

        renderer.sprite = Clouds[Random.Range(0, Clouds.Length)];
    }

    void Update()
    {
        if (mPlayer.Physics.Height > GameLogic.ScreenTop)
            return;

        foreach(GameObject go in mCloudObjects)
        {
            go.transform.Translate(GameLogic.GameDeltaTime * go.transform.localScale.x, 0, 0);

            if(go.transform.position.x > GameLogic.ScreenBounds * 1.5f)
            {
                go.transform.position = new Vector3(GameLogic.ScreenBounds * -1.5f, Random.Range(CloudFloor, CloudCeiling), 0);
                SetupCloud(go);
            }
        }
    }
}
