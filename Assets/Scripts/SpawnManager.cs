using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject[] _powerups;
    [SerializeField] private int[] _powerupBalance;
    private float _waitTime = 2f;
    [SerializeField] private GameObject _enemyContainer;
    private bool _stopSpawning = false;

    private int _waveNumber;
    private int _numEnemies;
    private int _numEnemiesToSpawn;
    private int _moveType;
    private int _shieldTrigger;

    private UIManager _uiManager;

    [SerializeField] private GameObject _bossPrefab;
    private bool _bossLevel = false;
    private int _bossCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        _waveNumber = 0;
        _numEnemies = 0;
        _numEnemiesToSpawn = 0;

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        //yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            if (_bossLevel == false)
            {
                if (_numEnemies == 0)
                {
                    _waveNumber++;
                    _numEnemies = _waveNumber * 2;
                    _numEnemiesToSpawn = _numEnemies;
                    _moveType = (_waveNumber - 1) % 4;
                    _shieldTrigger = ((_waveNumber - 1) % 12) / 4;
                    _uiManager.UpdateWave(_waveNumber.ToString());
                    if (_waveNumber != 1 && (_waveNumber - 1) % 4 == 0)
                    {
                        _bossLevel = true;
                        GameObject bossSpawn = Instantiate(_bossPrefab, new Vector3(0f, 12f, 0f), Quaternion.identity);
                        Boss boss = bossSpawn.GetComponent<Boss>();
                        boss.SetBossHealth(_bossCount);
                        _uiManager.UpdateWave("Boss");
                    }
                }
                else
                {
                    if (_numEnemiesToSpawn > 0)
                    {
                        bool hasShield = false;
                        switch (_shieldTrigger)
                        {
                            case 0:
                                hasShield = false;
                                break;
                            case 1:
                                if (_numEnemiesToSpawn % 2 == 1)
                                {
                                    hasShield = true;
                                }
                                break;
                            case 2:
                                hasShield = true;
                                break;
                            default:
                                hasShield = false;
                                break;
                        }
                        Vector3 posToSpawn = new Vector3(Random.Range(-8.0f, 8.0f), 7.0f, 0);
                        GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
                        Enemy enemy = newEnemy.GetComponent<Enemy>();
                        enemy.InitializeEnemy(_moveType, hasShield);
                        newEnemy.transform.parent = _enemyContainer.transform;
                        _numEnemiesToSpawn--;
                    }
                }
            }
            
            yield return new WaitForSeconds(_waitTime);
        }
        
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8.0f, 8.0f), 7.0f, 0);
            int randomPowerUp = GetRandomPowerup();
            Instantiate(_powerups[randomPowerUp], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3, 8));
        }
        
    }

    private int GetRandomPowerup()
    {
        int powerup = 0;
        int randomNum = Random.Range(0, 100);
        for (int i = 0; i < _powerupBalance.Length; i++)
        {
            if (randomNum < _powerupBalance[i])
            {
                powerup = i;
                return powerup;
            }
        }

        return powerup;
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public void OnEnemyDeath()
    {
        _numEnemies--;
    }

    public void OnBossDeath()
    {
        _bossLevel = false;
        _bossCount++;
        _uiManager.UpdateWave(_waveNumber.ToString());
    }
}
