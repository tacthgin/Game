using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public const int AUDIO_CLICK = 0;
    public const int AUDIO_ILLEGAL = 1;
    public const int AUDIO_LOSS = 2;
    public const int AUDIO_WIN = 3;
    public const int AUDIO_CAPTURE = 4;
    public const int AUDIO_ENEMY_CAPTURE = 5;
    public const int AUDIO_CHECK = 6;
    public const int AUDIO_ENEMY_CHECK = 7;
    public const int AUDIO_MOVE = 8;
    public const int AUDIO_ENEMY_MOVE = 9;
    public const int AUDIO_DRAW = 10;

    [SerializeField]
    private AudioSource effectSource;

    [SerializeField]
    private AudioClip[] effectClip;

    static private SoundManager instance = null;

    static public SoundManager MyInstance
    {
        set { }
        get { return instance; }
    }

	void Awake ()
    {
        if (instance == null)
        {
            instance = this;
        }else if(instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
	}

    private void Play(AudioClip clip)
    {
        effectSource.clip = clip;
        effectSource.Play();
    }

    public void PlayEffect(int audioId)
    {
        if(audioId > 0 && audioId < effectClip.Length)
        {
            Play(effectClip[audioId]);
        }
    }
}
