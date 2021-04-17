﻿using UnityEngine;
using XLua;

/// <summary>

    /// <summary>

    /// <summary>
        {
        }
    {
        if (!SoundSource1.isPlaying)
            return SoundSource1;
        return SoundSource2;
    }
        AudioSource tempSource = GetIdleSoundAudioSource();
        {
            Debug.LogWarning("当前没有空闲的音效组件,ab:" + abname + "soundname:" + soundname);
            return;
        }

        tempSource.clip = bundle.LoadAsset(soundname) as AudioClip;
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
            BGMusicSource.volume = value;
    }

    /// <summary>
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
                SoundSource1.volume = value;
                SoundSource2.volume = value;
    }

        if (SoundSource1 != null)

        if (SoundSource2 != null)
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

    /// <summary>

    /// <summary>
            SoundSource1.Stop();
        if (SoundSource2 != null)
            SoundSource2.Stop();