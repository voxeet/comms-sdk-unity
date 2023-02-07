using UnityEngine;
 
[RequireComponent(typeof(Camera))]
public class Camera : MonoBehaviour 
{
 
    public float moveSpeed = 0.01f;
    public float mouseSensitivity = 1.0f;
    public bool autoLockCursor = false;
 
    void Awake () {
        Cursor.lockState = (autoLockCursor) ? CursorLockMode.Locked : CursorLockMode.None;
    }
   
    void Update () {
        float speed = moveSpeed;
       
        this.gameObject.transform.Translate(Vector3.forward * speed * Input.GetAxis("Vertical"));
        this.gameObject.transform.Translate(Vector3.right * speed * Input.GetAxis("Horizontal"));
        this.gameObject.transform.Translate(Vector3.up * speed * (Input.GetAxis("Jump") + (Input.GetAxis("Fire1") * -1)));
                
        this.gameObject.transform.Rotate(Input.GetAxis("Mouse Y") * mouseSensitivity, Input.GetAxis("Mouse X") * mouseSensitivity, 0.0f);

        if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Cursor.lockState == CursorLockMode.Locked && Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}