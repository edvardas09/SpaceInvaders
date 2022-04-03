using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DestructionTimer());
    }

    private IEnumerator DestructionTimer()
    {
        yield return new WaitForSeconds(2);
        Destroy();
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
