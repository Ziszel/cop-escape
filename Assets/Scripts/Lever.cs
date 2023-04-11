using System.Collections;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [Header("Linked Gate")]
    public Transform gatePosition; // one lever to one gate in a scene with multiple gates makes this safe

    [Header("Lever parameters")]
    [SerializeField] private float distanceTolerance;
    [SerializeField] private float rotationDuration;
    [SerializeField] private Vector3 rotationVector;

    private GameObject _playerObj;
    private Transform _playerPosition; // there is only ever one player so GetComponent is better for this variable
    private Transform _handle;
    private bool _handlePressed;

    private void Start()
    {
        _playerObj = GameObject.Find("Player"); // only one player so this is safe
        _playerPosition = _playerObj.transform;
        // https://docs.unity3d.com/ScriptReference/Transform.Find.html
        _handle = transform.Find("Handle"); // Find child by name | FindChild() deprecated
        _handlePressed = false;
    }
    
    private void Update()
    {
        if (Vector3.Distance(_playerPosition.position, transform.position) <= distanceTolerance && !_handlePressed)
        {
            StartCoroutine(RotateLever(rotationDuration));
            StartCoroutine(MoveGate(rotationDuration));
        }
    }

    private IEnumerator RotateLever(float duration)
    {
        // Sets the current rotation of the lever to the specified '_rotationVector' rotation amount.
        // MUST be passed through Quaternion.Euler for Quaternion.Slerp to process it
        // https://stackoverflow.com/questions/36781086/how-to-convert-vector3-to-quaternion
        // https://docs.unity3d.com/ScriptReference/Quaternion.Slerp.html
        // To add two Quaternions together we must times them
        // This addition ensures an update to the end rotation by the amount, instead of setting it directly.
        var endRotation = transform.rotation * Quaternion.Euler(rotationVector);
        
        float time = 0.0f;
        while (time <= 1.0f)
        {
            _handle.transform.rotation = Quaternion.Slerp(_handle.transform.rotation, endRotation, time);
            time += (time + Time.deltaTime) / duration;
            yield return null;
        }
        _handle.transform.rotation = endRotation;
        _handlePressed = true;
    }

    private IEnumerator MoveGate(float duration)
    {

        Vector3 targetPosition = new Vector3(gatePosition.transform.position.x,
            gatePosition.transform.position.y - 5,
            gatePosition.transform.position.z);

        float time = 0.0f;
        while (time <= 1.0f)
        {
            gatePosition.transform.position = Vector3.MoveTowards(gatePosition.transform.position, targetPosition, time);
            time += (time + Time.deltaTime) / duration;
            yield return null;
        }
    }
}
