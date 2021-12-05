using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    private float zoom;

    private void FixedUpdate()
    {
        transform.position = target.position+offset;
    }

    private void Update()
    {
        Zoom();
    }

    private void Zoom()
    {
        float zoomAmount = 80f;

        if (Input.mouseScrollDelta.y>0)
        {
            zoom -= zoomAmount * Time.deltaTime *10f;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoom += zoomAmount * Time.deltaTime;
        }

        zoom = Mathf.Clamp(zoom, 40f, 250f);
    }
}
