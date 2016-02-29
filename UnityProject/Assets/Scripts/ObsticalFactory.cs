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
	private float mColumnWidth;
	
	void Awake()
	{
		if( mInstance == null )
		{
			mInstance = this;

            // Work out the width of each column
			mColumnWidth = ( GameLogic.ScreenHeight * Camera.main.aspect * 0.8f ) / (int)GameState.Column.NumColumns;

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
		// Look for a free enemy and then dispatch them 
		GameObject result = null;
		if( mInactive.Count > 0 )
		{
			GameObject enemy = mInactive[0];
			Vector3 position = enemy.transform.position;
			position.x = -mColumnWidth + ( mColumnWidth * (float)column ); 
			position.y = GameLogic.ScreenHeight * 0.5f;
			position.z = 0.0f;
			enemy.transform.position = position;
			enemy.SetActive( true );
			mActive.Add( enemy );
			mInactive.Remove( enemy );
			result = enemy;
		}

        if(result == null)
        {
            //we need to instantiate our obstical

        }
		
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
