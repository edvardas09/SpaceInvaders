using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Tooltip("Enemy's type")]
    public EnemyType EnemyType;

    [Tooltip("Enemy's projectile prefab")]
    public GameObject Projectile;

    [Tooltip("Enemy's rocket prefab")]
    public GameObject Rocket;

    [Tooltip("Minimum time to pass before shooting projectile")]
    public float ProjectileMinTime = 1f;
    [Tooltip("Maximum time to pass before shooting projectile")]
    public float ProjectileMaxTime = 3f;

    [Tooltip("Minimum time to pass before shooting rocket")]
    public float RocketMinTime = 3f;
    [Tooltip("Maximum time to pass before shooting rocket")]
    public float RocketMaxTime = 5f;

    [Tooltip("Movement speed when spawned")]
    public float Speed = 40;

    [HideInInspector]
    public Transform[] Path;

    [HideInInspector] 
    public LineController LineController;

    private Health health;
    private Transform front;
    private float currentPathPercent;
    private Vector3[] pathPositions;
    private bool movingIsActive;
    private bool shouldRotate = true;

    public void SetPath()
    {
        currentPathPercent = 0;
        pathPositions = new Vector3[Path.Length];
        for (int i = 0; i < pathPositions.Length; i++)
        {
            pathPositions[i] = Path[i].position;
        }
        transform.position = NewPositionByPath(pathPositions, 0);
        transform.rotation = Quaternion.identity;
        movingIsActive = true;
    }

    private void Awake()
    {
        health = GetComponent<Health>();
        front = gameObject.transform.Find("Front");
    }

    private void Update()
    {
        if (movingIsActive)
        {
            currentPathPercent += Speed / 100 * Time.deltaTime;

            transform.position = NewPositionByPath(pathPositions, currentPathPercent);
            transform.right = Interpolate(CreatePoints(pathPositions), currentPathPercent + 0.01f) - transform.position;
            transform.Rotate(Vector3.forward * -90);
            if (currentPathPercent > 1)
            {
                movingIsActive = false;
                switch (EnemyType)
                {
                    case EnemyType.Simple:
                        Invoke(nameof(Shooting), UnityEngine.Random.Range(ProjectileMinTime, ProjectileMaxTime));
                        break;
                    case EnemyType.Advanced:
                        Invoke(nameof(Shooting), UnityEngine.Random.Range(ProjectileMinTime, ProjectileMaxTime));
                        Invoke(nameof(LaunchRocket), UnityEngine.Random.Range(RocketMinTime, RocketMaxTime));
                        break;
                }
            }
        }
        else if (shouldRotate)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(0, 0, 180, 0), Time.deltaTime * 0.01f);
            if (transform.rotation.z == 1 || transform.rotation.z == -1)
            {
                shouldRotate = false;
            }
        }
    }

    private void Shooting()
    {
        Instantiate(Projectile, front.transform.position, Projectile.transform.rotation);
        Invoke(nameof(Shooting), UnityEngine.Random.Range(ProjectileMinTime, ProjectileMaxTime));
    }

    private void LaunchRocket()
    {
        Instantiate(Rocket, front.transform.position, Rocket.transform.rotation);
        Invoke(nameof(LaunchRocket), UnityEngine.Random.Range(RocketMinTime, RocketMaxTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Projectile":
            case "EnemyProjectile":
            case "Player":
                if (health.HealthAmount == 1)
                {
                    LineController.OnShipDestroyed();
                }
                health.ReceiveDamage(1);
                Destroy(collision.gameObject);
                break;
        }
    }

    private Vector3 NewPositionByPath(Vector3[] pathPos, float percent)
    {
        return Interpolate(CreatePoints(pathPos), currentPathPercent);
    }

    private Vector3 Interpolate(Vector3[] path, float t)
    {
        int numSections = path.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);
        float u = t * numSections - currPt;
        Vector3 a = path[currPt];
        Vector3 b = path[currPt + 1];
        Vector3 c = path[currPt + 2];
        Vector3 d = path[currPt + 3];
        return 0.5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) + (-a + c) * u + 2f * b);
    }

    private Vector3[] CreatePoints(Vector3[] path)
    {
        Vector3[] pathPositions;
        Vector3[] newPathPos;
        int dist = 2;
        pathPositions = path;
        newPathPos = new Vector3[pathPositions.Length + dist];
        Array.Copy(pathPositions, 0, newPathPos, 1, pathPositions.Length);
        newPathPos[0] = newPathPos[1] + (newPathPos[1] - newPathPos[2]);
        newPathPos[newPathPos.Length - 1] = newPathPos[newPathPos.Length - 2] + (newPathPos[newPathPos.Length - 2] - newPathPos[newPathPos.Length - 3]);
        if (newPathPos[1] == newPathPos[newPathPos.Length - 2])
        {
            Vector3[] LoopSpline = new Vector3[newPathPos.Length];
            Array.Copy(newPathPos, LoopSpline, newPathPos.Length);
            LoopSpline[0] = LoopSpline[LoopSpline.Length - 3];
            LoopSpline[LoopSpline.Length - 1] = LoopSpline[2];
            newPathPos = new Vector3[LoopSpline.Length];
            Array.Copy(LoopSpline, newPathPos, LoopSpline.Length);
        }
        return newPathPos;
    }
}