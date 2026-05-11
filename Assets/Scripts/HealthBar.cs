using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using BulletFury;
using BulletFury.Data;
using UnityEngine.SceneManagement;

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
    public GameObject phse3SML2;

  //  public GameObject BG1;
  //  public GameObject BG2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
     //   BG1.SetActive(true);
     //   BG2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
       if (healthSlider.value != health)
        {
            healthSlider.value = health;
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

           

            phase3BIG.SetActive(true);
            phase3SML.SetActive(true);
            phse3SML2.SetActive(true);
            //Debug.Log("Start phase 2");
        } else if (health <= 100 && health > 0)
        {
            //  BG1.SetActive(false);
            //  BG2.SetActive(true);

            phase3BIG.SetActive(false);
            phase3SML.SetActive(false);
            phse3SML2.SetActive(false);


            phase2BIG.SetActive(true);
            phase2SML.SetActive(true);
            //Debug.Log("Start phase 3");
        } else if (health <= 0)
        {
            SceneManager.LoadScene(3);
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
