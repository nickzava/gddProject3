﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioClips : MonoBehaviour
{
	public AudioSource[] sounds;

    // Start is called before the first frame update
    void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void PlayAudio(int clipNum)
	{
		AudioSource.PlayClipAtPoint(sounds[clipNum].clip, this.transform.position);
	}
}
