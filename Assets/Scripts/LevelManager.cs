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

    private GameState _gmState;
    private GameObject[] _cops;

    private void Start()
    {
        _gmState = GameState.MainLoop;
        _cops = GameObject.FindGameObjectsWithTag("Cop");
    }

    private void Update()
    {
        if (_gmState == GameState.MainLoop)
        {
            // do something if required else remove this if statement later
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

    // This function will be called when the player has been seen
    // Potentially split this out further into game 'states', and call different functions depending on the 
    public static void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void EndGame()
    {
        // Update any global game vars here (best time, best score, etc...)
        Debug.Log("Game over!");
        ShowWinUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EndGame();
        }
    }

    private void ShowWinUI()
    {
        // implement later
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

    public Transform[] GetClosestWayPoints(int depth, Transform startPos)
    {
        // From a point, return all the nearest wayPoints by Vector3 Distance up to a certain depth
        // Assign them to a new Transform[] array and return it
        
        return null;
    }
}
