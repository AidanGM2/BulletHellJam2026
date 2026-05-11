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

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 7)
        {
            takeDamage(10);
            Debug.Log("Hit by big player bullet");
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
