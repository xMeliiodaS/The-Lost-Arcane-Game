using UnityEngine;

public class BillBoaard : MonoBehaviour
{
    private Transform cam;
    [SerializeField] private Transform cutSceneCamera;

    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
        if(cutSceneCamera != null )
        {
            transform.LookAt(transform.position + cutSceneCamera.forward);
        }
    }
}
