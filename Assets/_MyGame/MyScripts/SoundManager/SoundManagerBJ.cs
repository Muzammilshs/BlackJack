using UnityEngine;

/// <summary>
/// Manages all sound playback for the blackjack game, including pooling of AudioSources,
/// singleton access, and utility methods for playing sound effects and music.
/// </summary>
public class SoundManagerBJ : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField]
    SoundMgrScriptable AllSoundsAsset; // ScriptableObject containing all game sounds and AudioSource prefab

    #endregion

    #region Audio Source Pool

    Transform AudioSourcePoolParent; // Parent transform for pooled AudioSources

    #endregion

    #region Static Fields and Singleton

    public static SoundMgrScriptable AllSounds; // Static reference to all game sounds

    private static SoundManagerBJ _instance;
    /// <summary>
    /// Singleton instance for global access.
    /// </summary>
    public static SoundManagerBJ Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<SoundManagerBJ>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    #endregion

    #region Unity Methods

    /// <summary>
    /// Ensures singleton instance and initializes sound assets.
    /// </summary>
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
            AllSounds = AllSoundsAsset;
        }
        else
        {
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }

    #endregion

    #region Audio Playback

    public AudioSource ASource; // The current AudioSource used for playback

    /// <summary>
    /// Plays an AudioClip with specified loop and volume settings.
    /// </summary>
    /// <param name="audioClip">The audio clip to play.</param>
    /// <param name="isLoop">Should the audio loop?</param>
    /// <param name="volume">Playback volume (0-1).</param>
    public void PlayAudioClip(AudioClip audioClip, bool isLoop, float volume)
    {
        Vector3 position = Vector3.zero;
        getAudioSource(position);
        ASource.loop = isLoop;
        ASource.clip = audioClip;
        ASource.volume = volume;
        ASource.playOnAwake = true;
        if (isLoop)
            ASource.Play();
        else
            ASource.PlayOneShot(audioClip);
        ASource.name = "AS: " + audioClip.name;
    }

    /// <summary>
    /// Plays an AudioClip once at default volume.
    /// </summary>
    /// <param name="audioClip">The audio clip to play.</param>
    public void PlayAudioClip(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            Debug.LogError("Audio clip is missing, Assign audio Clip");
            return;
        }
        Vector3 position = Vector3.zero;
        getAudioSource(position);
        ASource.loop = false;
        ASource.clip = audioClip;
        ASource.volume = 1f; // Default volume
        ASource.playOnAwake = true;
        ASource.PlayOneShot(audioClip);
        ASource.name = "AS: " + audioClip.name;
    }

    int soundNumber; // (Unused) Can be used for sound indexing if needed

    /// <summary>
    /// Plays an AudioClip and returns the AudioSource used for playback.
    /// </summary>
    /// <param name="audioClip">The audio clip to play.</param>
    /// <param name="isLoop">Should the audio loop?</param>
    /// <param name="isReturntype">If true, returns the AudioSource used.</param>
    /// <returns>The AudioSource used for playback.</returns>
    public AudioSource PlayAudioClip(AudioClip audioClip, bool isLoop, bool isReturntype)
    {
        Vector3 position = Vector3.zero;
        getAudioSource(position);
        ASource.loop = isLoop;
        ASource.clip = audioClip;
        ASource.playOnAwake = true;
        if (isLoop)
            ASource.Play();
        else
            ASource.PlayOneShot(audioClip);
        ASource.name = "AS: " + audioClip.name;
        return ASource;
    }

    #endregion

    #region AudioSource Pooling

    /// <summary>
    /// Gets an available AudioSource from the pool, or instantiates a new one if needed.
    /// </summary>
    /// <param name="position">The position for the AudioSource.</param>
    void getAudioSource(Vector3 position)
    {
        ASource = null;

        // Create pool parent if it doesn't exist
        if (!AudioSourcePoolParent)
        {
            GameObject poolObj = new GameObject("AudioSourcesPoolParent");
            AudioSourcePoolParent = poolObj.transform;
            DontDestroyOnLoad(poolObj); // Ensures persistence between scenes
        }

        // Try to find an available (not playing) AudioSource in the pool
        for (int i = 0; i < AudioSourcePoolParent.childCount; i++)
        {
            AudioSource source = AudioSourcePoolParent.GetChild(i).GetComponent<AudioSource>();
            if (!source.isPlaying)
            {
                ASource = source;
                ASource.transform.position = position;
                break;
            }
        }

        // If none available, instantiate a new one from the prefab
        if (!ASource)
        {
            GameObject newAudioSource = Instantiate(AllSounds.AudioSourcePrefab, position, Quaternion.identity);
            newAudioSource.transform.parent = AudioSourcePoolParent;
            ASource = newAudioSource.GetComponent<AudioSource>();
        }
    }

    #endregion
}
