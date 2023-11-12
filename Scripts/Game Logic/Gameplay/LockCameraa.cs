using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCameraa : MonoBehaviour
{
    #region Singelton
    // One inventory at all times
    private static LockCameraa instance;
    //public InventorySlot[] slots;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one instance of Inventory found!");
            return;
        }
        instance = this;
    }
    public static LockCameraa GetInstance()
    {
        return instance;
    }

    #endregion

    private void Start()
    {
        //Application.targetFrameRate = 60;
    }

    [SerializeField] private GameObject cameraRotationObject;     // Reference to the camera follow player
    private Quaternion initialRotation;                           // Reference to the camera follow player rotation

    public void ShowMouseCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideMouseCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LockCameraRotation()
    {
        GameObject player = GameObject.FindWithTag("Player");
        cameraRotationObject.transform.rotation = initialRotation;

        player.GetComponent<CameraRotation>().enabled = false;
    }

    public void UnlockCameraRotation()
    {
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<CameraRotation>().enabled = true;
    }
}
