using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _scoreText;
    [SerializeField] private Image _livesImg;
    [SerializeField] private Sprite[] _liveSprites;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Text _restartText;
    [SerializeField] private Slider _thrusterSlider;
    private RectTransform _thrusterSliderRect;
    [SerializeField] private Text _ammoText;

    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _scoreText.text = "Score: " + 0;
        _thrusterSliderRect = _thrusterSlider.fillRect;
        _ammoText.text = "Ammo: " + 15;

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager ==  null)
        {
            Debug.LogError("GameManager is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateThruster(float thrusterCharge)
    {
        _thrusterSliderRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -5, 1.5f * thrusterCharge);
    }

    public void UpdateLives(int currentLives)
    {
        _livesImg.sprite = _liveSprites[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    void GameOverSequence()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        _gameManager.GameOver();
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void UpdateAmmo(int currentAmmo)
    {
        _ammoText.text = "Ammo: " + currentAmmo;
    }
}
