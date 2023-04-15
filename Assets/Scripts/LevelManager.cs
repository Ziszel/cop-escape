using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

enum GameState
{
    MainLoop,
    PlayerSeen,
    PlayerWon
}

public class LevelManager : MonoBehaviour
{
    public TMP_Text hiddenText;
    
    private GameState _gmState;
    private GameObject[] _cops;
    private GameObject[] _waypoints;
    private PlayerController _player;

    private void Start()
    {
        _gmState = GameState.MainLoop;
        _cops = GameObject.FindGameObjectsWithTag("Cop");
        _waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (_gmState == GameState.MainLoop)
        {
            ShouldShowHiddenText();
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

    // This function will be called when the player has been seen
    // Potentially split this out further into game 'states', and call different functions depending on the 
    public static void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public static void EndGame()
    {
        // Update any global game vars here (best time, best score, etc...)
        ShowWinUI();
    }

    private static void ShowWinUI()
    {
        // implement later
        SceneManager.LoadScene("MainMenu");
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
