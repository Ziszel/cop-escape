using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Use of state to dictate how the game should react
public enum GameState
{
    MainLoop,
    PlayerSeen,
    PlayerWon
}

public class LevelManager : MonoBehaviour
{
    // UI Elements.
    public Slider playerVisibleSlider;
    public TMP_Text hiddenText;
    public TMP_Text okText;
    public TMP_Text seenText;
    public TMP_Text returnText;
    public TMP_Text deadText;
    public RawImage minimap;
    public Image minimapMask;
    public Image deathScreen;
    public AudioSource mainLoopMusic;
    public AudioSource deathMusic;

    [Header("Difficulty")]
    [SerializeField] private float timerValue = 1.2f;
    
    private static GameState gmState;
    private GameObject[] _cops;
    private GameObject[] _waypoints;
    private PlayerController _player;
    private float _sliderValue;
    private List<CopController> _copControllerList;
    private float _seenMeterTimer;
    private float _showDeathTextTimer;
    private readonly float _fadeRate = 1.2f;

    private void Start()
    {
        gmState = GameState.MainLoop;
        _cops = GameObject.FindGameObjectsWithTag("Cop");
        _copControllerList = GetCopControllerFromGameobject();
        _waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        _sliderValue = playerVisibleSlider.value;
        _showDeathTextTimer = 2.5f;
        ResetSeenTimer();
    }

    private void Update()
    {
        if (gmState == GameState.MainLoop)
        {
            ShouldShowHiddenText();

            if (_seenMeterTimer <= 0.0f)
            {
                // https://www.youtube.com/watch?v=tWUyEfD0kV0
                UpdateVisibilitySlider();
                ResetSeenTimer();
            }
            UpdateSeenTimer();

            if (_sliderValue == 4)
            {
                ShowDeathScreen();
                mainLoopMusic.Stop();
                deathMusic.Play();
                gmState = GameState.PlayerSeen;
            }
        }

        else if (gmState == GameState.PlayerSeen)
        {
            if (_showDeathTextTimer > 0)
            {
                UpdateShowDeathTimer();
            }

            if (_showDeathTextTimer <= 0)
            {
                deadText.gameObject.SetActive(true);
                returnText.gameObject.SetActive(true);
            }
            
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ResetLevel();
            }
        }
        else if (gmState == GameState.PlayerWon)
        {
            EndGame();
        }
    }
    
    // TIMER RELATED
    // each cop has a local variable they can set to check if they can see a player. If this was a global variable here
    // then it would immediately be set to false if even one cop could not see a player. This variable is set via a 
    // raycast inside of CopController, and is used here to manipulate the seen bar (slider)
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

    private void UpdateShowDeathTimer()
    {
        _showDeathTextTimer -= Time.deltaTime;
    }
    
    private void UpdateSeenTimer()
    {
        _seenMeterTimer -= Time.deltaTime;
    }
    
    private void ResetSeenTimer()
    {
        _seenMeterTimer = timerValue;
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

    // Shows or hides the 'HIDDEN' message to the player
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
        // There is no win UI due to time constraints. The game will automatically return to the main menu so that it can
        // be played again in the event that a player completes the game.
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowDeathScreen()
    {
        // Fade the screen black and stop the game.
        DisableMainLoopUI();
        deathScreen.gameObject.SetActive(true);
        StartCoroutine(FadeIn(deathScreen));
    }

    // Set all unneeded UI elements to not active. They will re-activated when the scene is reloaded
    private void DisableMainLoopUI()
    {
        playerVisibleSlider.gameObject.SetActive(false);
        okText.gameObject.SetActive(false);
        seenText.gameObject.SetActive(false);
        minimap.gameObject.SetActive(false);
        minimapMask.gameObject.SetActive(false);
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
        // This function will return a list of all the cops (specifically the controller component), called on Start
        List<CopController> copControllerList = new List<CopController>();
        foreach (var cop in _cops)
        {
            var copController = cop.GetComponent<CopController>();
            copControllerList.Add(copController);
        }
        return copControllerList;
    }
    
    // Called by CopAI and Lever, therefore made globally available inside of the LevelManager to stick to DRY principles
    public CopAI GetNearestCop(Transform localTransform)
    {
        // Find the nearest cop, assign this to _nearestCop
        // https://docs.unity3d.com/ScriptReference/GameObject.FindGameObjectsWithTag.html
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 pos = localTransform.position;
        // Iterate over each cop and set the curDistance to be the smallest value. If difference is lower, make
        // curDistance == to Distance. This ensures I will find the closest cop to the transform passed into the variable
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

    // Get the closest waypoints to a transform passed in. A Maximum depth can be passed into it as well. This is useful
    // for setting different guards to have different size of patrol dynamically as the game is running
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

    // Getter for gmState
    public static GameState GetGameState()
    {
        return gmState;
    }
    
    // https://stackoverflow.com/questions/56516299/how-to-fade-in-ui-image-in-unity
    IEnumerator FadeIn(Image image)
    {
        // the targetAlpha is 1.0f meaning 255 in the alpha channel of the image
        float targetAlpha = 1.0f;
        Color currentColor = image.color; // You cannot set a colour directly, thus a secondly variable is used
        while(currentColor.a < targetAlpha) {
            // lerp the colour towards the targetAlpha by the fadeRate for a smooth transition
            currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, _fadeRate * Time.deltaTime);
            // Update the logo colour by the now updated colour value
            image.color = currentColor;
            yield return null;
        }
    }
}
