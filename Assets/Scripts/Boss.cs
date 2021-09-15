using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private float _speed = 2.0f;
    private float _rotateSpeed = 20.0f;
    [SerializeField] private int _health = 1;

    [SerializeField] private GameObject _explosionPrefab;
    private Player _player;

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
        MoveDown();
    }

    void MoveDown()
    {
        if (transform.position.y > 4f)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
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
