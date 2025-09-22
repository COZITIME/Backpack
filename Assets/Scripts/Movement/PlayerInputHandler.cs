using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(DirectionalArtHandler))]
public class PlayerInputHandler : MonoBehaviour
{
    [FormerlySerializedAs("player")]
    [SerializeField] private PlayerTransform playerTransform;

    private PlayerInputActions _inputActions;

    private Coroutine _inputCoroutine;
    public static PlayerInputHandler Instance { get; private set; }

    private Vector2 _inputSum;
    private bool _stopMoveInput = false;

    private void Awake()
    {
        Instance = this;
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

        _inputActions.Gameplay.Move.performed += OnMove;
        _inputActions.Gameplay.Wait.performed += OnWait;
        _inputActions.Gameplay.Move.canceled += _ => { _stopMoveInput = false; };
    }


    private void OnDestroy()
    {
        _inputActions?.Disable();
        _inputActions?.Dispose();
    }

    private void OnMove(InputAction.CallbackContext btn)
    {
        if (_stopMoveInput) return;

        if (_inputCoroutine == null)
        {
            StartCoroutine(OnMoveDelayedInputCoroutine());
            _inputSum = Vector2.zero;
        }

        _inputSum += Vector2Int.RoundToInt(btn.ReadValue<Vector2>());
    }

    private IEnumerator OnMoveDelayedInputCoroutine()
    {
        yield return new WaitForSecondsRealtime(.05f);
        _stopMoveInput = true;
        if (_inputSum != Vector2.zero)
        {
            var intSum = Vector2Int.RoundToInt(_inputSum);
            if (intSum != Vector2Int.zero)
            {
                var dir = intSum.DirectionToFaceDirection(true);
                OnButtonClicked(dir);
            }
        }
    }

    private void OnWait(InputAction.CallbackContext ctx)
    {
        OnButtonClicked(null);
    }

    public void OnButtonClicked(FaceDirection? direction)
    {
        _inputSum = Vector2.zero;

        if (direction.HasValue)
        {
            var faceVector = direction.Value;
            var faceDirection = faceVector.FaceDirectionToDirection();
            playerTransform.FaceInDirection(faceVector);
            playerTransform.TryTranslate(faceDirection);
        }
        else
        {
            PlayerTransform.Instance.DoNothingTurn();
        }
    }
}