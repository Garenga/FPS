using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public int maxBulletAmount;
    public int currentBulletAmount;

    public float reloadTime;
    public float reloadIconTimer;

    public Transform startingPosition;
    public Transform aimPosition;

    public float aimFOV = 35f;
    public float normalFOV = 60f;

   public float fireRate = 0.1f;
   public bool canShoot = true;

    public TextMeshProUGUI UIBulletsAmount;
    public Image UIReloadIcon;

    public Transform shootPoint;
    public GameObject bullet;

    public bool isAiming;
    public float aimBloom;

   public bool canReload = true;

    void Shoot()
    {
        float x = Screen.width / 2;
        float y = Screen.height / 2;

        float xAcc = Random.Range(x - aimBloom, x + aimBloom);
        float yAcc = Random.Range(y - aimBloom, y + aimBloom);

        var ray = Camera.main.ScreenPointToRay(new Vector3(xAcc, yAcc, 0));

        GameObject bulletShoot = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
        Rigidbody bulletRB = bulletShoot.GetComponent<Rigidbody>();

        bulletRB.velocity = bulletShoot.GetComponent<Bullet>().speed * ray.direction;

        currentBulletAmount--;
    }

    private void Start()
    {
        currentBulletAmount = maxBulletAmount;
        Camera.main.fieldOfView = normalFOV;
    }

    private void Update()
    {
        UiReload();
        if (Input.GetMouseButton(0))
        {
            if (canShoot && currentBulletAmount > 0)
            {
                Shoot();
                canShoot = false;
                Invoke("ResetBool", fireRate);

            }
            else if (currentBulletAmount == 0)
            {
                canShoot = false;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            gameObject.transform.position = aimPosition.position;
            Camera.main.fieldOfView = aimFOV;

            Debug.Log(shootPoint.position);
        }
        if (Input.GetMouseButtonUp(1))
        {
            gameObject.transform.position = startingPosition.position;
            Camera.main.fieldOfView = normalFOV;

            Debug.Log(shootPoint.position);

        }

        if (Input.GetKeyDown(KeyCode.R) && canReload && currentBulletAmount<maxBulletAmount)
        {
            canShoot = false;
            canReload = false;
            reloadIconTimer = reloadTime;
            StartCoroutine(ReloadBullets());
        }

        else if (currentBulletAmount == 0 && canReload)
        {
            canShoot = false;
            reloadIconTimer = reloadTime;
            StartCoroutine(ReloadBullets());
        }

        UIBulletsAmount.text = string.Format("{0}/{1}", currentBulletAmount, maxBulletAmount);

    }

    private void UiReload()
    {
        if ((!canReload && currentBulletAmount < maxBulletAmount) || (currentBulletAmount==0))
        {
            UIReloadIcon.fillAmount = reloadIconTimer / reloadTime;
            reloadIconTimer -= Time.deltaTime;
        }
    }

    void ResetBool()
    {
        canShoot = true;
    }

    IEnumerator ReloadBullets()
    {
        canReload = false;//janky, treba popravit
        UiReload();
        yield return new WaitForSeconds(reloadTime);

        currentBulletAmount = maxBulletAmount;
        canReload = true;
        canShoot = true;
        reloadIconTimer = 0;
    }

}

