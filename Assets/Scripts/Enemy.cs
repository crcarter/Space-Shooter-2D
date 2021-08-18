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
    private bool _isAlive;

    [SerializeField] private int _moveType;
    private Vector3 _moveDirection;
    private Vector3 _moveCurve;

    private void Start()
    {
        _isAlive = true;
        _moveType = Random.Range(0, 3);
        SetMoveDirection();

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
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire && _isAlive == true)
        {
            FireLaser();
        }
        if (Time.time > _canFireAtPowerup && _isAlive == true)
        {
            CheckForPowerups();
        }
    }
        
    void CalculateMovement()
    {
        transform.Translate(_moveDirection * _speed * Time.deltaTime);
        if (_moveType == 2)
        {
            _moveDirection += _moveCurve;
            _moveDirection = _moveDirection.normalized;
        }

        if (transform.position.y < -5.0f)
        {
            float randomX = Random.Range(-8.0f, 8.0f);
            transform.position = new Vector3(randomX, 7.0f, 0);
            SetMoveDirection();
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
                if (transform.position.x > 0)
                {
                    _moveDirection = Vector3.down + Vector3.left;
                }
                else
                {
                    _moveDirection = Vector3.down + Vector3.right;
                }
                _moveDirection = _moveDirection.normalized;
                break;
            case 2:
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

    void FireLaser()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }

            EnemyDeath();
        }
        else if (other.transform.tag == "Laser")
        {
            Laser laser = other.transform.GetComponent<Laser>();
            if (laser.GetIsEnemyLaser() == false)
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
    
    private void EnemyDeath()
    {
        _anim.SetTrigger("OnEnemyDeath");
        _speed = 0;
        _audioSource.Play();
        _isAlive = false;

        Destroy(GetComponent<Collider2D>());
        Destroy(this.gameObject, 2.8f);
    }

    private void CheckForPowerups()
    {
        Collider2D hitCollider = Physics2D.OverlapBox(gameObject.transform.position + new Vector3(0f, -5f), new Vector3(0.18f, 4f), 0f);

        if (hitCollider != null)
        {
            if (hitCollider.tag == "Powerup")
            {
                FireLaser();
                _canFireAtPowerup = Time.time + _fireRate;
            }
        }
    }
}
