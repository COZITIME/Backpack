using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerArtHandler))]
public class PlayerInputHandler : MonoBehaviour
{
    [FormerlySerializedAs("player")]
    [SerializeField] private PlayerTransform playerTransform;

    private PlayerInputActions _inputActions;

    private FaceDirection _forwards = FaceDirection.Down;
    private Vector2Int _faceInput;

    private Coroutine _releaseCoroutine;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

        _inputActions.Gameplay.Move.performed += OnMove;
        _inputActions.Gameplay.Move.canceled += OnFaceCanceled;
        _inputActions.Gameplay.Wait.canceled += OnWait;
    }

    private void OnDestroy()
    {
        _inputActions.Disable();
        _inputActions.Dispose();
    }

    // -------- Facing --------
    private void OnMove(InputAction.CallbackContext btn)
    {
        var input = Vector2Int.RoundToInt(btn.ReadValue<Vector2>());


        if (_faceInput == Vector2Int.zero ||
            input.magnitude > _faceInput.magnitude) // gone from nothing or gotten bigger
        {
            ApplyFacing(input);
        }
        else if (_faceInput != input) // we have released something
        {
            if (_releaseCoroutine != null)
            {
                StopCoroutine(_releaseCoroutine);
            }

            _releaseCoroutine = this.ExecuteDelayedRealtime(0.05f, () => { ApplyFacing(input); });
        }
        else
        {
            ApplyFacing(input);
        }
    }

    private void OnFaceCanceled(InputAction.CallbackContext ctx)
    {
        if (_releaseCoroutine != null)
        {
            StopCoroutine(_releaseCoroutine);
        }

        playerTransform.TryTranslate(_forwards.FaceDirectionToDirection());
        _faceInput = Vector2Int.zero;
    }

    private void ApplyFacing(Vector2Int dir)
    {
        if (dir != Vector2Int.zero) _forwards = dir.DirectionToFaceDirection();
        _faceInput = dir;
        playerTransform.FaceInDirection(_forwards);
    }

    // -------- Stepping --------
    private void OnWait(InputAction.CallbackContext ctx)
    {
        PlayerTransform.Instance.DoNothingTurn();
    }
}