using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioMan : MonoBehaviour
{
    private static AudioMan instance;
    private AudioSource audioSource;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip tutorialMusic;
    public AudioClip defaultMusic; // for Level 1
    public AudioClip musicLv2;
    public AudioClip musicLv3;
    public AudioClip musicLv4;
    public AudioClip musicLv5;
    public AudioClip musicLv6;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.3f;
    }

    void Start()
    {
        // Play music for the scene that is already loaded
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip clipToPlay = null;
        string sceneName = scene.name;

        if (sceneName == "MainMenu")
        {
            clipToPlay = mainMenuMusic;
        }
        else if (sceneName == "TutorialLevel")
        {
            clipToPlay = tutorialMusic;
        }
        else if (sceneName == "Level1")
        {
            clipToPlay = defaultMusic;
        }
        else if (sceneName == "Level2")
        {
            clipToPlay = musicLv2;
        }
        else if (sceneName == "Level3" || sceneName == "Level 3")
        {
            clipToPlay = musicLv3;
        }
        else if (sceneName == "Level4")
        {
            clipToPlay = musicLv4;
        }else if (sceneName == "Level5")
        {
            clipToPlay = musicLv5;
        }
        else if (sceneName == "Level6")
        {
            clipToPlay = musicLv6;
        }
        else
        {
            Debug.LogWarning("AudioMan: No music setting for scene: " + sceneName);
            return;
        }

        PlayMusic(clipToPlay);
    }

    void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioMan: No clip assigned for this scene!");
            return;
        }

        if (audioSource.clip == clip && audioSource.isPlaying)
        {
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();
    }
}