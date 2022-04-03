using UnityEngine;

public class Health : MonoBehaviour
{
    public int HealthAmount;

    private LevelController levelController;

    private void Awake()
    {
        var eventSystem = GameObject.FindGameObjectWithTag("GameController");
        levelController = eventSystem.GetComponent<LevelController>();
    }

    public void ReceiveDamage(int damageAmount)
    {
        HealthAmount -= damageAmount;
        if (HealthAmount <= 0)
        {
            switch (gameObject.tag)
            {
                case "Player":
                    levelController.OnPlayerDestroyed();
                    break;
                case "Enemy":
                    levelController.AddScore(10);
                    break;
            }
            Destroy(gameObject);
        }
    }
}
