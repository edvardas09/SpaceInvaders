using UnityEngine;

public class DirectMovement : MonoBehaviour
{
    [Tooltip("Movement speed on Y axis in local space")]
    public float Speed = 20;

    private void Update()
    {
        transform.Translate(Vector3.up * Speed * Time.deltaTime, Space.World);
    }
}