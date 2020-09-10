using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

[RequireComponent(typeof(MLSettings))]

public class TanksAreaBase : MonoBehaviour
{
    internal AreaSettings areaSettings;
    private List<AreaObjectInfo> obstructs;
    private List<AreaObjectInfo> powerUps;

    [HideInInspector]
    public List<AreaObjectInfo> agents;

    //GroundBase for resizing the field
    public GameObject baseGround;

    ////Info regarding the objects to be placed in the scene
    //public AreaPrefabInfo[] obstuctPrefabs;
    //public AreaPrefabInfo[] powerUpPrefabs;
    //public AreaPrefabInfo[] agentPrefabs;

 
    //how often to Reset the obstructions
    public float obstructionTimeOut = 100f;

    //Just to make the GameObject orgenized
    private GameObject powerUpHolder;
    private GameObject obstructionHolder;
    private GameObject agentsHolder;

    private MLSettings levels;
    public CameraControl cameraControl;

    public int currentLevel;
    public void Awake()
    {
        levels = GetComponent<MLSettings>();
        SetupGameObject();
        obstructs = new List<AreaObjectInfo>();
        powerUps = new List<AreaObjectInfo>();
        agents = new List<AreaObjectInfo>();

        SetLevel(currentLevel);

        InvokeRepeating("ResetObstructions", obstructionTimeOut, obstructionTimeOut);
       // Invoke("TestLevel", 60f);

    }
    private void TestLevel()
    {
        SetLevel(1);
    }
    private void SetLevel(int level)
    {
        if (level < 0 || level >= levels.levels.Count)
            return;
        currentLevel = level;
        areaSettings = levels.levels[currentLevel].areaSettings;
     
        SetupField();
        ResetField();

    }

    public void ResetObstructions()
    {
        float l = Academy.Instance.EnvironmentParameters.GetWithDefault("level", 0f);
        Debug.Log(l);
       
        if (l != currentLevel)
        {
            SetLevel((int)l);
            return;
        }
        ResetAreaList(obstructs, 0f, true);
    }

    internal MLRewards GetRewards()
    {
        return levels.levels[currentLevel].rewards;
    }

    public void ResetPowerUp(GameObject go)
    {
        AreaObjectInfo aoi = powerUps.Find((pu) => pu.go == go);
        aoi.go.transform.position = FindEmptySpace(aoi.radius, 1f);
    }

    internal TankSettings GetTankSettings()
    {
        return levels.levels[currentLevel].tankSettings;
    }

    internal void ResetAgent(GameObject tank)
    {
       AreaObjectInfo aoi = agents.Find((ag) => ag.go == tank);
       tank.transform.position = FindEmptySpace(aoi.radius, 0f);
    }

    private void SetupGameObject()
    {
        powerUpHolder = new GameObject("PowerUps");
        powerUpHolder.transform.parent = this.transform;
        obstructionHolder = new GameObject("Obstructions");
        obstructionHolder.transform.parent = this.transform;
        agentsHolder = new GameObject("Agents");
        agentsHolder.transform.parent = this.transform;
    }

    private void ResetField()
    {
        //we first relocate the powerups and agents
        //the obstructs will happen in the Reset function
        RelocateAreaList(powerUps);
        RelocateAreaList(agents);

        ResetObstructions();
        ResetAreaList(powerUps, 1f, false) ;
        ResetAreaList(agents, 0f, false);

    }


    private void SetupField()
    {
        baseGround.transform.localScale = Vector3.one * (areaSettings.fieldSize /5f); //Plane length = 10


        SetupAreaList(areaSettings.obstuctPrefabs, obstructs, obstructionHolder.transform, "wall");

        SetupAreaList(areaSettings.powerUpPrefabs, powerUps, powerUpHolder.transform, "energy");

        foreach (AreaObjectInfo aoi in powerUps)
        {
            aoi.go.GetComponent<PowerUp>().SetTriggerRadius(areaSettings.powerUpTriggerRadius);
        }

        SetupAreaList(areaSettings.agentPrefabs, agents, agentsHolder.transform, "agent");

        if (cameraControl != null)
            cameraControl.SetArea(this);
    }

    private void RelocateAreaList(List<AreaObjectInfo> oList)
    {
        foreach (AreaObjectInfo obs in oList)
        {
            obs.go.transform.position = obs.go.transform.position + new Vector3(0f, -1000f, 0f);
        }
    }

    //Repositions all object in the area list
    private void ResetAreaList(List<AreaObjectInfo> oList, float yPos, bool rot)
    {
        //Position all objects out of sight 
        RelocateAreaList(oList);

        //Reposition all objects
        foreach (AreaObjectInfo obs in oList)
        {
            obs.go.transform.position = FindEmptySpace(obs.radius, yPos);
            if (rot)
                obs.go.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
        }
    }

    public void SetupAreaList(AreaPrefabInfo[] pList, List<AreaObjectInfo> oList, Transform parent, string vTag)
    {
        //Clear old list
        foreach (AreaObjectInfo oi in oList)
            Destroy(oi.go);
        oList.Clear();

        for (int i = 0; i < pList.Length; i++)
        {
            
               

                 AreaPrefabInfo opi = pList[i];
            float radius = opi.radius;
                //calculate radius for placement
                if (radius <= 0f)
                {
                    Renderer rend = opi.prefab.GetComponent<Renderer>();
                    if (rend != null)
                        radius = rend.bounds.extents.magnitude;
                    else
                        radius = 1f;
                }
                for (int j = 0; j < opi.count; j++)
                {
                   
                    GameObject go = Instantiate(opi.prefab, parent);
                    if (vTag != null)
                        go.tag = vTag;
                go.name = opi.prefab.name + "(" + j + ")";
                    AreaObjectInfo oi = new AreaObjectInfo(go, radius);
                    oList.Add(oi);
                }
            
        }
    }


    public Vector3 FindEmptySpace(float radius, float yOffset)
    {
        int layerMask = 1 << 8;// 1 << 9;

        layerMask = ~layerMask;
        for (int i = 0; i < 50; i++)
        {
            Vector3 spawnPos = UnityEngine.Random.insideUnitSphere * areaSettings.fieldSize + transform.position;

            spawnPos.y = yOffset;

            if (!Physics.CheckSphere(spawnPos, radius, layerMask))
            {  // Check if area is empty
                return spawnPos;
            }
        }

        return new Vector3(0f, -1000f, 0f) + transform.position;
    }
}
//[System.Serializable]
//public enum YTType  { NONE, FLAG, BUILDING, AGENT };


public struct AreaObjectInfo
{
    public GameObject go;
    public float radius;

    public AreaObjectInfo(GameObject go, float radius)
    {
        this.go = go;
        this.radius = radius;
    }
}
    [System.Serializable]
    public struct AreaPrefabInfo
    {
        public GameObject prefab;
        public int count;
        public float radius;
    }


[System.Serializable]
public class AreaSettings
{
    public float fieldSize = 100f;
    public float powerUpTriggerRadius = 1f;
    //Info regarding the objects to be placed in the scene
    public AreaPrefabInfo[] obstuctPrefabs;
    public AreaPrefabInfo[] powerUpPrefabs;
    public AreaPrefabInfo[] agentPrefabs;
}