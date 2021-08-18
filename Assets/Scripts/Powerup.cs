using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private int _powerupID; // 0: Triple Shot, 1: Speed, 2: Shields

    [SerializeField] private AudioClip _audioClip;

    private Player _player;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL in Powerup.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            Vector3 directionVector = _player.transform.position - transform.position;
            directionVector.Normalize();
            transform.Translate(directionVector * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }

        if (transform.position.y < -6.0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_audioClip, transform.position);

            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    case 3:
                        player.CollectAmmo();
                        break;
                    case 4:
                        player.CollectHealth();
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;
                }
            }

            Destroy(this.gameObject);
        }
    }

    public void PowerupDestroy()
    {
        Destroy(this.gameObject);
    }
}
