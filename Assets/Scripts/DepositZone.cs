using UnityEngine;

public sealed class DepositZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ChamberGameManager.Instance.TryDeposit(other);
    }
}
