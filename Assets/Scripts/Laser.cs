using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;
    [SerializeField] private bool _isEnemyLaser = false;
    [SerializeField] private int _laserDirection = 0;


    // Update is called once per frame
    void Update()
    {
        if (_laserDirection == 0)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        CheckOffScreen();
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        CheckOffScreen();
    }

    void CheckOffScreen()
    {
        if (transform.position.y <= -8.0f || transform.position.y >= 9.0f || transform.position.x <= -11.3f || transform.position.x >= 11.3f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser(int direction)
    {
        _isEnemyLaser = true;
        _laserDirection = direction;
    }

    public void RotateLaser(float rotation)
    {
        transform.Rotate(Vector3.forward * rotation);
    }

    public bool GetIsEnemyLaser()
    {
        return _isEnemyLaser;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyLaser == true)
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
                Destroy(this.gameObject);
            }
        }
        
        if (other.tag == "Powerup" && _isEnemyLaser == true)
        {
            Powerup powerup = other.GetComponent<Powerup>();

            if (powerup != null)
            {
                powerup.PowerupDestroy();
                Destroy(this.gameObject);
            }
        }
    }
}
