using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Glob Data", fileName = "New Blob Data")]
public class GlobData : ScriptableObject
{
    public int globValue;

    public Color globColor;

    public int shootWeight;    // A weight to determine how likely this is to get when shooting Globz
    public int spawnWeight;    // A separate weight to check when spawning a Glob on the grid
}
