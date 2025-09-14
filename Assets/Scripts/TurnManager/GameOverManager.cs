using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [SerializeField]
    private RectTransform gameOverPanel;

    [SerializeField]
    private UnityEngine.UI.Button restartButton;

    [SerializeField]
    private TMP_Text levelText;

    [SerializeField]
    public TMP_Text scoreText;

    private void Awake()
    {
        Instance = this;
        gameOverPanel.gameObject.SetActive(false);
    }

    public void GameOver()
    {
        gameOverPanel.gameObject.SetActive(true);
        restartButton.onClick.AddListener(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); });

        scoreText.text = $"Consumed {XpManager.Instance.Xp}";
        levelText.text = $"Level {XpManager.Instance.GetLevel()}";
    }
}