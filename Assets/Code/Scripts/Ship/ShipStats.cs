using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that handles modification and adjustment of ship stats
public class ShipStats : MonoBehaviour
{
    public string name;

    // Base stats for the ship. These are stats without ANY modifications. These get modified by various modules in the game.
    public int baseHealth;
    public int baseArmor;
    public int baseHull;
    public int baseCargo;
    
    // Movement stats for the ship. These stats change how the ship moves and operates.
    public int rotationSpeed;
    public int thrust;
}
