using UnityEngine;
using DG.Tweening;

public class ZoomCameraScript : MonoBehaviour
{
    [SerializeField] private Camera zoomCamera;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject player;
    private Bounds cameraBounds;
    private Vector3 targetPosition;
    private bool inZoom = false;
    private bool active = false;
    public float tweenspeed;

    Tween shrinkTween;
    Tween growTween;

    void Start()
    {
        DOTween.defaultAutoKill = false;
        shrinkTween = DOTween.To(() => zoomCamera.orthographicSize, x => zoomCamera.orthographicSize = x, 2, 1);
        growTween = DOTween.To(() => zoomCamera.orthographicSize, x => zoomCamera.orthographicSize = x, 7, 1);

        float height = zoomCamera.orthographicSize;
        float width = height * zoomCamera.aspect;

        float minX = 0 - (width * 2);
        float maxX = 0 + (width * 2);

        float minY = -100 - (height * 2);
        float maxY = -100 + (height * 2);



        cameraBounds = new Bounds();
        cameraBounds.SetMinMax(
            new Vector3(minX, minY, 0.0f),
            new Vector3(maxX, maxY, 0.0f)
            );
        print(cameraBounds);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (inZoom == true && active == false) targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
        else if (inZoom == false && active == false) targetPosition = new Vector3(0f, -100f, -10f);
        if (active == false) targetPosition = GetCameraBounds();

        if (active == false)transform.position = targetPosition;
    }

    private Vector3 GetCameraBounds()
    {
        return new Vector3(
            Mathf.Clamp(targetPosition.x, cameraBounds.min.x, cameraBounds.max.x),
            Mathf.Clamp(targetPosition.y, cameraBounds.min.y, cameraBounds.max.y),
            transform.position.z
            );
    }

    public void shrink()
    {
        active = true;
        mainCamera.enabled = false;
        zoomCamera.enabled = true;
        inZoom = true;
        DOTween.To(() => zoomCamera.orthographicSize, x => zoomCamera.orthographicSize = x, 2, tweenspeed).OnComplete(() => SetCameraSize());
        DOTween.To(() => transform.position, x => transform.position = x, new Vector3(player.transform.position.x, player.transform.position.y, -10f), tweenspeed * 2);
        
    }

    public void grow()
    {
        active = true;
        //mainCamera.enabled = true;
        //zoomCamera.enabled = false;
        inZoom = false;
        DOTween.To(() => zoomCamera.orthographicSize, x => zoomCamera.orthographicSize = x, 7, tweenspeed * 2).OnComplete(() => SetCameraSize());
        DOTween.To(() => transform.position, x => transform.position = x, new Vector3(0f, -100f, -10f), tweenspeed);
    }

    private void SetCameraSize()
    {
        if (inZoom == true)
        {
            float height = zoomCamera.orthographicSize;
            float width = height * zoomCamera.aspect;

            float minX = (Globals.WorldBounds.min.x + width) * (1f + (0.455f));
            float maxX = (Globals.WorldBounds.extents.x - width) * (1f + (0.455f));

            float minY = Globals.WorldBounds.min.y + height;
            float maxY = Globals.WorldBounds.max.y - height;



            cameraBounds = new Bounds();
            cameraBounds.SetMinMax(
                new Vector3(minX, minY, 0.0f),
                new Vector3(maxX, maxY, 0.0f)
                );
        }
        else
        {
            float height = zoomCamera.orthographicSize;
            float width = height * zoomCamera.aspect;

            float minX = 0 - (width);
            float maxX = 0 + (width);

            float minY = -100 - (height * 2);
            float maxY = -100 + (height * 2);



            cameraBounds = new Bounds();
            cameraBounds.SetMinMax(
                new Vector3(minX, minY, 0.0f),
                new Vector3(maxX, maxY, 0.0f)
                );
        }
        active = false;
    }
}
