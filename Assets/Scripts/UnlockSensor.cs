using UnityEngine;

public sealed class UnlockSensor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<ActorController>() != null)
            ChamberGameManager.Instance.UnlockGate();
    }
}
