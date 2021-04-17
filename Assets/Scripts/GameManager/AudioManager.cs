using UnityEngine;
using XLua;

/// <summary>/// 音乐音效播放管理/// </summary>[LuaCallCSharp]public class AudioManager{    public static readonly AudioManager Instance = new AudioManager();    /// <summary>    /// 背景音乐播放组件    /// </summary>    AudioSource BGMusicSource;    /// <summary>    /// 音效播放组件1    /// </summary>    AudioSource SoundSource1;

    /// <summary>    /// 音效播放组件2    /// </summary>    AudioSource SoundSource2;

    /// <summary>    /// 添加组件的对象    /// </summary>    GameObject AudioMgrObj;    private AudioManager()    {        BGMusicSource = null;        SoundSource1 = null;        AudioMgrObj = null;    }       public void InitAudioMgr(GameObject obj)    {        AudioMgrObj = obj;        if (BGMusicSource == null)        {            BGMusicSource = AudioMgrObj.AddComponent<AudioSource>();        }        if(SoundSource1 == null)        {            SoundSource1 = AudioMgrObj.AddComponent<AudioSource>();        }        if (SoundSource2 == null)        {            SoundSource2 = AudioMgrObj.AddComponent<AudioSource>();        }    }    public void testPlaySound()    {        string soundname = "hall";        SoundSource1.clip = Resources.Load(soundname) as AudioClip;        if (SoundSource1.clip == null)        {            Debug.LogWarning("testPlaySound loadsound failed music:" + soundname);        }        else        {            SoundSource1.volume = 1f;            SoundSource1.loop = true;            SoundSource1.Play();        }    }    /// <summary>    ///播放背景音乐    /// </summary>    /// <param name="musicname"></param>    public void PlayBGMusic(string abname,string musicname,bool isloop = true, bool forceStopPlaying = true)    {        if(BGMusicSource == null)        {            Debug.LogWarning("背景音乐播放组件没有初始化" );            return;        }        AssetBundle bundle = AssetBundleManager.GetAssetBundle(abname);        if(bundle == null)        {            Debug.LogWarning("音乐所在的ab不存在 abname:" + abname +"musicname:" + musicname);            return;        }        AudioClip clip = bundle.LoadAsset(musicname) as AudioClip;        if (BGMusicSource.isPlaying)
        {            if (!forceStopPlaying && clip == BGMusicSource.clip)                return;            BGMusicSource.Stop();
        }        BGMusicSource.clip = clip;        if(BGMusicSource.clip ==null)        {            Debug.LogWarning("PlayBGMusic loadmusic failed music:" + musicname);        }        else        {            BGMusicSource.loop = isloop;            BGMusicSource.Play();        }    }     private AudioSource GetIdleSoundAudioSource()
    {
        if (!SoundSource1.isPlaying)
            return SoundSource1;
        return SoundSource2;
    }    /// <summary>    /// 播放音效    /// </summary>    /// <param name="soundname"></param>    public void PlaySound(string abname ,string soundname)    {
        AudioSource tempSource = GetIdleSoundAudioSource();        if (tempSource == null)
        {
            Debug.LogWarning("当前没有空闲的音效组件,ab:" + abname + "soundname:" + soundname);
            return;
        }        tempSource.Stop();        AssetBundle bundle = AssetBundleManager.GetAssetBundle(abname);        if (bundle == null)        {            Debug.LogWarning("音效所在的ab不存在 abname:" + abname + "soundname:" + soundname);            return;        }

        tempSource.clip = bundle.LoadAsset(soundname) as AudioClip;        if (tempSource.clip == null)        {            Debug.LogWarning("PlaySound loadsound failed music:" + soundname);        }        else        {            tempSource.loop = false;            tempSource.Play();        }    }    /// <summary>    /// 设置背景音乐音量    /// </summary>    /// <param name="volume"></param>    public float MusicVolume
    {
        get
        {
            if (BGMusicSource == null)
                return 0.0f;
            return BGMusicSource.volume;
        }
        set
        {
            if (BGMusicSource == null)
            {
                Debug.LogWarning("音效组件没有初始化");
                return;
            }
            BGMusicSource.volume = value;        }
    }

    /// <summary>    /// 设置音效音量    /// </summary>    /// <param name="volume"></param>    public float SoundVolume
    {
        get
        {
            if (SoundSource1 == null)
                return 0.0f;
            return SoundSource1.volume;
        }
        set
        {
            if (SoundSource1 == null && SoundSource2 == null)
            {
                Debug.LogWarning("音效组件没有初始化");
                return;
            }
            if (SoundSource1 != null)
                SoundSource1.volume = value;            if (SoundSource2 != null)
                SoundSource2.volume = value;        }
    }    /// <summary>    /// 静音    /// </summary>    public void Quiet()    {        if (BGMusicSource != null)        {            BGMusicSource.volume = 0;        }

        if (SoundSource1 != null)        {            SoundSource1.volume = 0;        }

        if (SoundSource2 != null)        {            SoundSource2.volume = 0;        }
    }

    /// <summary>
    /// 判断是否有空闲的音效播放组件
    /// </summary>
    /// <returns>true(有空闲的组件)false(没有)</returns>
    public bool GetIdleSoundAudioSourceState()
    {
        if (!SoundSource1.isPlaying || !SoundSource2.isPlaying)
            return true;
        return false;
    }

    /// <summary>    /// 暂停背景音乐    /// </summary>    public void PauseMusic()    {        if (BGMusicSource == null)        {            Debug.LogWarning("音效组件没有初始化");            return;        }        BGMusicSource.Pause();    }    /// <summary>    /// 停止背景音乐    /// </summary>    public void StopMusic()    {        if (BGMusicSource == null)        {            Debug.LogWarning("音效组件没有初始化");            return;        }        BGMusicSource.Stop();    }

    /// <summary>    /// 停止音效    /// </summary>    public void StopSound()    {        if (SoundSource1 != null)        {
            SoundSource1.Stop();        }
        if (SoundSource2 != null)        {
            SoundSource2.Stop();        }    }}