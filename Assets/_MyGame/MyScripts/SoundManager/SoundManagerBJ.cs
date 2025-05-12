using UnityEngine;

public class SoundManagerBJ : MonoBehaviour
{
    [SerializeField]
    SoundMgrScriptable AllSoundsAsset;
    Transform AudioSourcePoolParent;

    public static SoundMgrScriptable AllSounds;

    #region Creating Instance
    private static SoundManagerBJ _instance;
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
    public AudioSource ASource;
    public void PlayAudioClip(AudioClip audioClip, bool isLoop)
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
    }
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
        ASource.playOnAwake = true;
        ASource.PlayOneShot(audioClip);
        ASource.name = "AS: " + audioClip.name;
    }
    int soundNumber;
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

    void getAudioSource(Vector3 position)
    {
        ASource = null;

        if (!AudioSourcePoolParent)
        {
            GameObject poolObj = new GameObject("AudioSourcesPoolParent");
            AudioSourcePoolParent = poolObj.transform;
            DontDestroyOnLoad(poolObj); // <-- This line ensures persistence between scenes
        }
        
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

        if (!ASource)
        {
            GameObject newAudioSource = Instantiate(AllSounds.AudioSourcePrefab, position, Quaternion.identity);
            newAudioSource.transform.parent = AudioSourcePoolParent;
            ASource = newAudioSource.GetComponent<AudioSource>();
        }
    }

}