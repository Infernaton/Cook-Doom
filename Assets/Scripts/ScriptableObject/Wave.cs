using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Wave/New Wave")]
public class Wave : ScriptableObject
{
    public List<GameObject> MobList;
    public float WaveDuration;
    public float SpawnRate;
}
