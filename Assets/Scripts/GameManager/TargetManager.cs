using UnityEngine;
public enum Target
{
    Player,
    Point
}

public class TargetManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject point;

    public static TargetManager Instance; // A static reference to the TargetManager instance

    void Awake()
    {
        if (Instance == null) // If there is no instance already
        {
            Instance = this;
        }
        else if (Instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
    }

    public GameObject GetGameObject(Target target)
    {
        switch (target)
        {
            case Target.Player:
                return player;
            case Target.Point:
                return point;
            default: return null;
        }
    }
}
