using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public int healCost=12;
    public int lifeCost;
    public int movementCost;
    public int gunCost;

    public TextMeshProUGUI healCostText;
    public TextMeshProUGUI lifeCostText;
    public TextMeshProUGUI movementCostText;
    public TextMeshProUGUI gunCostText;

    public GameObject[] shopItems;//(NE KORISTI SE)
    [SerializeField] PlayerHealth playerHP;
    [SerializeField] PlayerMovement playerMovement;
    bool spentMoney;


    public bool SpendMoney(int money)
    {
        if (GameManager.instance.score > money)
        {
            GameManager.instance.ScoreUpdate(-money);
             return true;
        }
        else
        {
             return false;
        }
    }
    public void AddHealthPoints(int addedHP)
    {

        if (SpendMoney(healCost))
        {
                if (playerHP.currentHealth < playerHP.maxHealth)
            {
                playerHP.HealDamage(addedHP);
            }
        }
    }

    public void GetItem(int article)
    {
        GameManager.instance.playerItemes.Add(shopItems[article]);
    }


    public void AddLife()
    {

        if (SpendMoney(lifeCost))
        {
            playerHP.lives++;
        }
    }

    public void AddMovmentSpeed(float addedSpeed)//treba popravit
    {

        if (SpendMoney(movementCost))
        {
           
        }
    }

    public void AddRocketJump()
    {

    }


    public void AddNewGun()
    {

        if (SpendMoney(gunCost))
        {
            GameManager.instance.boughtweapon_GreenFlare = true;
        }
    }

    private void Start()
    {
        movementCostText.text = movementCost.ToString() + " $";
        lifeCostText.text = lifeCost.ToString() + " $";
        healCostText.text = healCost.ToString() + " $";
        gunCostText.text=gunCost.ToString() + " $";
    }

}
