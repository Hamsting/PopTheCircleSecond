using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AudioCenter : MonoBehaviour
{
	private static AudioCenter Instance;
	private AudioSource audioSource;

#if UNITY_ANDROID && !UNITY_EDITOR
	public static AndroidJavaClass unityActivityClass;
	public static AndroidJavaObject activityObj;
	private static AndroidJavaObject soundObj;
	


	public static void playSound( int soundId )	{
		soundObj.Call( "playSound", new object[] { soundId } );
	}
	
	public static void playSound( int soundId, float volume ) {
		soundObj.Call( "playSound", new object[] { soundId, volume } );
	}
	
	public static void playSound( int soundId, float leftVolume, float rightVolume, int priority, int loop, float rate  ) {
		soundObj.Call( "playSound", new object[] { soundId, leftVolume, rightVolume, priority, loop, rate } );
	}
	
	public static int loadSound( string soundName ) {
		return soundObj.Call<int>( "loadSound", new object[] { "Resources/Sounds/" +  soundName + ".wav" } );
	}
	
	public static void unloadSound( int soundId ) {
		soundObj.Call( "unloadSound", new object[] { soundId } );
	}
#else
	private Dictionary<int, AudioClip> audioDic = new Dictionary<int, AudioClip>();
	
	public static void playSound( int soundId ) {
		//AudioCenter.Instance.audioSource.clip = AudioCenter.Instance.audioDic[soundId];
		AudioCenter.Instance.audioSource.PlayOneShot(AudioCenter.Instance.audioDic[soundId]);
	}

	public static void playSound( int soundId, float volume ) {
		AudioCenter.Instance.audioSource.PlayOneShot(AudioCenter.Instance.audioDic[soundId], volume);
	}

	public static void playSound( int soundId, float leftVolume, float rightVolume, int priority, int loop, float rate ) {
		//float panRatio = AudioCenter.Instance.audioSource.panStereo;
		//rightVolume = Mathf.Clamp(rightVolume, 0, 1);
		//leftVolume = Mathf.Clamp(leftVolume, 0, 1);
		//AudioCenter.Instance.audioSource.panStereo = Mathf.Clamp(rightVolume, 0, 1) - Mathf.Clamp(leftVolume, 0, 1);
		float volume = (leftVolume + rightVolume) / 2;
		AudioCenter.Instance.audioSource.PlayOneShot(AudioCenter.Instance.audioDic[soundId], volume);
		//AudioCenter.Instance.audioSource.panStereo = panRatio;
	}
	
	public static int loadSound( string soundName ) {
		var soundID = soundName.GetHashCode();
		var audioClip = Resources.Load<AudioClip>("Sounds/" + soundName);
		AudioCenter.Instance.audioDic[soundID] = audioClip;
		
		return soundID;
	}
	
	public static void unloadSound( int soundId ) {
		var audioClip = AudioCenter.Instance.audioDic[soundId];
		Resources.UnloadAsset(audioClip);
		AudioCenter.Instance.audioDic.Remove(soundId);
	}
#endif

	private void Awake() {
		if (Instance == null || Instance == this) {
			Instance = this;
		} else {
			Destroy(this);
			return;
		}
		
		#if !UNITY_ANDROID || UNITY_EDITOR
			audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.hideFlags = HideFlags.HideInInspector;
		#else
			unityActivityClass =  new AndroidJavaClass( "com.unity3d.player.UnityPlayer" );
			activityObj = unityActivityClass.GetStatic<AndroidJavaObject>( "currentActivity" );
			//soundObj = new AndroidJavaObject( "com.catsknead.androidsoundfix.AudioCenter", 1, activityObj, activityObj );
			soundObj = new AndroidJavaObject( "com.catsknead.androidsoundfix.AudioCenter", 5, activityObj );
		#endif
	}
}
