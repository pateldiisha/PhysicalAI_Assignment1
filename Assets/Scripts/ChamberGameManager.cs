using UnityEngine;

public sealed class ChamberGameManager : MonoBehaviour
{
    public static ChamberGameManager Instance { get; private set; }

    public bool GateUnlocked { get; private set; }
    public bool IsComplete { get; private set; }
    private float playStartedAt;

    private void Awake()
    {
        Instance = this;
        playStartedAt = Time.time;
    }

    public void UnlockGate()
    {
        if (GateUnlocked) return;
        GateUnlocked = true;
        Debug.Log("SENSOR TRIGGERED: magnetic roadblock removed.");
    }

    public void TryDeposit(Collider other)
    {
        if (IsComplete || other.GetComponentInParent<PayloadMarker>() == null) return;
        if (!GateUnlocked)
        {
            Debug.Log("Deposit ignored: unlock the sensor gate first.");
            return;
        }

        IsComplete = true;
        float elapsed = Time.time - playStartedAt;
        Debug.Log($"CHAMBER COMPLETE: payload deposited in {elapsed:F2} seconds.");
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.UpperLeft,
            fontSize = 15,
            padding = new RectOffset(14, 14, 10, 10)
        };
        style.normal.textColor = Color.white;
        string state = IsComplete ? "DEPOSIT COMPLETE" : GateUnlocked ? "GATE OPEN  •  PUSH THE PAYLOAD TO THE EXIT" : "GATE LOCKED  •  TOUCH THE CYAN SENSOR WITH THE ACTOR";
        GUI.Box(new Rect(16, 16, 470, 72), "WASD  •  Move the blue actor\n" + state, style);
    }
}
