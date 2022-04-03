using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Tooltip("Shooting frequency")]
    public float FireRate;

    [Tooltip("Projectile prefab")]
    public GameObject ProjectileObject;

    private float nextFire;
    private Health health;
    private bool isImmortal = true;
    private Rect cameraRect;
    private Transform front;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        health = GetComponent<Health>();
        front = gameObject.transform.Find("Front");
    }

    private void Start()
    {
        StartCoroutine(RemoveImmortality());

        var camera = Camera.main;

        var bottomLeft = camera.ScreenToWorldPoint(new Vector3(0, 0, 15));
        var topRight = camera.ScreenToWorldPoint(new Vector3(
            camera.pixelWidth, camera.pixelHeight, 15));

        var collider = gameObject.GetComponent<BoxCollider2D>();

        cameraRect = new Rect(
            bottomLeft.x + collider.bounds.size.x / 2,
            bottomLeft.y + collider.bounds.size.y / 2,
            topRight.x - bottomLeft.x - collider.bounds.size.x,
            topRight.y - bottomLeft.y - collider.bounds.size.y);
    }

    private void Update()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            var localPosition = transform.localPosition;

            localPosition.x = Mathf.Clamp(localPosition.x + Input.GetAxis("Horizontal") * Time.deltaTime * 20, cameraRect.xMin, cameraRect.xMax);
            localPosition.y = Mathf.Clamp(localPosition.y + Input.GetAxis("Vertical") * Time.deltaTime * 20, cameraRect.yMin, cameraRect.yMax);

            transform.localPosition = localPosition;
        }

        if (Input.GetKey(KeyCode.Space) && Time.time > nextFire)
        {
            Instantiate(ProjectileObject, front.transform.position, ProjectileObject.transform.rotation);
            nextFire = Time.time + 1 / FireRate;
        }
    }

    private IEnumerator RemoveImmortality()
    {
        yield return new WaitForSeconds(1);
        isImmortal = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "EnemyProjectile":
            case "Enemy":
                if (!isImmortal)
                {
                    health.ReceiveDamage(1);
                }
                Destroy(collision.gameObject);
                break;
        }
    }
}