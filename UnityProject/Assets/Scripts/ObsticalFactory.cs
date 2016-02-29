using UnityEngine;
using System.Collections.Generic;

public class ObsticalFactory : MonoBehaviour 
{
	private static ObsticalFactory mInstance;

    [SerializeField] private int NumberOfObjects = 1;
    [SerializeField] private int NumberOfTraffic = 1;

    private GameObject [] mPool;
	private List<GameObject> mActive;
	private List<GameObject> mInactive;
	
	void Awake()
	{
		if( mInstance == null )
		{
			mInstance = this;

			// Create the enemies, initialise the active and available lists, put all enemies in the available list
			mActive = new List<GameObject>();
			mInactive = new List<GameObject>();
		}
		else
		{
			Debug.LogError("Only one ObsticalFactory allowed - destorying duplicate");
			Destroy( this.gameObject );
		}
	}

    void Update()
    {
        List<GameObject> deadObsticals = new List<GameObject>();
        //check which obsticals are off screen
        foreach(GameObject go in mActive)
            if(go.transform.position.y < GameLogic.ScreenBottom)
                deadObsticals.Add(go);

        foreach (GameObject go in deadObsticals)
            Return(go);
    }

	public static GameObject Dispatch(GameState.Column column )
	{
		if( mInstance != null )
		{
			return mInstance.DoDispatch( column );
		}
		return null;
	}

	public static bool Return( GameObject enemy )
	{
		if( mInstance != null )
		{
			if( mInstance.mActive.Remove( enemy ) )
			{
				enemy.SetActive( false );
				mInstance.mInactive.Add( enemy ); 
			}
		}
		return false;
	}

	public static void Reset()
	{
		if( mInstance != null )
		{
			foreach(GameObject go in mInstance.mActive)
            {
                mInstance.mInactive.Add(go);
            }

            mInstance.mActive.Clear();
		}
	}

	private GameObject DoDispatch( GameState.Column column )
	{
        //decide what type of obstical we will be
        string oType = DecideObsticalType();

		// Look for a free obstical object and then dispatch them 
		GameObject result = null;
		if( mInactive.Count > 0 )
		{
            foreach(GameObject go in mInactive)
            {
                if (go.name == oType)
                {
                    result = go;
                    mInactive.Remove(go);
                    go.SetActive(true);
                    break;
                }
            }
		}

        if(result == null)
        {
            //we need to instantiate our obstical as none could be found
            GameObject prefab = Resources.Load<GameObject>(oType);
            result = Instantiate(prefab) as GameObject;
            result.name = oType;
            result.transform.SetParent(transform);
        }
        
        //initialise position to top of the screen
        Vector3 position = result.transform.position;
        position.x = -GameLogic.ColumnSize + (GameLogic.ColumnSize * (float)column);
        position.y = GameLogic.ScreenTop;
        position.z = 0.0f;
        result.transform.position = position;
        mActive.Add(result);

        // Returns true if a free enemy was found and dispatched
        return result;
	}

    private string DecideObsticalType()
    {
        if(Random.value > 0.5f)
        {
            return "Objects/r" + Random.Range(0, NumberOfObjects);
        }
        else
        {
            return "Traffic/t" + Random.Range(0, NumberOfTraffic);
        }
    }
}
