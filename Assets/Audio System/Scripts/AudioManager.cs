using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSFXObject;

    [Tooltip("Looping/music sources. Any assigned here start playing on Start. Sources created by PlayLooping are added automatically.")]
    [SerializeField] private List<AudioSource> loopingSources = new List<AudioSource>();

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (Instance != this) return;

        // Play any sources that were already assigned in the inspector
        foreach (AudioSource source in loopingSources)
        {
            if (source != null && !source.isPlaying)
            {
                source.Play();
            }
        }
    }

    /// <summary>
    /// Play looping ambience or music. The source is tracked in the looping list and returned
    /// so you can stop it later with StopLooping.
    /// </summary>
    public AudioSource PlayLooping(AudioClip audioClip, float volume = 1f, Transform spawnTransform = null)
    {
        if (audioClip == null) return null;

        AudioSource source = CreateSource(audioClip, volume, spawnTransform);
        source.loop = true;
        source.Play();

        loopingSources.RemoveAll(s => s == null); // clean up any destroyed sources
        loopingSources.Add(source);

        return source;
    }

    /// <summary>
    /// Stop a tracked looping source and remove it from the list.
    /// </summary>
    public void StopLooping(AudioSource source)
    {
        if (source == null) return;

        loopingSources.Remove(source);
        source.Stop();

        // Only destroy sources this manager spawned, not ones you placed in the scene yourself
        if (source.transform.parent == transform && source != audioSFXObject)
        {
            Destroy(source.gameObject);
        }
    }

    /// <summary>
    /// Stop every tracked looping source.
    /// </summary>
    public void StopAllLooping()
    {
        // Iterate over a copy since StopLooping modifies the list
        foreach (AudioSource source in new List<AudioSource>(loopingSources))
        {
            StopLooping(source);
        }
    }

    /// <summary>
    /// Play a one-shot audio clip. If spawnTransform is set, the sound is 3D at that position;
    /// otherwise it plays as a regular audio clip.
    /// </summary>
    /// <param name="audioClip">Clip to play</param>
    /// <param name="volume">0 to 1</param>
    /// <param name="spawnTransform">Where the sound comes from (null = 2D)</param>
    /// <param name="pitchRandomization">+/- range around pitch 1, e.g. 0.1 gives 0.9 to 1.1</param>
    public void PlayOneShot(AudioClip audioClip, float volume = 1f, Transform spawnTransform = null, float pitchRandomization = 0f)
    {
        if (audioClip == null) return;

        AudioSource source = CreateSource(audioClip, volume, spawnTransform);

        if (pitchRandomization > 0f)
        {
            source.pitch = Random.Range(1f - pitchRandomization, 1f + pitchRandomization);
        }

        source.Play();

        // Clip plays slower/faster depending on pitch, so account for it
        float lifetime = audioClip.length / Mathf.Max(0.01f, Mathf.Abs(source.pitch));
        Destroy(source.gameObject, lifetime);
    }

    private AudioSource CreateSource(AudioClip clip, float volume, Transform spawnTransform)
    {
        Vector3 position = spawnTransform != null ? spawnTransform.position : transform.position;

        AudioSource source = Instantiate(audioSFXObject, position, Quaternion.identity, transform);
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = spawnTransform != null ? 1f : 0f; // 1 = spatial audio, 0 = not spatial

        return source;
    }
}