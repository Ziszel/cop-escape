using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

enum GameState
{
    MainLoop,
    PlayerSeen,
    PlayerWon
}

public class LevelManager : MonoBehaviour
{
    public Slider playerVisibleSlider;
    public TMP_Text hiddenText;

    [Header("Difficulty")]
    [SerializeField] private float _timerValue = 2.0f;
    
    private GameState _gmState;
    private GameObject[] _cops;
    private GameObject[] _waypoints;
    private PlayerController _player;
    private float _sliderValue;
    private List<CopController> _copControllerList;
    private float _seenMeterTimer;

    private void Start()
    {
        _gmState = GameState.MainLoop;
        _cops = GameObject.FindGameObjectsWithTag("Cop");
        _copControllerList = GetCopControllerFromGameobject();
        _waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        _sliderValue = playerVisibleSlider.value;
        ResetTimer();
    }

    private void Update()
    {
        if (_gmState == GameState.MainLoop)
        {
            ShouldShowHiddenText();

            if (_seenMeterTimer <= 0.0f)
            {
                // https://www.youtube.com/watch?v=tWUyEfD0kV0
                UpdateVisibilitySlider();
                ResetTimer();
            }
            UpdateTimer();

            if (_sliderValue == 4)
            {
                _gmState = GameState.PlayerSeen;
                ResetLevel();
            }
        }

        else if (_gmState == GameState.PlayerSeen)
        {
            // stop game loop
            // show end game UI
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ResetLevel();
            }
        }
        else if (_gmState == GameState.PlayerWon)
        {
            EndGame();
        }
    }
    
    // TIMER RELATED
    private bool IsPlayerInViewOfCop()
    {
        foreach (var cop in _copControllerList)
        {
            if (cop.localIsPlayerInView)
            {
                return true;
            }
        }
        
        return false;
    }

    private void UpdateTimer()
    {
        _seenMeterTimer -= Time.deltaTime;
    }
    
    private void ResetTimer()
    {
        _seenMeterTimer = _timerValue;
    }

    // UI RELATED
    private void UpdateVisibilitySlider()
    {
        if (IsPlayerInViewOfCop())
        {
            _sliderValue++;
        }
        else if (!IsPlayerInViewOfCop())
        {
            _sliderValue--;
        }

        // Make sure the slider is capped at the bottom end of the scale. No check for higher as a value here as
        // the max slider value will change the game state in the Update loop
        if (_sliderValue < 0)
        {
            _sliderValue = 0; 
        }

        // No loss of position as I'm using whole numbers stored as a float due to the way Slider works in Unity.
        // Because updating the Slider value is expensive, this check is designed to stop updating it if the value
        // is the same as it was the last time this function was called.
        if (playerVisibleSlider.value != _sliderValue)
        {
            playerVisibleSlider.value = _sliderValue;   
        }
    }

    private void ShouldShowHiddenText()
    {
        if (_player.GetHiddenValue())
        {
            hiddenText.gameObject.SetActive(true);
        }
        else
        {
            hiddenText.gameObject.SetActive(false);
        }
    }
    
    private static void ShowWinUI()
    {
        // implement later
        SceneManager.LoadScene("MainMenu");
    }

    // GAME STATE RELATED
    public static void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public static void EndGame()
    {
        // Update any global game vars here (best time, best score, etc...)
        ShowWinUI();
    }

    // UTILITY METHODS
    
    // if I needed the functionality,
    // I could make this return a generic type to get any type of component from a game object
    private List<CopController> GetCopControllerFromGameobject()
    {
        List<CopController> copControllerList = new List<CopController>();
        foreach (var cop in _cops)
        {
            var copController = cop.GetComponent<CopController>();
            copControllerList.Add(copController);
        }
        return copControllerList;
    }
    
    public CopAI GetNearestCop(Transform localTransform)
    {
        // Find the nearest cop, assign this to _nearestCop
        // https://docs.unity3d.com/ScriptReference/GameObject.FindGameObjectsWithTag.html
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 pos = localTransform.position;
        foreach (GameObject cop in _cops)
        {
            Vector3 difference = cop.transform.position - pos;
            float curDistance = difference.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = cop;
                distance = curDistance;
            }
        }
        return closest.GetComponent<CopAI>();
    }

    public Transform[] GetClosestWayPoints(Transform startPos, int depth)
    {
        // From a point, return all the nearest wayPoints by Vector3 Distance up to a certain depth
        List<Transform> returnWaypoints = new List<Transform>();
        
        // Use a sorted dictionary to sort by the distance from the point (float), but store the transform along with
        // the value for retrieval after the foreach loop
        // https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.sorteddictionary-2.enumerator?view=net-8.0
        SortedDictionary<float, Transform> distances = new SortedDictionary<float, Transform>();
        
        Vector3 pos = startPos.position;
        foreach (GameObject waypoint in _waypoints)
        {
            Vector3 difference = waypoint.transform.position - pos;
            float curDistance = difference.sqrMagnitude;
            distances.Add(curDistance, waypoint.transform);
        }

        // Until max depth is reached, add the waypoint to the returnWaypoints list
        for (int i = 0; i < depth; ++i)
        {
            // https://code-maze.com/csharp-get-item-by-index-from-dictionary/
            returnWaypoints.Add(distances.ElementAt(i).Value);
        }
        
        return returnWaypoints.ToArray(); // return the list as an array 
    }
}
