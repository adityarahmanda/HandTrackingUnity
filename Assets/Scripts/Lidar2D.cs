using System.Collections.Generic;
using UnityEngine;

public class Lidar2D : MonoBehaviour 
{
    [SerializeField] private  int resolution = 360;
    [Range(0, 360f), SerializeField] private float fovAngle = 360f;
    [SerializeField] private  float laserMaxDistance = 100f;
    
    [Header("Point Cloud Visual")]
    [SerializeField] private GameObject dotPrefab;
    private MeshRenderer[] _dotMeshRenderers;

    public Vector3[] pointClouds;
    private float _incrementAngle;

    private void Start() 
    {
        pointClouds = new Vector3[resolution];
        _dotMeshRenderers = new MeshRenderer[resolution];
        _incrementAngle = 360.0f / resolution;
    }

    private void FixedUpdate() 
    {
        Vector3 dir;
        RaycastHit hit;
        var startAngle = -fovAngle / 2f;
        var endAngle = fovAngle / 2f;
        
        for (int i = 0; i < resolution; i++)
        {
            var angle = startAngle + i * _incrementAngle;
            if (angle > endAngle)
                break;
            
            dir = transform.rotation * Quaternion.Euler(0, angle, 0) * transform.forward;
            if (Physics.Raycast(transform.position, dir, out hit, laserMaxDistance))
            {
                pointClouds[i] = hit.point;
                DrawDot(i, pointClouds[i], Color.green);
            }
            else
            {
                pointClouds[i] = transform.position + dir * laserMaxDistance;
                DrawDot(i, pointClouds[i], Color.red);
            }
        }
    }

    private void DrawDot(int index, Vector3 position, Color color)
    {
        if (_dotMeshRenderers[index] != null)
        {
            _dotMeshRenderers[index].transform.position = position;
            _dotMeshRenderers[index].material.color = color;
        }
        else
        {
            var dotInstance = Instantiate(dotPrefab, position, Quaternion.identity);
            dotInstance.transform.parent = transform;
            var dotMeshRenderer = dotInstance.GetComponent<MeshRenderer>();
            _dotMeshRenderers[index] = dotMeshRenderer;
        }
    }
}
