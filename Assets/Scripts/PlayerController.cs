using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move parameters")]
    [SerializeField] private float speed;
    
    private bool _isHidden; // will track if player is in shadows
    private Rigidbody _rb;
    private Transform _tr;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>(); // assign the Rigidbody attached to the player to the local _rb variable
        _isHidden = false;
    }
    
    private void FixedUpdate()
    {
        float xSpeed = Input.GetAxisRaw("Horizontal");
        float zSpeed = Input.GetAxisRaw("Vertical");

        if (xSpeed != 0 || zSpeed != 0)
        {
            MovePlayer(xSpeed, zSpeed);
        }
    }

    // Movement handled by Rigidbody, therefore no need to adjust values by Time.deltatime
    // Only call this method from FixedUpdate()
    private void MovePlayer(float xSpeed, float zSpeed)
    {
        // forward / backward movement
        if (zSpeed > 0)
        {
            _rb.AddForce(transform.forward * speed);
        }
        else if (zSpeed < 0)
        {
            _rb.AddForce(transform.forward * -speed);
        }
        
        // Left / right movement
        if (xSpeed > 0)
        {
            _rb.AddForce(transform.right * speed);
        }
        else if (xSpeed < 0)
        {
            _rb.AddForce(transform.right * -speed);
        }
    }

    public bool GetHiddenValue()
    {
        return _isHidden;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("SmokeArea"))
        {
            _isHidden = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("SmokeArea"))
        {
            _isHidden = false;
        }
    }
}
