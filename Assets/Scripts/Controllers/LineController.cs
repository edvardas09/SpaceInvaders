using System.Collections;
using System.Linq;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [Tooltip("Minimum time to pass before spawning line")]
    public float SpawnDelayMinTime = 1f;
    [Tooltip("Maximum time to pass before spawning line")]
    public float SpawnDelayMaxTime = 3f;

    [Range(1, 7)]
    [Tooltip("Minimum amount of enemies")]
    public int MinEnemyCount = 3;
    [Range(1, 7)]
    [Tooltip("Maximum amount of enemies")]
    public int MaxEnemyCount = 7;

    [Range(0, 100)]
    [Tooltip("Chance for advanced enemy to spawn")]
    public int ChanceForAdvancedEnemySpawn = 20;

    public GameObject SimpleEnemy;
    public GameObject AdvancedEnemy;
    public Transform[] PathPoints;

    private int enemiesCount;
    private Vector3 initialLastPointPosition;

    public void OnShipDestroyed()
    {
        enemiesCount--;
        if (enemiesCount == 0)
        {
            StartCoroutine(SpawnLine());
        }
    }

    private void Start()
    {
        initialLastPointPosition = PathPoints.Last().transform.position;
        StartCoroutine(SpawnLine());
    }

    private IEnumerator SpawnLine()
    {
        PathPoints.Last().transform.position = initialLastPointPosition;
        yield return new WaitForSeconds(Random.Range(SpawnDelayMinTime, SpawnDelayMaxTime));

        PathPoints.Last().transform.position = PathPoints.Last().transform.position - new Vector3(Random.Range(0f, 15f), 0, 0);

        var enemiesSpawnCount = Random.Range(MinEnemyCount, MaxEnemyCount);
        enemiesCount = enemiesSpawnCount;
        for (int i = 0; i < enemiesSpawnCount; i++)
        {
            if (Random.value > ChanceForAdvancedEnemySpawn / 100)
            {
                SpawnEnemy(SimpleEnemy);
            }
            else
            {
                SpawnEnemy(AdvancedEnemy);
            }
            yield return new WaitForSeconds(1);
        }
    }

    private void SpawnEnemy(GameObject enemy)
    {
        GameObject newEnemy;
        newEnemy = Instantiate(enemy, enemy.transform.position, Quaternion.identity);
        var enemyController = newEnemy.GetComponent<EnemyController>();

        PathPoints.Last().transform.position = PathPoints.Last().transform.position - new Vector3(2, 0, 0);

        enemyController.Path = PathPoints;
        enemyController.LineController = this;
        enemyController.SetPath();
        newEnemy.SetActive(true);
    }
}