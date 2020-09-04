using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLSettings : MonoBehaviour
{
    public List<TLevel> levels;
}

[System.Serializable]
public struct TLevel
{
    public int level;
    public AreaSettings areaSettings;
    public TankSettings tankSettings;
    public TankDriverRewards driverRewards;
}
