using System.Collections;
using BulletFury;
using BulletFury.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BulletFury.Samples
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class DemoHitFlash : MonoBehaviour, IBulletHitHandler
    {
        [SerializeField] private Color flashColor = Color.red;
        [SerializeField, Min(0.01f)] private float flashDuration = 0.08f;

        private SpriteRenderer _spriteRenderer;
        private Color _baseColor;
        private Coroutine _flashRoutine;

       

        public Slider healthSlider;
        public Slider easeHealthSlider;
        public float maxHealth = 10f;
        public float health;
        public float lerpSpeed = 0.01f;

        private void Awake()
        {
            health = maxHealth;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _baseColor = _spriteRenderer.color;
        }


        private void Update()
        {
            if (healthSlider.value != health)
            {
                healthSlider.value = health;
            }


            if (healthSlider.value != easeHealthSlider.value)
            {
                easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, lerpSpeed);
            }

            if (health <= 0)
            {
                SceneManager.LoadScene(2);
            }

        }

      

        public void Hit(BulletContainer bullet)
        {
            if (!isActiveAndEnabled || _spriteRenderer == null)
                return;

            if (_flashRoutine != null)
                StopCoroutine(_flashRoutine);

            _flashRoutine = StartCoroutine(FlashRoutine());

            health -= 1;

        }

        private IEnumerator FlashRoutine()
        {
            _spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            _spriteRenderer.color = _baseColor;
            _flashRoutine = null;
        }

        private void OnDisable()
        {
            if (_spriteRenderer != null)
                _spriteRenderer.color = _baseColor;

            _flashRoutine = null;
        }
    }
}
