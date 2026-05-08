using BulletFury;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using DG.Tweening;
#endif

namespace BulletFury.Samples
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private BulletSpawner spawner;
        [SerializeField, Min(0f)] private float moveSpeed = 6f;
        [SerializeField] private float shrinkSize;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera zoomCamera;
        [SerializeField] private ZoomCameraScript zCS;
        [SerializeField] private float tweenSpeed;

        private Animator playerAnimator;
        private SpriteRenderer playerSprite;
        private Rigidbody2D _rigidbody2D;
        private Vector2 _moveInput;
        private Bounds playerBounds;
        private Vector3 targetPosition;
        private bool cooldowntimer = true;
        private bool falsepress = false;

#if ENABLE_INPUT_SYSTEM
        private InputAction _moveAction;
        private InputAction _fireAction;
        private InputAction _focusAction;
#endif

        private void Awake()
        {
            playerAnimator = GetComponent<Animator>();
            playerSprite = GetComponent<SpriteRenderer>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _rigidbody2D.gravityScale = 0f;
            _rigidbody2D.freezeRotation = true;
            zCS.tweenspeed = tweenSpeed;
        }

        private void Start()
        {
            spawner?.Stop();
            ResizePlayer();
        }

        private void FixedUpdate()
        {
            var delta = _moveInput * (moveSpeed * Time.fixedDeltaTime);
            if (_moveInput.x < 0) ChangeLean(-1);
            else if (_moveInput.x == 0) ChangeLean(0);
            else if (_moveInput.x > 0) ChangeLean(1);
            if (delta.x > 0 && _rigidbody2D.position.x + delta.x > playerBounds.max.x)
            {
                delta.x = 0;
                _rigidbody2D.position = new Vector2(playerBounds.max.x, _rigidbody2D.position.y);
            }
            if (delta.x < 0 && _rigidbody2D.position.x + delta.x < playerBounds.min.x)
            {
                delta.x = 0;
                _rigidbody2D.position = new Vector2(playerBounds.min.x, _rigidbody2D.position.y);
            }
            if (delta.y > 0 && _rigidbody2D.position.y + delta.y > playerBounds.max.y)
            {
                delta.y = 0;
                _rigidbody2D.position = new Vector2(_rigidbody2D.position.x, playerBounds.max.y);
            }
            if (delta.y < 0 && _rigidbody2D.position.y + delta.y < playerBounds.min.y)
            {
                delta.y = 0;
                _rigidbody2D.position = new Vector2(_rigidbody2D.position.x, playerBounds.min.y);
            }
            //if (_rigidbody2D.position.x - delta.x < playerBounds.min.x || _rigidbody2D.position.x + delta.x > playerBounds.max.x) delta.x = 0;
            //if (_rigidbody2D.position.y - delta.y < playerBounds.min.y || _rigidbody2D.position.y + delta.y > playerBounds.max.y) delta.x = 0;
            _rigidbody2D.MovePosition(_rigidbody2D.position + delta);
        }

        private void ResizePlayer()
        {
            float height = GetComponent<SpriteRenderer>().size.x / 2f * transform.localScale.x;
            float width = GetComponent<SpriteRenderer>().size.y / 2f * transform.localScale.y;

            float minX = (Globals.WorldBounds.min.x + width);
            float maxX = (Globals.WorldBounds.extents.x - width);

            float minY = Globals.WorldBounds.min.y + height;
            float maxY = Globals.WorldBounds.max.y - height;

            playerBounds = new Bounds();
            playerBounds.SetMinMax(
                new Vector2(minX, minY),
                new Vector2(maxX, maxY)
                );
        }

        private Vector3 GetPlayerBounds()
        {
            return new Vector3(
                Mathf.Clamp(targetPosition.x, playerBounds.min.x, playerBounds.max.x),
                Mathf.Clamp(targetPosition.y, playerBounds.min.y, playerBounds.max.y),
                transform.position.z
                );
        }

        private void ChangeLean(float Direction)
        {
            switch (Direction){
                case -1:
                    playerAnimator.SetTrigger("LeanTrigger");
                    playerSprite.flipX = true;
                    break;
                case 0:
                    playerAnimator.SetTrigger("ForwardTrigger");
                    playerSprite.flipX = false;
                    break;
                case 1:
                    playerAnimator.SetTrigger("LeanTrigger");
                    break;
            }
        }

        private void cooldown()
        {
            cooldowntimer = true;
        }

#if ENABLE_INPUT_SYSTEM
        private void OnEnable()
        {
            if (_moveAction == null)
            {
                _moveAction = new InputAction("Move", expectedControlType: "Vector2");
                _moveAction.AddCompositeBinding("2DVector")
                    .With("Up", "<Keyboard>/upArrow")
                    .With("Down", "<Keyboard>/downArrow")
                    .With("Left", "<Keyboard>/leftArrow")
                    .With("Right", "<Keyboard>/rightArrow");
                _moveAction.performed += OnMovePerformed;
                _moveAction.canceled += OnMoveCanceled;

                _fireAction = new InputAction("Fire", InputActionType.Button, "<Keyboard>/x");
                _fireAction.started += OnFireStarted;
                _fireAction.canceled += OnFireCanceled;

                _focusAction = new InputAction("Focus", InputActionType.Button, "<Keyboard>/leftShift");
                _focusAction.started += OnFocusStarted;
                _focusAction.canceled += OnFocusCanceled;
            }

            _moveAction.Enable();
            _fireAction.Enable();
            _focusAction.Enable();
        }

        private void OnDisable()
        {
            _moveAction?.Disable();
            _fireAction?.Disable();
            _moveInput = Vector2.zero;
            spawner?.Stop();
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext _)
        {
            _moveInput = Vector2.zero;
        }

        private void OnFireStarted(InputAction.CallbackContext _)
        {
            spawner?.Play();
        }

        private void OnFireCanceled(InputAction.CallbackContext _)
        {
            spawner?.Stop();
        }

        private void OnFocusStarted(InputAction.CallbackContext _)
        {
            if (cooldowntimer == true)
            {
                cooldowntimer = false;
                moveSpeed = moveSpeed / 4;
                DOTween.To(() => transform.localScale, x => transform.localScale = x, new Vector3(shrinkSize, shrinkSize, shrinkSize), tweenSpeed);
                ResizePlayer();
                zCS.shrink();
            }
            else falsepress = true;
        }

        private void OnFocusCanceled(InputAction.CallbackContext _)
        {
            if (falsepress == false)
            {
                moveSpeed = moveSpeed * 4;
                DOTween.To(() => transform.localScale, x => transform.localScale = x, new Vector3(1f, 1f, 1f), tweenSpeed);
                ResizePlayer();
                Invoke("cooldown", 1f);
                if (_rigidbody2D.position.x > playerBounds.max.x) _rigidbody2D.position = new Vector2(playerBounds.max.x, _rigidbody2D.position.y);
                if (_rigidbody2D.position.x < playerBounds.min.x) _rigidbody2D.position = new Vector2(playerBounds.min.x, _rigidbody2D.position.y);
                if (_rigidbody2D.position.y > playerBounds.max.y) _rigidbody2D.position = new Vector2(_rigidbody2D.position.x, playerBounds.max.y);
                if (_rigidbody2D.position.y < playerBounds.min.y) _rigidbody2D.position = new Vector2(_rigidbody2D.position.x, playerBounds.min.y);
                zCS.grow();
            }
            else falsepress = false;
        }
#else
        private void OnEnable()
        {
            Debug.LogWarning("DemoPlayerController requires the Input System package.", this);
        }
#endif
    }
}
