using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Create Challenger")]
public class DeathRaceLineup : ScriptableObject
{
    public string playerName;

    [Header("Weapon Stats")]
    public string weaponName;
    public float weaponDamage;
    public float weaponFireRate;
    public float weaponBulletSpeed; 
}
