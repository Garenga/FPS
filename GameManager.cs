using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Transform[] spawnPoints;
    public Transform respawnPosition;

    public int spawnedEnemies;
    public int levelEnemies;//koliko je Enemy-a potrebno ubiti da se aktivira Boss portal
    public int killedEnemies;

    public GameObject bossArena;

    public GameObject[] enemies;
    public GameObject[] weapons;
    public List<GameObject> playerItemes;//(NE KORISTI SE)

    public PlayerHealth health;
    public Gun guncontroller;
    public GameObject deathPanel;
    public GameObject UIShopPanel;
    public GameObject UIMenuPanel;

    public bool boughtweapon_GreenFlare = false;
    public int score;//kolicina novaca
    public TextMeshProUGUI UIScorePoints;

    public TextMeshProUGUI UILevelEnemies;

    public GameObject bossTeleport;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, 2f);

        for (int i = 0; i < weapons.Length; i++)//aktivira prvi gun
        {
            if (i != 0)
            {
                weapons[i].SetActive(false);
            }
        }
        UIScorePoints.text=score.ToString()+" $";
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        deathPanel.SetActive(false);
        UIMenuPanel.SetActive(false);

        bossTeleport.GetComponent<Collider>().enabled = false;

    }

    void SpawnEnemy()
    {
        if (spawnedEnemies <= levelEnemies)
        {
            int randomPosition = Random.Range(0, spawnPoints.Length);
            int randomEnemy = Random.Range(0, enemies.Length);

            Instantiate(enemies[randomEnemy], spawnPoints[randomPosition].position, Quaternion.identity);
            spawnedEnemies++;
        }
    }

    void BossUnlockCondition()
    {
        if (killedEnemies == levelEnemies)
        {
            bossArena.SetActive(true);
            bossTeleport.SetActive(true);
        }
    }

    void ChangeWeapons(int indexActive)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].GetComponent<Gun>().canReload=false;
            weapons[i].GetComponent<Gun>().reloadIconTimer = 0;
            weapons[i].GetComponent<Gun>().UIReloadIcon.fillAmount = 0;
            weapons[i].SetActive(false);
        }
        weapons[indexActive].GetComponent<Gun>().canReload = true;
        weapons[indexActive].SetActive(true);
        
        weapons[indexActive].GetComponent<Gun>().UIReloadIcon.fillAmount= weapons[indexActive].GetComponent<Gun>().reloadIconTimer / weapons[indexActive].GetComponent<Gun>().reloadTime;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            ChangeWeapons(0);
            ChangeWeapons(0);

        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            ChangeWeapons(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            if (boughtweapon_GreenFlare)
            {
                ChangeWeapons(2);
            }
        }

         OpenMenu();

        if (health.currentHealth <= 0)
        {
            deathPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            ActivateShop();
        }

        if (killedEnemies == levelEnemies)
        {
            bossTeleport.GetComponent<Collider>().enabled = true;
        }

        UILevelEnemies.text = killedEnemies + "/" + levelEnemies;
        BossUnlockCondition();
    }

    private void OpenMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && UIMenuPanel.activeSelf == false)
        {
            Time.timeScale = 0;
            Debug.Log("prvi");
            UIMenuPanel.SetActive(true);
            health.gameObject.GetComponent<Movement>().enabled = false;
            guncontroller.enabled = false;
            Camera.main.GetComponent<CameraLook>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

        }
        else if (Input.GetKeyDown(KeyCode.Escape) && UIMenuPanel.activeSelf == true)
        {
            Time.timeScale = 1;
            Debug.Log("drugi");
            UIMenuPanel.SetActive(false);
            health.gameObject.GetComponent<Movement>().enabled = true;
            guncontroller.enabled = true;
            Camera.main.GetComponent<CameraLook>().enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ScoreUpdate(int scoreUpdate)
    {
        score += scoreUpdate;
        UIScorePoints.text=score.ToString()+" $";

    }

    public void ActivateShop()
    {
        Cursor.visible = !Cursor.visible;
        guncontroller.canShoot = !guncontroller.canShoot;
        UIShopPanel.SetActive(!UIShopPanel.activeSelf);


        if (UIShopPanel.activeSelf && health.isAlive)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (!UIShopPanel.activeSelf && health.isAlive)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ActivateMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && UIMenuPanel.activeSelf==false)
        {
            Debug.Log("prvi");
            UIMenuPanel.SetActive(true);
            health.gameObject.GetComponent<Movement>().enabled = false;
            guncontroller.enabled = false;
            Camera.main.GetComponent<CameraLook>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            //Time.timeScale = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && UIMenuPanel.activeSelf == true)
        {
            Debug.Log("drugi");
            UIMenuPanel.SetActive(false);
            health.gameObject.GetComponent<Movement>().enabled = true;
            guncontroller.enabled = true;
            Camera.main.GetComponent<CameraLook>().enabled = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            // Time.timeScale = 1;
        }
    }

    public void Die()
    {
        if (health.lives <= 0)
        {
            health.isAlive = false;
            health.gameObject.GetComponent<Movement>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("You Died!!!");
        }
        else
        {
            health.lives--;

            health.currentHealth = health.maxHealth;
            health.healtpoints.fillAmount = health.currentHealth / health.maxHealth;

            health.transform.position = respawnPosition.position;
        }
    }

}

