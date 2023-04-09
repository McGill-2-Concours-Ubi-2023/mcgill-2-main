using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Music : MonoBehaviour {

	public AudioClip[] clips;
	public AudioSource source;
    int currentsong = -1;

	void Start()
	{
		if (PlayerPrefs.GetInt("mute",1)==0) 
		source.mute=true;
	}
	void Update()
	{
		if (source.isPlaying==false)
		Song();
		


		if (Input.GetKeyDown(KeyCode.M)){
		source.mute = !source.mute;
	    	if (source.mute)
		    PlayerPrefs.SetInt("mute",0);
		    else
		    PlayerPrefs.SetInt("mute",1);
		}
	}
	void Song () {
		int RandomClip = Random.Range (0, clips.Length);
        while (RandomClip == currentsong)
            RandomClip = Random.Range(0, clips.Length);
        source.clip =  clips[RandomClip];
        currentsong = RandomClip;
		source.Play ();
	}
}