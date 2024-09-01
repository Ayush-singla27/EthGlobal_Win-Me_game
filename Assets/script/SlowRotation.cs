using UnityEngine;

public class SlowRotation : MonoBehaviour
{
    // Speed of rotation in degrees per second
    public float rotationSpeed = 50f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}