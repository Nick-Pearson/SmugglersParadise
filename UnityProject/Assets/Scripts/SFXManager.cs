using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour {
    public static SFXManager instance;

    public enum Sound
    {
        Big_Hit,
        Medium_Hit,
        Small_Hit,
        Gun
    }

    public AudioClip[] Sounds;
    public AudioMixerGroup SFXAudioGroup;

    public static void PlaySound(Sound s)
    {
        if(instance == null)
        {
            Debug.LogError("No SFX Manager exists so sound cannot be played");
            return;
        }

        instance.StartCoroutine(instance.PlayOneshot(s));
    } 

    public IEnumerator PlayOneshot(Sound s)
    {
        //create a new game object to play the sound
        GameObject go = new GameObject(s.ToString() + " (oneshot)");
        AudioSource source = go.AddComponent<AudioSource>();

        source.clip = Sounds[(int)s];
        source.outputAudioMixerGroup = SFXAudioGroup;
        source.Play();

        yield return new WaitForSeconds(source.clip.length);

        Destroy(go);
    }

    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
    }
}
