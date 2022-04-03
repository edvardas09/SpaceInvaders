using System.Collections;
using System.Linq;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public GameObject SimpleEnemy;
    public GameObject AdvancedEnemy;
    public Transform[] PathPoints;
    public bool isFromRightSide;

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
        yield return new WaitForSeconds(Random.Range(1f, 3f));

        PathPoints.Last().transform.position = isFromRightSide ?
            PathPoints.Last().transform.position + new Vector3(Random.Range(0f, 15f), 0, 0) :
            PathPoints.Last().transform.position - new Vector3(Random.Range(0f, 15f), 0, 0);

        var enemiesSpawnCount = Random.Range(3, 7);
        enemiesCount = enemiesSpawnCount;
        for (int i = 0; i < enemiesSpawnCount; i++)
        {
            if (Random.value > 0.2)
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

        PathPoints.Last().transform.position = isFromRightSide ?
            PathPoints.Last().transform.position + new Vector3(2, 0, 0) :
            PathPoints.Last().transform.position - new Vector3(2, 0, 0);

        enemyController.Path = PathPoints;
        enemyController.LineController = this;
        enemyController.SetPath();
        newEnemy.SetActive(true);
    }
}