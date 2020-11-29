using System.Data;
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
  private float speed = 10;
  public int level = 3;
  public int health = 40;
  public bool flightEnabled = false;

  private Player instance;

  public void Update()
  {
    Vector3 input = new Vector3(
      // Input.GetAxis vs. Input.GetAxisRaw
      // Input.GetAxis smoothes the input over time, whereas
      // Input.GetAxisRaw applies no smoothing. The latter is
      // prefferable if you want to add your own easing.
      Input.GetAxisRaw("Horizontal"),
      Input.GetAxisRaw("Vertical"),
      0
    );

    Vector3 direction = input.normalized;
    Vector3 velocity = direction * speed;
    Vector3 moveAmount = velocity * Time.deltaTime;

    transform.position += moveAmount;
  }

  public void SavePlayer()
  {
    SaveSystem.Save(this);
    Debug.Log("--------------");
    Debug.Log("SAVED SETTINGS");
    Debug.Log("--------------");
    Debug.Log("Level: " + level);
    Debug.Log("Health: " + health);
    Debug.Log("Flight Enabled: " + flightEnabled);
  }

  public void LoadPlayer()
  {
    PlayerData data = SaveSystem.LoadPlayer();
    level = data.level;
    health = data.health;
    flightEnabled = data.flightEnabled;
    Vector3 position;
    position.x = data.position[0];
    position.y = data.position[1];
    position.z = data.position[2];
    transform.position = position;

    Debug.Log("-----------------");
    Debug.Log("RESTORED SETTINGS");
    Debug.Log("-----------------");
    Debug.Log("Level: " + level);
    Debug.Log("Health: " + health);
    Debug.Log("Flight Enabled: " + flightEnabled);
  }

  private int stringToInt(string input)
  {
    try
    {
      int result = Int32.Parse(input);
      return result;
    }
    catch (FormatException)
    {
      Debug.LogError($"Unable to parse '{input}'");
      return 0;
    }
  }

  // private bool stringToBool(string input)
  // {
  //   try
  //   {
  //     Debug.Log(input);
  //     return input == "true";
  //   }
  //   catch (FormatException)
  //   {
  //     Debug.LogError($"Unable to parse '{input}'");
  //     return false;
  //   }
  // }

  public void setLevel(string _level)
  {
    level = stringToInt(_level);
  }

  public void setHealth(string _health)
  {
    health = stringToInt(_health);
  }

  public void setFlightEnabled(bool _flightEnabled)
  {
    flightEnabled = _flightEnabled;
  }
}
