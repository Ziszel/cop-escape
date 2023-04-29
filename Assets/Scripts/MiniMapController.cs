using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    public Transform playerTransform;

    private Vector3 _offset;
    void Start()
    {
        // Force the camera to be in the correct position on the game's start
        AlignCamera();
        _offset = new Vector3(-5.0f, 0.0f, -5.0f);
    }

    // LateUpdate() is called after Update() each frame
    // this ensures the player moves THEN the camera position is recalculated
    // https://www.maxester.com/blog/2020/02/24/how-do-you-make-the-camera-follow-the-player-in-unity-3d/
    private void LateUpdate()
    {
        AlignCamera();
    }

    private void AlignCamera()
    {
        // Set the position of the camera to the new player position with offsets on the x and z to ensure
        // the minimap is positioned correctly when the texture is rendered
        transform.position = new Vector3(playerTransform.position.x + _offset.x, 
            transform.position.y, 
            playerTransform.position.z + _offset.z);
    }
}
