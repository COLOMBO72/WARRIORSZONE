using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float health;
    public float currentHealth;
    public float lifes;

    private Animator anim;

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Slider healthSlider;

    Vector3 spawnPos;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        health = maxHealth;
        lifes = 5f;
        healthSlider.maxValue = maxHealth;
    }

    void Update()
    {
        if (health < currentHealth)
        {
            currentHealth = health;
            anim.SetTrigger("Attacked");
            if (health <= 0 && currentHealth <= 0)
            {
                KillPlayer();
                //start method with timeout 5sec
                Invoke(nameof(Respawn), 5f);
                if (lifes <= 0)
                {
                    Debug.Log("Lifes 0, enemy dead forever.");
                    healthSlider.value = 0f;
                    Destroy(gameObject);
                }
            }
        }
    }

    void Respawn()
    {
        gameObject.SetActive(true);
        transform.position = new Vector3(0, 7);
        transform.position += new Vector3(0, 7);
        health = maxHealth;
        currentHealth = maxHealth;
    }


    void KillPlayer()
    {
        gameObject.SetActive(false);
        health = 0;
        lifes--;
        Debug.Log($"{gameObject.name} dead. He have only {lifes} lifes.");
    }

    private void OnGUI()
    {
        //float t = Time.deltaTime / 1f;
        // SMOOTH ANIM
        //healthSlider.value = Mathf.Lerp(healthSlider.value,health,t);

        //SHARP ANIM
        healthSlider.value = health;
    }
}
