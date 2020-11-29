using UnityEngine;

public class GameManager : MonoBehaviour
{
  private AudioManager audioManager;
  void Awake()
  {
    audioManager = AudioManager.instance;
    if (audioManager == null) Debug.LogError("NO AudioManager instance found in this scene for GameManager!");
  }

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
}
