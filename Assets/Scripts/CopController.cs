using UnityEngine;

// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/internal
// internal means this code is only accessible within the same assembly, in this case the CopController class
internal enum CopState
{
    Patrolling = 0, // Standard patrol cycle (could be influenced by recent reports from other cops)
    Investigating = 1, // Has seen something of interest and moving towards it before radioing for backup
    Assisting = 2 // Moving to a new patrol location closest to a report from another cop
}

public class CopController : MonoBehaviour
{
    [SerializeField] private float closeDistanceRange; // The range at which the 
    [SerializeField] private float rayRange;
    [SerializeField] private float fieldOfViewAngle;
    [SerializeField] private float movementSpeed;
    
    private CopState _currentState;
    private GameObject _playerObj;
    private PlayerController _player;

    private void Start()
    {
        // https://docs.unity3d.com/ScriptReference/GameObject.GetComponent.html
        // find the game object since the PlayerController script is on another game object
        _playerObj = GameObject.Find("Player"); // only one player so this is safe
        _currentState = CopState.Patrolling; // cops ALWAYS start in patrol mode
        _player = _playerObj.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (_currentState == CopState.Patrolling)
        {
            ScanForPlayer();
        }
    }

    // https://answers.unity.com/questions/15735/field-of-view-using-raycasting.html
    // https://forum.unity.com/threads/raycasting-a-cone-instead-of-single-ray.39426/
    private bool ScanForPlayer()
    {
        var rayDirection = _player.transform.position - transform.position;
 
        // If the player is close and directly in-front of the cop, return True
        // Vector3.Angle calculates the angle between 'from' and 'to'.
        // If the angle is less than 90, the player is in direct view of the cop
        if((Vector3.Angle(rayDirection, transform.forward)) < fieldOfViewAngle && 
           (Vector3.Distance(transform.position, _player.transform.position) <= closeDistanceRange)){
            Debug.Log("Player in direct view!");
            return true;
        }

        // Detect if player is within the field of view
        if((Vector3.Angle(rayDirection, transform.forward)) < fieldOfViewAngle / 2)
        { 
            Debug.Log("within field of view.");
            RaycastHit hit;
                
            // Send out a raycast to detect if the player is in direct line of sight
            // https://docs.unity3d.com/ScriptReference/Physics.Raycast.html
            if (Physics.Raycast (transform.position, rayDirection, out hit, rayRange)) {
 
                // use the Player tag to determine if the player has been seen
                if (hit.collider.gameObject.transform.CompareTag("Player")) {
                    Debug.Log("Can see player.");
                    return true;
                }
                Debug.Log("Can not see player.");
                return false;
            }
        }
        return false;
    }
    
    
}
