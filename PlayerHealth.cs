using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;


public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public ParticleSystem bloodSplatter;
    public bool isAlive = true;
    public PostProcessVolume volume;
    public Vignette vignette;
    public ChromaticAberration cAberration;
    public float vValue;

    public Image healtpoints;
    public int lives;
    private float someTime=0.5f;

    private void Start()
    {

        currentHealth = maxHealth;
        healtpoints.fillAmount = maxHealth / 100;
        volume.profile.TryGetSettings(out vignette);
        volume.profile.TryGetSettings(out cAberration);
    }
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        healtpoints.fillAmount = currentHealth / 100;
        bloodSplatter.Play();

        if (currentHealth <= 0)
        {
            GameManager.instance.Die();
        }
    }

    public void HealDamage(int heal)
    {
        currentHealth += heal;
        healtpoints.fillAmount = currentHealth / 100;
    }

    private void Update()
    {
        if(currentHealth < maxHealth*0.25f)
        {
            vignette.intensity.value = Mathf.PingPong(Time.time*.5f, 0.5f);
            cAberration.intensity.value = Mathf.PingPong(Time.time*.5f, 0.5f);
            Debug.Log(vignette.intensity.value);
        }
        else
        {
            vignette.intensity.value = 0f;
            cAberration.intensity.value = 0f;
        }

    }

} 


