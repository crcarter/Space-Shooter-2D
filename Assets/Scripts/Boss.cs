using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private float _speed = 2.0f;
    private float _rotateSpeed = 20.0f;
    private int _health = 10;

    [SerializeField] private GameObject _explosionPrefab;
    private Player _player;

    private float _fireRate = 2.0f;
    private float _canFire = 2.0f;
    private int _currentWeapon = 0;
    [SerializeField] private GameObject _laserPrefab;

    private float _laserRotation = 30f;
    private int _numSpreadLasers = 7;
    private float _startLaserRotation = -90f;

    private int _numTargetLasers = 5;

    private bool _inPosition = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 10f, 0);

        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveAndRotate();
        if (_inPosition == true)
        {
            FireWeapons();
        }
    }

    void MoveAndRotate()
    {
        if (transform.position.y > 4f)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            _inPosition = true;
            transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
        }
    }

    void FireWeapons()
    {
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(2.0f, 3.0f);
            _canFire = Time.time + _fireRate;

            switch (_currentWeapon)
            {
                case 0:
                    for (int i = 0; i < _numSpreadLasers; i++)
                    {
                        float laserRotation = _startLaserRotation + (_laserRotation * i);
                        FireRotatedLaser(laserRotation);
                    }
                    _currentWeapon = 1;
                    break;
                case 1:
                    if (_numTargetLasers > 0)
                    {
                        Vector3 playerDirectionVector = _player.transform.position - transform.position;
                       // playerDirectionVector.Normalize();
                        float playerAngle = Vector3.Angle(playerDirectionVector, Vector3.down);
                        if (_player.transform.position.x < 0)
                        {
                            playerAngle = playerAngle * -1;
                        }
                        Debug.Log("Angle: " + playerAngle);
                        FireRotatedLaser(playerAngle);
                        _canFire = Time.time + 0.2f;
                        _numTargetLasers--;
                    } else
                    {
                        _numTargetLasers = 5;
                        _currentWeapon = 0;
                    }
                    break;
                default:
                    _currentWeapon = 0;
                    break;
            }
        }
    }

    void FireRotatedLaser(float rotation)
    {
        GameObject laser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        Laser bossLaser = laser.GetComponent<Laser>();
        bossLaser.AssignEnemyLaser(1);
        bossLaser.RotateLaser(rotation);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_inPosition == true)
        {
            if (other.transform.tag == "Player")
            {
                Player player = other.transform.GetComponent<Player>();
                if (player != null)
                {
                    player.Damage();
                }
                TakeDamage();
            }
            if (other.transform.tag == "Laser")
            {
                Laser laser = other.transform.GetComponent<Laser>();
                if (laser.GetIsEnemyLaser() == false)
                {
                    Destroy(other.gameObject);
                    TakeDamage();
                }
            }
        }
    }

    void TakeDamage()
    {
        _health--;
        if (_health == 0)
        {
            if (_player != null)
            {
                _player.AddScore(100);
            }
            GameObject explosionObject = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Explosion explosion = explosionObject.GetComponent<Explosion>();
            explosion.DoubleSize();
            Destroy(this.gameObject, 0.25f);
        }
    }
}
