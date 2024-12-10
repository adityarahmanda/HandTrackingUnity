using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LidarInteractiveSample : MonoBehaviour
{
    [SerializeField] private float triggerDistance;
    
    [Header("Dependency")] 
    [SerializeField] private LidarDataUDPReceiver lidarDataUDPReceiver;
    
    [Header("UI References")]
    [SerializeField] private Image leftImage;
    private bool _leftImageValid;
    [SerializeField] private Image rightImage;
    private bool _rightImageValid;
    
    public List<Vector3> _validCloudPoints = new ();
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        _validCloudPoints.Clear();
        foreach (var cloudPoint in lidarDataUDPReceiver.CloudPoints)
        {
            var distanceFromCamera = Vector3.Distance(_camera.transform.position, cloudPoint);
            if (distanceFromCamera < triggerDistance)
                _validCloudPoints.Add(cloudPoint);
        }
        
        CheckTriggerDetection(leftImage);
        CheckTriggerDetection(rightImage);
    }

    private void CheckTriggerDetection(Image image)
    {
        var isTriggered = false;
        foreach (var cloudPoint in _validCloudPoints)
        {
            var screenPoint = RectTransformUtility.WorldToScreenPoint(_camera, cloudPoint);
            if (RectTransformUtility.RectangleContainsScreenPoint(image.rectTransform, screenPoint))
            {
                isTriggered = true;
                break;                
            }
        }
        
        if (isTriggered)
            image.color = Color.green;
        else
            image.color = Color.white;
    }
}
