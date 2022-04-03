using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public int PlayerLives = 3;
    public Text ScoreText;
    public Text PlayerLivesText;

    private int score;

    private void Start()
    {
        PlayerLivesText.text = PlayerLives.ToString();
    }

    public void OnPlayerDestroyed()
    {
        PlayerLives--;
        PlayerLivesText.text = PlayerLives.ToString();
        if (PlayerLives > 0)
        {
            StartCoroutine(SpawnPlayer());
        }
    }

    public void AddScore(int scoreAmount)
    {
        score += scoreAmount;
        ScoreText.text = score.ToString();
    }

    private IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(3);
        Instantiate(PlayerPrefab);
    }
}