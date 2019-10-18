using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    #region Singleton
    public static GameManager GM;

    private void Awake()
    {
        GM = this;
        killCount = 0;
        deathCount = 0;
        DeadZone = -40;
        _currentScene = SceneManager.GetActiveScene();
        playerGravityScale = 3.1f;
    }
    #endregion

    public bool _debugMode = false;

    public GameObject Player;

    public LayerMask environmentLayer;

    public LayerMask playerLayer;

    public Image playerAttackCooldown;

    public TextMeshProUGUI killCountText;

    public int killCount;

    public int deathCount;

    public GameObject MessageBoard;

    public float DeadZone;

    public float playerGravityScale;

    public GameObject PlayerSpawn;

    private Scene _currentScene;

    public void ReloadScene()
    {
        SceneManager.LoadScene(_currentScene.name);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
