using UnityEngine;

public class ZoomCameraScript : MonoBehaviour
{
    [SerializeField] private Camera zoomCamera;
    [SerializeField] private GameObject player;
    private Bounds cameraBounds;
    private Vector3 targetPosition;
    

    void Start()
    {
        float height = zoomCamera.orthographicSize;
        float width = height * zoomCamera.aspect;

        float minX = (Globals.WorldBounds.min.x + width) * (1f +(0.455f));
        float maxX = (Globals.WorldBounds.extents.x - width) * (1f +(0.455f));

        float minY = Globals.WorldBounds.min.y + height;
        float maxY = Globals.WorldBounds.max.y - height;



        cameraBounds = new Bounds();
        cameraBounds.SetMinMax(
            new Vector3(minX, minY, 0.0f),
            new Vector3(maxX, maxY, 0.0f)
            );
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
        targetPosition = GetCameraBounds();

        transform.position = targetPosition;
    }

    private Vector3 GetCameraBounds()
    {
        return new Vector3(
            Mathf.Clamp(targetPosition.x, cameraBounds.min.x, cameraBounds.max.x),
            Mathf.Clamp(targetPosition.y, cameraBounds.min.y, cameraBounds.max.y),
            transform.position.z
            );
    }
}
