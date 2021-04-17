using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay : MonoBehaviour {

    public bool hall = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlaySound(int audioId)
    {
        if (hall)
            CustomAudio.GetInstance().PlayCustomAudio((uint)audioId);
        else
            CustomAudioDataManager.GetInstance().PlayAudio(audioId);
    }
}
