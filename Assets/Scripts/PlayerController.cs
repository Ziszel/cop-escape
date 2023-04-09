using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move parameters")]
    [SerializeField] private float speed;
    
    private float _isHidden; // will track if player is in shadows
    private Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>(); // assign the Rigidbody attached to the player to the local _rb variable
    }

    // Update is called once per frame
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
}
