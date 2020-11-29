// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public readonly struct DEFAULTS
{
  public readonly static float volume = 0.7f;
  public readonly static float pitch = 1f;
  public readonly static float randomVolumeMod = 0.1f;
  public readonly static float randomPitchMod = 0.1f;
  public readonly static float repeatWaitTime = 0f;
}

[System.Serializable]
public class Sound
{
  [Header("Properties")]
  // `name` serves as the unique identifier in this tutorial
  public string name;
  public AudioClip clip;

  // the range attribute only allows setting within a min/max in the Editor
  [Range(0f, 1f)]
  public float volume = DEFAULTS.volume;

  // I HAVE NO FREAKING IDEA WHY THE SOUND ARRAY IS NOT INITIALIZING
  // WITH THE VALUES PROVIDED BELOW.
  [Range(0.5f, 1.5f)]
  public float pitch = DEFAULTS.pitch;

  [Tooltip("Volume randomness modifier")]
  [Range(0f, 0.5f)]
  public float randomVolumeMod = DEFAULTS.randomVolumeMod;

  [Tooltip("Pitch randomness modifier")]
  [Range(0f, 0.5f)]
  public float randomPitchMod = DEFAULTS.randomPitchMod;


  [Tooltip("Time to wait between the same sound playing again")]
  [Range(0f, 5f)]
  public float repeatWaitTime = DEFAULTS.repeatWaitTime;

  private AudioSource source;
  private bool isInitialized = false;
  private float timeLastPlayed = 0f;
  public float TimeLastPlayed { get { return timeLastPlayed; } }

  public void Init()
  {
    if (isInitialized) return;
    volume = DEFAULTS.volume;
    pitch = DEFAULTS.pitch;
    randomVolumeMod = DEFAULTS.randomVolumeMod;
    randomPitchMod = DEFAULTS.randomPitchMod;
    repeatWaitTime = DEFAULTS.repeatWaitTime;
    isInitialized = true;
  }

  public void SetSource(AudioSource _source)
  {
    source = _source;
    source.clip = clip;
  }

  public void Play(float currentTime)
  {
    if (currentTime < timeLastPlayed + repeatWaitTime) return;
    timeLastPlayed = currentTime;
    source.volume = volume * (1 + Random.Range(-randomVolumeMod / 2f, randomVolumeMod / 2f));
    source.pitch = pitch * (1 + Random.Range(-randomPitchMod / 2f, randomPitchMod / 2f)); ;
    source.Play();
  }
}

public class AudioManager : MonoBehaviour
{
  // making a field Serializable means that this field is editable by the Editor, but not
  // necessarily by other scripts (it needs to have public scope for that)
  [SerializeField]
  public Sound[] sounds = new Sound[0];

  // in a real game, you would only want to expose this via a getter so that other
  // classes can't change this
  public static AudioManager instance;

  void Awake()
  {
    if (instance != null)
    {
      Debug.LogError("More than one AudioManager in the scene.");
    }
    else
    {
      instance = this;
    }
  }

  // Start is called before the first frame update
  void Start()
  {
    for (int i = 0; i < sounds.Length; i++)
    {
      GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
      // nest the new game object within the AudioManager game object.
      _go.transform.SetParent(this.transform);
      // set the source and add the AudioSource component in a single pass
      // to avoid unnecessary garbage collection
      sounds[i].SetSource(
        _go.AddComponent<AudioSource>()
      );
    }
  }

  public void PlaySound(string _name)
  {
    for (int i = 0; i < sounds.Length; i++)
    {
      if (sounds[i].name == _name)
      {
        sounds[i].Play(Time.time);
        return;
      }
    }

    // no sound with _name
    Debug.LogWarning("AudioManager: \"" + _name + "\" not found in sounds list!");
  }
}
