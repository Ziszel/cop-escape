using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move parameters")]
    [SerializeField] private float speed;
    
    private bool _isDucking;
    private bool _isHidden; // will track if player is in shadows
    private Rigidbody _rb;
    private Transform _tr;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>(); // assign the Rigidbody attached to the player to the local _rb variable
        _isDucking = false;
        _isHidden = false;
    }

    private void Update()
    {
        CheckDucking();
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

    private void CheckDucking()
    {
        // avoids multiple retrievals of transform.position
        Vector3 currentTransform = transform.position;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            _isDucking = true;
            transform.position = new Vector3(currentTransform.x, 0.5f, currentTransform.z);
        }
        else
        {
            _isDucking = false;
            transform.position = new Vector3(currentTransform.x, 2.0f, currentTransform.z);
        }
    }
}
