using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : ObservableObject
{
    [Header("Movement Settings")]
    [SerializeField] private List<Transform> positionsToMoveTo = new();
    [SerializeField] private MovementType movementType = MovementType.Smooth;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private float minimumMoveDistance = 1f;
    
    [Header("Movement Audio")]
    [SerializeField] private AudioClip[] movementSounds;
    [SerializeField] private AudioClip[] teleportSounds;
    
    [Header("Effects")]
    [SerializeField] private ParticleSystem moveEffect;
    
    public enum MovementType { Instant, Smooth, Teleport }
    
    private Vector3 startPosition;
    private bool isMoving = false;
    
    protected override void OnStart()
    {
        startPosition = transform.position;
    }
    
    protected override void OnBecameObserved()
    {
        // Stop movement or play detection sound
        LogAction("stopped moving - being watched");
    }
    
    protected override void OnLeftObservation()
    {
        if (!isMoving)
            MoveToNewPosition();
    }
    
    private void MoveToNewPosition()
    {
        Vector3 newPosition = SelectValidPosition();
        if (newPosition == Vector3.zero) return;
        
        switch (movementType)
        {
            case MovementType.Instant:
                MoveInstantly(newPosition);
                break;
            case MovementType.Smooth:
                StartCoroutine(MoveSmoothly(newPosition));
                break;
            case MovementType.Teleport:
                StartCoroutine(TeleportMovement(newPosition));
                break;
        }
    }
    
    private Vector3 SelectValidPosition()
    {
        if (positionsToMoveTo.Count == 0) return Vector3.zero;
        
        List<Transform> validPositions = new List<Transform>();
        foreach (Transform pos in positionsToMoveTo)
        {
            if (pos != null && Vector3.Distance(transform.position, pos.position) >= minimumMoveDistance)
                validPositions.Add(pos);
        }
        
        if (validPositions.Count == 0) return Vector3.zero;
        return validPositions[Random.Range(0, validPositions.Count)].position;
    }
    
    private void MoveInstantly(Vector3 newPosition)
    {
        transform.position = newPosition;
        PlayRandomAudioClip(movementSounds);
        TriggerEffects();
    }
    
    private IEnumerator MoveSmoothly(Vector3 targetPosition)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, targetPosition);
        float duration = distance / movementSpeed;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPos, targetPosition, movementCurve.Evaluate(t));
            yield return null;
        }
        
        transform.position = targetPosition;
        isMoving = false;
        PlayRandomAudioClip(movementSounds);
        TriggerEffects();
    }
    
    private IEnumerator TeleportMovement(Vector3 targetPosition)
    {
        isMoving = true;
        PlayRandomAudioClip(teleportSounds);
        TriggerEffects();
        yield return new WaitForSeconds(0.2f);
        transform.position = targetPosition;
        yield return new WaitForSeconds(0.1f);
        isMoving = false;
    }
    
    private void TriggerEffects()
    {
        if (moveEffect != null)
            moveEffect.Play();
    }
}
