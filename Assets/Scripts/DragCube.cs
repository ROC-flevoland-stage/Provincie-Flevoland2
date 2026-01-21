using UnityEngine;

public class DragCube : MonoBehaviour
{
    Camera cam;
    bool dragging;
    float fixedY;
    FileMovement mover; // zodat het ni moved waanneer we draggen

    void Start()
    {
        cam = Camera.main;
        fixedY = transform.position.y;
        mover = GetComponent<FileMovement>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    dragging = true;
                    // niet bewegen als ik drag
                    if (mover != null)
                        mover.enabled = false;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
            // moven als ik ni drag
            if (mover != null)
                mover.enabled = true;
        }

        if (dragging)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            float distance = (fixedY - ray.origin.y) / ray.direction.y;
            Vector3 pos = ray.origin + ray.direction * distance;
            transform.position = new Vector3(pos.x, fixedY, pos.z);
        }
    }
}
