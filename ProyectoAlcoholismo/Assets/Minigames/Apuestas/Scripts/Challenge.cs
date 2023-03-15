using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Challenge : ScriptableObject
{
    public int chId { get; set; }
    public string chText { get; set; }
    public int chLevel { get; set; }
}
