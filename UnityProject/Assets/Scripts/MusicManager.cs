using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {
    public static MusicManager insatance;

    public AudioClip[] Music;

    private AudioSource source;
    private int lastClip;
    private int currentLevel = 0;

    public static void ChangeMusic()
    {
        if(insatance == null)
        {
            Debug.LogError("Tried to play music when no music manager exists");
            return;
        }

        insatance.StartCoroutine(insatance.PlayTrack());
    }

	void Awake () {
	    if(insatance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
        insatance = this;
        source = GetComponent<AudioSource>();
        lastClip = -1;

        StartCoroutine(PlayTrack(0, 0));
	}

    public IEnumerator PlayTrack(float fadeTime = 2.0f, int iterations = 20)
    {
        //fade out
        for(int i = 0; i < iterations; i++)
        {
            source.volume = 1 - (i * fadeTime / iterations);
            yield return new WaitForSeconds(fadeTime / iterations);
        }
        source.Stop();

        //select a new clip
        int nextClip;
        do
        {
            nextClip = Random.Range(0, Music.Length);
        } while (Music.Length > 1 && nextClip == lastClip);

        source.clip = Music[nextClip];
        lastClip = nextClip;
        
        source.Play();

        //fade in
        for (int i = 0; i < iterations; i++)
        {
            source.volume = i * fadeTime / iterations;
            yield return new WaitForSeconds(fadeTime / iterations);
        }
    }
}
