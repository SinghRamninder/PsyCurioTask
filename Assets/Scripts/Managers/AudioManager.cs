using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public IEnumerator PlaySound(AudioClip clip)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();

        yield return new WaitForSeconds(source.clip.length);

        Destroy(source, 1f);
    }
}
