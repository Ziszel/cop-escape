using UnityEngine;
using UnityEngine.AI; // allows access to navmesh agent

// Script created with help from:
// https://docs.unity3d.com/2019.4/Documentation/Manual/nav-BuildingNavMesh.html
// https://docs.unity3d.com/2019.4/Documentation/Manual/nav-CreateNavMeshAgent.html
// https://docs.unity3d.com/Packages/com.unity.ai.navigation@1.1/manual/NavAgentPatrol.html
// https://www.youtube.com/watch?v=c8Nq19gkNfs
public class CopAI : MonoBehaviour
{
    [SerializeField]private float minimumDistance; // how close the AI should get to a waypoint before selecting a new waypoint
    
    private NavMeshAgent _agent; // get a reference to the agent connected to this gameobject
    public Transform[] waypoints; // get a list of waypoints for the agent to move to
    private int _waypointIndex; // a reference to the currently selected waypoint
    private Vector3 _target; // the current waypoint target (for checking distance)
    private int _previousWaypoint;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        // set the waypoint and target values so the AI moves when the scene starts
        //_waypointIndex = 1;
        SetNextWaypoint();
        UpdateTargetDestination();
    }
    
    private void Update()
    {
        // if the agent is closer than the minimum distance to a waypoint, select a new waypoint and update the target
        if (Vector3.Distance(transform.position, _target) < minimumDistance)
        {
            SetNextWaypoint();
            UpdateTargetDestination();
        }
    }

    // set _target == to the current waypoint's position
    private void UpdateTargetDestination()
    {
        _target = waypoints[_waypointIndex].position;
        _agent.SetDestination(_target);
    }

    // select a random waypoint from the list of waypoints, make sure not to select the same waypoint
    private void SetNextWaypoint()
    {
        int oldWaypointIndex = _waypointIndex;
        // select a random point for the agent to move to, never go back and forth between points
        while (_waypointIndex == _previousWaypoint)
        {
            _waypointIndex = Random.Range(0, waypoints.Length);   
        }

        _previousWaypoint = oldWaypointIndex;
    }
}
