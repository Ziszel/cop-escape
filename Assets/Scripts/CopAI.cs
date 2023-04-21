using UnityEngine;
using UnityEngine.AI; // allows access to navmesh agent

// Script created with help from:
// https://docs.unity3d.com/2019.4/Documentation/Manual/nav-BuildingNavMesh.html
// https://docs.unity3d.com/2019.4/Documentation/Manual/nav-CreateNavMeshAgent.html
// https://docs.unity3d.com/Packages/com.unity.ai.navigation@1.1/manual/NavAgentPatrol.html
// https://www.youtube.com/watch?v=c8Nq19gkNfs

// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/internal
// internal means this code is only accessible within the same assembly, in this case the CopController class
public enum CopState
{
    Patrolling = 0, // Standard patrol cycle (could be influenced by recent reports from other cops)
    Investigating = 1, // Has seen something of interest and moving towards it before radioing for backup
    Idle = 2 // Cop is currently doing nothing
}

public class CopAI : MonoBehaviour
{
    [SerializeField] private float minimumDistance; // how close the AI should get to a waypoint before selecting a new waypoint
    [SerializeField] private bool randomPatrol;
    [SerializeField] private int waypointUpdateDepth;
    [SerializeField] private CopState currentState;
    
    private NavMeshAgent _agent; // get a reference to the agent connected to this gameobject
    public Transform[] waypoints; // get a list of waypoints for the agent to move to
    private int _waypointIndex = 0; // a reference to the currently selected waypoint (value overwritten in Start)
    private Vector3 _target; // the current waypoint target (for checking distance)
    private int _previousWaypoint;
    private Transform _gatePosition;
    private LevelManager _lm;

    void Start()
    {
        _lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        _agent = GetComponent<NavMeshAgent>();
        // set the waypoint and target values so the AI moves when the scene starts
        //_waypointIndex = 1;
        if (randomPatrol) { RandomlySetNextWaypoint(); }; // if patrol is random update the waypoint
        if (currentState == CopState.Patrolling) { UpdateTargetDestinationToWaypoint(); }
    }
    
    private void Update()
    {
        if (currentState == CopState.Patrolling)
        {
            // if the agent is closer than the minimum distance to a waypoint, select a new waypoint and update the target
            if (Vector3.Distance(transform.position, _target) < minimumDistance)
            {
                if (randomPatrol) { RandomlySetNextWaypoint(); }
                else { IterativelySetNextWaypoint(); }
                
                UpdateTargetDestinationToWaypoint();
            }   
        }
        else if (currentState == CopState.Investigating)
        {
            if (Vector3.Distance(transform.position, _target) < minimumDistance + 5)
            {
                AlertFriendlyCop(); // alert nearest cop to the gate to investigate that area
                currentState = CopState.Patrolling; // return to patrolling as usual
                UpdateTargetDestinationToWaypoint(); // Make sure a new target is set to allow for patrol
            }
        }
        
        // If cop is idle then I don't want to do anything with it. This particular state comes in handy to get them
        // watching an area, but also as a flag to update waypoints later!
    }

    // set _target == to the current waypoint's position
    private void UpdateTargetDestinationToWaypoint()
    {
        _target = waypoints[_waypointIndex].position;
        _agent.SetDestination(_target);
    }

    private void IterativelySetNextWaypoint()
    {
        // if index is less than the length of the waypoint array, increment, else point back to the start of the array
        // the - 1 on the length ensures the array does not go out of bounds
        if (_waypointIndex < waypoints.Length - 1)
        {
            _waypointIndex++;
        }
        else
        {
            _waypointIndex = 0;
        }
    }
    
    // select a random waypoint from the list of waypoints, make sure not to select the same waypoint
    private void RandomlySetNextWaypoint()
    {
        int oldWaypointIndex = _waypointIndex;
        // select a random point for the agent to move to, never go back and forth between points
        while (_waypointIndex == _previousWaypoint)
        {
            _waypointIndex = Random.Range(0, waypoints.Length);   
        }

        _previousWaypoint = oldWaypointIndex;
    }
    
    private void AlertFriendlyCop()
    {
        // Get nearest cop
        CopAI nearestCop = _lm.GetNearestCop(_gatePosition);
        // Update that cop's waypoints to be the x nearest waypoints to the gate position
        // the gate position is only set on the cop that visited the lever, meaning the gate position must be relayed
        // to the cop that will visit the gate FROM the cop that visited the lever
        nearestCop.UpdateWaypoints(_gatePosition, waypointUpdateDepth);
    }

    public void SetState(CopState cs, Transform dest, Transform gatePos)
    {
        if (cs == CopState.Investigating)
        {
            Vector3 pos = dest.position; // get the position of the transform for the cop to investigate
            currentState = cs;
            _target = pos;
            _gatePosition = gatePos;
            _agent.SetDestination(pos);
        }
    }

    private void UpdateWaypoints(Transform gatePos, int depth)
    {
        if (currentState != CopState.Idle)
        {
            waypoints = _lm.GetClosestWayPoints(gatePos, depth); // updates the cops waypoints   
        }
        else
        {
            // Do not update the waypoints, this is because better waypoints have been set in the editor for finer
            // control over this particular cop. This is a little rigid as it assumes the cop will not be moving at first
            // until a lever has been pulled. From play-testing, the full random setup was a little troublesome for certain
            // players, so this will allow for a mixture of both settings to avoid overwhelming them.
            UpdateTargetDestinationToWaypoint();
            currentState = CopState.Patrolling;
        }
    }
}
