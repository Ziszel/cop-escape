using UnityEngine;

public class CopController : MonoBehaviour
{
    public bool localIsPlayerInView;
    
    [SerializeField] private float closeDistanceRange; // The range at which the 
    [SerializeField] private float rayRange;
    [SerializeField] private float fieldOfViewAngle;

    private GameObject _playerObj;
    private PlayerController _player;
    private CopAI _copAi;

    private void Start()
    {
        // https://docs.unity3d.com/ScriptReference/GameObject.GetComponent.html
        // find the game object since the PlayerController script is on another game object
        _playerObj = GameObject.Find("Player"); // only one player so this is safe
        _player = _playerObj.GetComponent<PlayerController>();
        _copAi = GetComponent<CopAI>(); // will get the CopAI component attached to this game object
    }

    private void Update()
    {
        // Level manager will check this value for each cop and if any are set to true the timer will increase
        localIsPlayerInView = ScanForPlayer();

    }

    // https://answers.unity.com/questions/15735/field-of-view-using-raycasting.html
    // https://forum.unity.com/threads/raycasting-a-cone-instead-of-single-ray.39426/
    private bool ScanForPlayer()
    {
        Vector3 rayDirection = _player.transform.position - transform.position;
 
        // If the player is close and directly in-front of the cop, return True
        // Vector3.Angle calculates the angle between 'from' and 'to'.
        // If the angle is less than 90, the player is in direct view of the cop
        if((Vector3.Angle(rayDirection, transform.forward)) < fieldOfViewAngle && 
           (Vector3.Distance(transform.position, _player.transform.position) <= closeDistanceRange)){
            //Debug.Log("Player in direct view!");
            return true;
        }

        // Detect if player is within the field of view
        if((Vector3.Angle(rayDirection, transform.forward)) < fieldOfViewAngle / 2)
        { 
            //Debug.Log("within field of view.");
            RaycastHit hit;
                
            // Send out a raycast to detect if the player is in direct line of sight
            // https://docs.unity3d.com/ScriptReference/Physics.Raycast.html
            if (Physics.Raycast (transform.position, rayDirection, out hit, rayRange)) {
 
                // use the Player tag to determine if the player has been seen
                if (hit.collider.gameObject.transform.CompareTag("Player") && !_player.GetHiddenValue()) {
                    _copAi.SetState(CopState.Chasing);
                    return true;
                }
                // transform
                if (localIsPlayerInView)
                {
                    _copAi.SetState(CopState.Patrolling, _player.transform, transform);   
                }
                return false;
            }
        }
        return false;
    }
}
