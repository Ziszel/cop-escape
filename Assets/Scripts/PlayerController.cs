using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move parameters")]
    [SerializeField] private float speed; // Movement speed
    [SerializeField] private float rotationSpeed;
    
    private bool _isHidden; // will track if player is in shadows
    private Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>(); // assign the Rigidbody attached to the player to the local _rb variable
        _isHidden = false;
    }
    
    private void FixedUpdate()
    {
        // Stop the player moving if they are caught
        if (LevelManager.GetGameState() == GameState.MainLoop)
        {
            float xSpeed = Input.GetAxisRaw("Horizontal");
            float zSpeed = Input.GetAxisRaw("Vertical");

            if (xSpeed != 0 || zSpeed != 0)
            {
                MovePlayer(xSpeed, zSpeed);
                RotatePlayer();
            }
        }
        
    }

    // Movement handled by Rigidbody, therefore no need to adjust values by Time.deltatime
    // Only call this method from FixedUpdate()
    private void MovePlayer(float xSpeed, float zSpeed)
    {
        // forward / backward movement
        // https://docs.unity3d.com/ScriptReference/Rigidbody.AddForce.html
        if (zSpeed > 0)
        {
            _rb.AddForce(new Vector3(0.0f, 0.0f, speed));
        }
        else if (zSpeed < 0)
        {
            _rb.AddForce(new Vector3(0.0f, 0.0f, -speed));
        }
        
        // Left / right movement
        if (xSpeed > 0)
        {
            _rb.AddForce(new Vector3(speed, 0.0f, 0.0f));
        }
        else if (xSpeed < 0)
        {
            _rb.AddForce(new Vector3(-speed, 0.0f, 0.0f));
        }
    }

    private void RotatePlayer()
    {
        // the vector in which I want to look, and that == the current movement
        // https://forum.unity.com/threads/how-quaternion-lookrotation-works.985800/
        Vector3 movement = new Vector3(_rb.velocity.x, 0.0f, _rb.velocity.z);
        
        if (movement != Vector3.zero) // Ensures no 'Look Rotation Viewing Vector is Zero' error
        {
            // spherical lerp from the current rotation to the angle the player is attempting to move to
            // slerp treats a vector like a direction rather than a position which is perfect for working with rotations
            // https://www.reddit.com/r/Unity3D/comments/6iskah/movetowards_vs_lerp_vs_slerp_vs_smoothdamp/
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement),
                rotationSpeed * Time.deltaTime);
        }
    }

    // Getter for _isHidden
    public bool GetHiddenValue()
    {
        return _isHidden;
    }

    // Keep the collision logic for the player IN the player class
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("SmokeArea"))
        {
            _isHidden = true;
        }
        
        if (col.CompareTag("EndTransform"))
        {
            LevelManager.EndGame();
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
