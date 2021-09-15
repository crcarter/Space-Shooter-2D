using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField] private GameObject _laserPrefab;


    private Player _player;
    private Animator _anim;

    private AudioSource _audioSource;

    private float _fireRate = 3.0f;
    [SerializeField] private float _canFire = -1f;
    private float _canFireAtPowerup = -1f;
    private float _canFireBehind = -1f;
    private bool _isAlive;

    [SerializeField] private int _moveType = 0;
    private Vector3 _moveDirection;
    private Vector3 _moveCurve;

    private int _laserDown = 1;
    private int _laserUp = 0;

    [SerializeField] private GameObject _shieldVisualizer;
    private bool _isShieldActive = false;

    [SerializeField] private float _aggroRange = 4f;

    private SpawnManager _spawnManager;

    private void Start()
    {
        _isAlive = true;

        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL.");
        }

        _anim = this.GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("The Animator is NULL.");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source on the Enemy is NULL.");
        }

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire && _isAlive == true)
        {
            FireLaser(_laserDown);
        }
        if (Time.time > _canFireAtPowerup && _isAlive == true)
        {
            CheckForPowerups();
        }
        if (Time.time > _canFireBehind && _isAlive == true)
        {
            CheckForPlayerBehind();
        }
    }
        
    void CalculateMovement()
    {
        if (_player != null && Vector3.Distance(gameObject.transform.position, _player.transform.position) <= _aggroRange)
        {
            Vector3 directionVector = _player.transform.position - transform.position;
            directionVector.Normalize();
            transform.Translate(directionVector * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(_moveDirection * _speed * Time.deltaTime);
            if (_moveType == 3)
            {
                _moveDirection += _moveCurve;
                if (_moveDirection.y > -0.5f)
                {
                    _moveDirection.y = -0.5f;
                }
                _moveDirection = _moveDirection.normalized;
            }

            if (transform.position.x > 11.3)
            {
                transform.position = new Vector3(-11.3f, transform.position.y, 0);
            }
            else if (transform.position.x < -11.3)
            {
                transform.position = new Vector3(11.3f, transform.position.y, 0);
            }

            if (transform.position.y < -5.0f)
            {
                float randomX = Random.Range(-8.0f, 8.0f);
                transform.position = new Vector3(randomX, 7.0f, 0);
                SetMoveDirection();
            }
        }
        
    }

    void SetMoveDirection()
    {

        switch (_moveType)
        {
            case 0:
                _moveDirection = Vector3.down;
                break;
            case 1:
                _moveDirection = Vector3.down + Vector3.left;
                _moveDirection = _moveDirection.normalized;
                break;
            case 2:
                _moveDirection = Vector3.down + Vector3.right;
                _moveDirection = _moveDirection.normalized;
                break;
            case 3:
                if (transform.position.x > 0)
                {
                    _moveDirection = Vector3.down + Vector3.left;
                    _moveCurve = new Vector3(0.003f, 0f, 0f);
                }
                else
                { 
                    _moveDirection = Vector3.down + Vector3.right;
                    _moveCurve = new Vector3(-0.003f, 0f, 0f);
                }
                _moveDirection = _moveDirection.normalized;
                break;
            default:
                _moveDirection = Vector3.down;
                break;
        }
    }

    void FireLaser(int direction)
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;

        Vector3 laserPosition = transform.position;
        if (direction == _laserUp)
        {
            laserPosition += new Vector3(0f, 2f, 0f);
        }
        GameObject enemyLaser = Instantiate(_laserPrefab, laserPosition, Quaternion.identity);
        
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser(direction);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                if (_isShieldActive == true)
                {
                    _isShieldActive = false;
                    _shieldVisualizer.SetActive(false);
                    player.Damage();
                }
                player.Damage();
            }

            EnemyDeath();
        }
        else if (other.transform.tag == "Laser")
        {
            Laser laser = other.transform.GetComponent<Laser>();
            if (laser.GetIsEnemyLaser() == false)
            {
                if (_isShieldActive == true)
                {
                    _isShieldActive = false;
                    _shieldVisualizer.SetActive(false);
                    Destroy(other.gameObject);
                }
                else
                {
                    if (_player != null)
                    {
                        _player.AddScore(10);
                    }
                    Destroy(other.gameObject);

                    EnemyDeath();
                }
            }
        }
    }
    
    private void EnemyDeath()
    {
        _spawnManager.OnEnemyDeath();

        _anim.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _audioSource.Play();
        _isAlive = false;

        Destroy(GetComponent<Collider2D>());
        Destroy(this.gameObject, 2.8f);
    }

    private void CheckForPowerups()
    {
        Collider2D hitCollider = Physics2D.OverlapBox(gameObject.transform.position + new Vector3(0f, -5f, 0f), new Vector3(0.18f, 4f, 0f), 0f);

        if (hitCollider != null)
        {
            if (hitCollider.tag == "Powerup")
            {
                FireLaser(_laserDown);
                _canFireAtPowerup = Time.time + _fireRate;
            }
        }
    }

    private void CheckForPlayerBehind()
    {
        Collider2D hitCollider = Physics2D.OverlapBox(gameObject.transform.position + new Vector3(0f, 5f, 0f), new Vector3(0.18f, 4f, 0f), 0f);

        if (hitCollider != null)
        {
            if (hitCollider.tag == "Player")
            {
                FireLaser(_laserUp);
                _canFireBehind = Time.time + _fireRate;
            }
        }
    }

    public void ActivateShield()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
    }

    public void InitializeEnemy(int moveType, bool hasShield)
    {
        _moveType = moveType;
        SetMoveDirection();
        if (hasShield == true)
        {
            ActivateShield();
        } 
    }
}
