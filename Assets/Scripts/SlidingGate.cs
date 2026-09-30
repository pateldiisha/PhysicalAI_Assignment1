using UnityEngine;

public sealed class SlidingGate : MonoBehaviour
{
    [SerializeField] private float liftHeight = 3.2f;
    [SerializeField] private float liftSpeed = 2.8f;
    private Vector3 closedPosition;

    private void Start() => closedPosition = transform.position;

    private void Update()
    {
        if (ChamberGameManager.Instance != null && ChamberGameManager.Instance.GateUnlocked)
        {
            Vector3 target = closedPosition + Vector3.up * liftHeight;
            transform.position = Vector3.MoveTowards(transform.position, target, liftSpeed * Time.deltaTime);
        }
    }
}
