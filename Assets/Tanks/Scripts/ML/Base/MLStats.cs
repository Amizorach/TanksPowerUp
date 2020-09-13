using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

[System.Serializable]
public class MLStats
{
    public int powerUps = 0;
    public float totalDistance = 0;
    public float totalDamage;
    public float shots;
    public float miss;
    public float hit;

    public StatsRecorder recorder;

    public MLStats(StatsRecorder rec)
    {
        recorder = rec;
    }

    internal void Send()
    {
        recorder.Add("driver/PowerUps", powerUps, StatAggregationMethod.Average);
        recorder.Add("driver/TotalDistance", totalDistance, StatAggregationMethod.Average);
        if (shots > 0)
        {
            recorder.Add("shooter/Shots", shots, StatAggregationMethod.Average);
            recorder.Add("shooter/Miss", miss, StatAggregationMethod.Average);
            recorder.Add("shooter/hit", hit, StatAggregationMethod.Average);

            recorder.Add("shooter/hitPct", hit / shots, StatAggregationMethod.Average);


            recorder.Add("shooter/totalDamage", totalDamage, StatAggregationMethod.Average);
        }


    }
}
