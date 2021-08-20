using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;
    private float _speedMultiplier = 2.0f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShot;
    [SerializeField] private float _fireRate = 0.15f;
    private float _canFire = 0.0f;
    [SerializeField] private int _lives = 3;
    private SpawnManager spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldActive = false;

    [SerializeField] private GameObject _shieldVisualizer;
    private int _shieldHitsLeft;
    private SpriteRenderer _shieldSpriteRenderer;

    [SerializeField] private GameObject _rightEngine;
    [SerializeField] private GameObject _leftEngine;

    [SerializeField] private int _score;
    private UIManager _uiManager;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip _laserAudioClip;

    private float _thrusterBoost = 1.0f;
    private GameObject _thruster;
    private float _thrusterCharge = 100;
    private bool _isThrusterCharged = true;
    private bool _isThrusterActive = false;

    private int _ammoCount;
    private int _maxAmmo = 30;

    private Camera _mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _thruster = GameObject.Find("Thruster");
        _shieldHitsLeft = 0;
        _shieldSpriteRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();
        _ammoCount = 15;
        _mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        if (spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source on the Player is NULL.");
        }
        else
        {
            _audioSource.clip = _laserAudioClip;
        }

        if (_thruster == null)
        {
            Debug.LogError("The Thruster is NULL.");
        }

        if (_shieldSpriteRenderer == null)
        {
            Debug.LogError("The Shield Sprite Renderer is NULL.");
        }

        if (_mainCamera == null)
        {
            Debug.LogError("The Camera is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        
        FireLaser();
        
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        CheckThrusterBoost();

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _speed * _thrusterBoost * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 2f), 0);
        
        if (transform.position.x > 11.3)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    void CheckThrusterBoost()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _isThrusterCharged == true)
        {
            _thrusterBoost = 2.0f;
            _thruster.transform.localScale = new Vector3(1.2f, 1f, 1f);
            _isThrusterActive = true;
            _isThrusterCharged = false;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || _thrusterCharge <= 0)
        {
            _thrusterBoost = 1.0f;
            _thruster.transform.localScale = new Vector3(1f, 1f, 1f);
            _isThrusterActive = false;
        }
        
        if (_isThrusterActive == true)
        {
            _thrusterCharge -= 1f;
        }
        if (_isThrusterActive == false && _isThrusterCharged == false)
        {
            _thrusterCharge += 0.25f;
            if (_thrusterCharge >= 100)
            {
                _isThrusterCharged = true;
            }
        }

        _uiManager.UpdateThruster(_thrusterCharge);
    }

    void FireLaser()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            if (_ammoCount > 0)
            {
                _canFire = Time.time + _fireRate;


                if (_isTripleShotActive)
                {
                    Instantiate(_tripleShot, transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
                }

                _ammoCount--;
                _uiManager.UpdateAmmo(_ammoCount, _maxAmmo);
                _audioSource.Play();
            }
            else
            {
                _audioSource.time = 0.3f;
                _audioSource.Play();
            }
        }
        
    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            _shieldHitsLeft--;
            switch (_shieldHitsLeft)
            {
                case 2:
                    _shieldVisualizer.transform.localScale = new Vector3(2.3f, 2f, 2f);
                    _shieldSpriteRenderer.color = Color.green;
                    break;
                case 1:
                    _shieldVisualizer.transform.localScale = new Vector3(2f, 2f, 2f);
                    _shieldSpriteRenderer.color = Color.blue;
                    break;
                case 0:
                    _isShieldActive = false;
                    _shieldVisualizer.SetActive(false);
                    break;
            }
            
            return;
        }
        _lives--;

        _uiManager.UpdateLives(_lives);

        _mainCamera.TriggerShake();

        if (_lives == 2)
        {
            _rightEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _leftEngine.SetActive(true);
        }
        else if (_lives < 1)
        {
            spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
        _speed /= _speedMultiplier;
    }
    
    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldHitsLeft = 3;
        _shieldVisualizer.SetActive(true);
        _shieldVisualizer.transform.localScale = new Vector3(2.3f, 2.3f, 2f);
        _shieldSpriteRenderer.color = Color.white;
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void CollectAmmo()
    {
        _ammoCount += 10;
        if (_ammoCount > _maxAmmo)
        {
            _ammoCount = _maxAmmo;
        }
        _uiManager.UpdateAmmo(_ammoCount, _maxAmmo);
    }

    public void CollectHealth()
    {
        if (_lives < 3)
        {
            _lives++;
            _uiManager.UpdateLives(_lives);

            if (_lives == 3)
            {
                _rightEngine.SetActive(false);
            }
            else if (_lives == 2)
            {
                _leftEngine.SetActive(false);
            }
        }
    }
}
