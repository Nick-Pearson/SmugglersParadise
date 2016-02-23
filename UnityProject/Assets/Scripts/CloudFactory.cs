using UnityEngine;
using System.Collections;

public class CloudFactory : MonoBehaviour {
    [SerializeField] private Sprite[] Clouds;
    [Range(1, 100)]
    [SerializeField] private int CloudPoolSize;
    [SerializeField] private float CloudCeiling;
    [SerializeField] private float CloudFloor;


    private GameObject[] mCloudObjects;

    void Start()
    {
        mCloudObjects = new GameObject[CloudPoolSize];

        for(int i = 0; i < CloudPoolSize; i++)
        {
            mCloudObjects[i] = new GameObject("Cloud_" + i);
            mCloudObjects[i].transform.parent = transform;
            mCloudObjects[i].transform.position = new Vector3(Random.Range(-GameLogic.ScreenBounds, GameLogic.ScreenBounds), Random.Range(CloudFloor, CloudCeiling), 0);
            SpriteRenderer renderer = mCloudObjects[i].AddComponent<SpriteRenderer>();
            GenerateCloud(mCloudObjects[i]);
        }
    }

    void GenerateCloud(GameObject go)
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
        foreach(GameObject go in mCloudObjects)
        {
            go.transform.Translate(GameLogic.GameDeltaTime * go.transform.localScale.x, 0, 0);

            if(go.transform.position.x > GameLogic.ScreenBounds * 1.5f)
            {
                go.transform.position = new Vector3(GameLogic.ScreenBounds * -1.5f, Random.Range(CloudFloor, CloudCeiling), 0);
                GenerateCloud(go);
            }
        }
    }
}
