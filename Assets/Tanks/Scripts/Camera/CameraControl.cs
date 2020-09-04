using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 
    public float m_ScreenEdgeBuffer = 4f;           
    public float m_MinSize = 6.5f;
    private float m_ZoomSpeed;
    private Vector3 m_MoveVelocity;
    private Vector3 m_DesiredPosition;


    //[HideInInspector]
    public List<Transform> targets;
    //[HideInInspector]
    public List<Transform> usedTargets;

    private bool isStatic;
    private TanksAreaBase area;
    private Camera cam;
    private Vector3 staticCameraPosition;


    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        //baseCameraRotaion = 
    }

    public void SetArea(TanksAreaBase ytArea)
    {
        area = ytArea;
        foreach (AreaObjectInfo aoi in area.agents)
        {
            targets.Add(aoi.go.transform);

        }

        usedTargets = targets;
    }

    private void FixedUpdate()
    {
        Move();
        Zoom();
    }

    private void Update()
    {
        if (!Input.anyKeyDown)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (isStatic)
                return;
            staticCameraPosition = new Vector3(0f, area.areaSettings.fieldSize, -cam.transform.localPosition.z);

            transform.position = staticCameraPosition;
                Vector3 angels = cam.transform.rotation.eulerAngles;
                angels.x = 90 - angels.x;
                transform.rotation = Quaternion.Euler(new Vector3(90f- cam.transform.rotation.eulerAngles.x, 0f, 0f));
            
                isStatic = true;
            return;
        }


       
       
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            usedTargets = new List<Transform>();
            usedTargets.Add(targets[0]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            usedTargets = new List<Transform>();
            usedTargets.Add(targets[UnityEngine.Random.Range(0, targets.Count)]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            usedTargets = targets;

        } else
        {
            return;
        }
        if (isStatic)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            isStatic = false;

        }

    }

    private void Move()
    {
        if (isStatic)
        {
            return;
        }
        FindAveragePosition();

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        foreach (Transform target in usedTargets)
        {
            if (!target.gameObject.activeSelf)
                continue;

            averagePos += target.transform.position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        foreach (Transform target in usedTargets)
        {
            if (!target.gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(target.position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / cam.aspect);
        }
        
        size += m_ScreenEdgeBuffer;

        size = Mathf.Max(size, m_MinSize);

        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        cam.orthographicSize = FindRequiredSize();
    }
}