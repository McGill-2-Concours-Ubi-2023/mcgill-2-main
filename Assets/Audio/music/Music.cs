using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Music : MonoBehaviour {

	public AudioClip[] clips;
	public AudioSource source;
    int currentsong = -1;
	private bool isLocked;

	void Start()
	{
		if (PlayerPrefs.GetInt("mute",1)==0) 
		source.mute=true;
	}
	void Update()
	{
		if (source.isPlaying==false && !isLocked)
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
		isLocked = true;
		int RandomClip = Random.Range (0, clips.Length);
		if (clips.Length > 1)
		{
			while (RandomClip == currentsong)
				RandomClip = Random.Range(0, clips.Length);
		}
        source.clip =  clips[RandomClip];
        currentsong = RandomClip;
		source.Play ();
		LockSong();
	}

	async void LockSong()
    {
		isLocked = true;
		await Task.Delay(System.TimeSpan.FromSeconds(source.clip.length));
		isLocked = false;
    }
}