using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    public Transform playerTransform;

    private Vector3 _offset;
    void Start()
    {
        AlignCamera();
        _offset = new Vector3(15.0f, 0.0f, 15.0f);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        AlignCamera();
    }

    private void AlignCamera()
    {
        transform.position = new Vector3(playerTransform.position.x + _offset.x,
            transform.position.y,
            playerTransform.position.z + _offset.z);
    }
}
