﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver;

    // Start is called before the first frame update
    void Start()
    {
        _isGameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            SceneManager.LoadScene(0); //Current Game Scene
        }
    }

    public void GameOver()
    {
        _isGameOver = true;
    }
}