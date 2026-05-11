using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using BulletFury;
using BulletFury.Data;

public class HealthBar : MonoBehaviour, IBulletHitHandler
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float maxHealth = 100f;
    public float health;
    public float lerpSpeed = 0.01f;


    public GameObject phase1BIG;
    public GameObject phase1SML;

    public GameObject phase2BIG;
    public GameObject phase2SML;

    public GameObject phase3BIG;
    public GameObject phase3SML;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
       if (healthSlider.value != health)
        {
            healthSlider.value = health;
        } 

       if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            takeDamage(10);
        }


       if (healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health,lerpSpeed);
        }

        CheckPhase();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 7)
        {
            takeDamage(10);
            Debug.Log("Hit by big player bullet");
        }
    }

    void CheckPhase()
    {
        if (health <= 200 && health > 100)
        {
            phase1BIG.SetActive(false);
            phase1SML.SetActive(false);
            phase2BIG.SetActive(true);
            phase2SML.SetActive(true);
            //Debug.Log("Start phase 2");
        } else if (health <= 100 && health > 0)
        {
            phase2BIG.SetActive(false);
            phase2SML.SetActive(false);
            //Debug.Log("Start phase 3");
        } else if (health <= 0)
        {
            //Debug.Log("Dead 3");

        }
    }

    void takeDamage(float damage)
    {
        health -= damage;
    }

    public void Hit(BulletContainer bullet)
    {
        health -= bullet.Damage;
    }
}
