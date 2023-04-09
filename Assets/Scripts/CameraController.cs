using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController player;
    private Vector3 _offset;
    
    // Use this for initialization
    void Start () 
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        _offset = transform.position - player.transform.position;
    }

    // LateUpdate() is called after Update() each frame
    // this ensures the player moves THEN the camera position is recalculated
    // https://www.maxester.com/blog/2020/02/24/how-do-you-make-the-camera-follow-the-player-in-unity-3d/
    private void LateUpdate()
    {
        // Update the camera position to track the player
        transform.position = (player.transform.position + _offset);

    }
}