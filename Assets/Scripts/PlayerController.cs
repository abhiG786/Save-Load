using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotationSpeed = 100.0f;
    public Camera mainCamera;
    public float cameraSmoothSpeed = 0.125f;

    private Vector3 cameraOffset;

    void Start()
    {
        if (mainCamera != null)
        {
            cameraOffset = mainCamera.transform.position - transform.position;
        }
    }

    void Update()
    {
        float moveVertical = Input.GetAxis("Vertical");
        float moveHorizontal = Input.GetAxis("Horizontal");

        // Move the vehicle forward/backward
        transform.Translate(Vector3.forward * moveVertical * speed * Time.deltaTime);

        // Rotate the vehicle left/right
        float rotation = moveHorizontal * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotation);

        // Smooth camera follow
        if (mainCamera != null)
        {
            Vector3 desiredPosition = transform.position + cameraOffset;
            Vector3 smoothedPosition = Vector3.Lerp(mainCamera.transform.position, desiredPosition, cameraSmoothSpeed);
            mainCamera.transform.position = smoothedPosition;
            mainCamera.transform.LookAt(transform.position);
        }
    }
}
