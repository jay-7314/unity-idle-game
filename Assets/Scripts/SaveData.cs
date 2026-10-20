using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public double gold;
    public List<string> upgradeKeys = new List<string>();
    public List<int> upgradeLevels = new List<int>();
    public long lastSavedUnix;
}