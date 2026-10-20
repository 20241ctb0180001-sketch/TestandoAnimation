using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrocaCam : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform pos1;
    [SerializeField] private Transform pos3;
    private bool camSwitch;
    private Transform next;
    private InputAction SwitchCam;

    void Start()
    {
        mainCamera = GetComponentInChildren<Camera>();
        SwitchCam = InputSystem.actions.FindAction("Interact");
    }
    void Update()
    {
        if (SwitchCam.WasPressedThisFrame())
        {
            camSwitch = !camSwitch;
        }
        CSwitch();
    }

    void CSwitch()
    {
        switch (camSwitch)
        {
            case true :
            mainCamera.transform.position = pos3.transform.position;
            break;

            case false :
            mainCamera.transform.position = pos1.transform.position;
            break;

            /*default : 
            mainCamera.transform.position = pos1.transform.position;*/
        }
    }
}