using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioPlayer))]
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private AxisDirection whereIsRight = AxisDirection.PositiveX;
    private Vector3 rightDirection;
    [SerializeField] private AxisDirection whereIsUp = AxisDirection.PositiveZ;
    private Vector3 upDirection;

    [SerializeField] private float horizontalMoveSpeed = 1.0f;
    [SerializeField] private float verticalMoveSpeed = 1.0f;

    [SerializeField] private AnimationCurve dodgerollSpeedCurve;
    [SerializeField] private float dodgerollDuration = 1.0f;

    [SerializeField] private SpriteRenderer playerBillboard;

    private PlayerInputs.PlayerActions actions;
    private Vector2 movementInput = Vector2.zero;
    private Vector3 realMovement = Vector3.zero;

    private bool primaryMovementEnabled = true;

    private AudioPlayer audioPlayer;
    private const string dodgerollAudioName = "roll";
    private const string walkAudioName = "footstep";
    private const float timeBetweenFootstepSounds = 0.3f;
    private bool walkSoundActive = false;

    private void Start()
    {
        actions = PlayerInputController.Instance.inputActions.Player;
        rightDirection = GetVector3FromEnum(whereIsRight);
        upDirection = GetVector3FromEnum(whereIsUp);

        audioPlayer = GetComponent<AudioPlayer>();
    }

    private void Update()
    {
        bool dodgerollPressed = actions.Dodgeroll.WasPressedThisFrame();
        if (dodgerollPressed && primaryMovementEnabled)
        {
            StartCoroutine(DodgerollCoroutine());
        }
    }

    private void FixedUpdate()
    {
        movementInput = actions.Move.ReadValue<Vector2>();
        realMovement = Vector3.zero;
        realMovement += rightDirection * movementInput.x * horizontalMoveSpeed;
        realMovement += upDirection * movementInput.y * verticalMoveSpeed;
        if (primaryMovementEnabled)
        {
            gameObject.transform.position += realMovement * Time.fixedDeltaTime;
            if(realMovement.magnitude > 0.01f && !walkSoundActive)
            {
                StartCoroutine(FootstepCoroutine());
            }
        }
    }

    private Vector3 GetVector3FromEnum(AxisDirection direction)
    {
        switch (direction)
        {
            case AxisDirection.PositiveX:
                return Vector3.right;
            case AxisDirection.NegativeX:
                return Vector3.left;
            case AxisDirection.PositiveZ:
                return Vector3.forward;
            case AxisDirection.NegativeZ:
                return Vector3.back;
            case AxisDirection.PositiveY:
                return Vector3.up;
            case AxisDirection.NegativeY:
                return Vector3.down;
            default:
                return Vector3.zero;
        }
    }

    private IEnumerator DodgerollCoroutine()
    {
        primaryMovementEnabled = false;
        playerBillboard.transform.localScale = new Vector3(1, 0.5f);
        float timeSinceStart = 0;
        float currentSpeed = 0;
        audioPlayer.PlaySound(dodgerollAudioName);
        while (timeSinceStart < dodgerollDuration)
        {
            currentSpeed = dodgerollSpeedCurve.Evaluate(timeSinceStart / dodgerollDuration);
            yield return new WaitForFixedUpdate();
            gameObject.transform.position += realMovement * currentSpeed * Time.fixedDeltaTime;
            timeSinceStart += Time.fixedDeltaTime;
        }
        primaryMovementEnabled = true;
        playerBillboard.transform.localScale = new Vector3(1, 1);
    }

    private IEnumerator FootstepCoroutine()
    {
        walkSoundActive = true;
        audioPlayer.PlaySound(walkAudioName);
        yield return new WaitForSeconds(timeBetweenFootstepSounds);
        walkSoundActive = false;
    }
    public enum AxisDirection
    {
        PositiveX,
        NegativeX,
        PositiveZ,
        NegativeZ,
        PositiveY,
        NegativeY
    }
}
