using System.Collections;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>()?.gameObject;
        StartCoroutine(SelfDestroy());
    }

    private void Update()
    {
        if (player == null)
        {
            transform.Translate(Vector3.right * 5 * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.005f);
            RotateTowardsPlayer();
        }
    }

    private void RotateTowardsPlayer()
    {
        var vectorToTarget = player.transform.position - transform.position;
        var angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        var angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, angleAxis, Time.deltaTime * 10);
    }

    private IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(4.5f);
        Destroy(gameObject);
    }
}
