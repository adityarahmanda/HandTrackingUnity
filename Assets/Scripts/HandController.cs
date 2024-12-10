using System;
using UnityEngine;
using UnityEngine.UI;

public class HandController : MonoBehaviour
{
    public RectTransform HandRectTransform => handImage.rectTransform;
    
    [SerializeField] private UDPDataReceiver udpDataReceiver;
    [SerializeField] private RectTransform handCanvas;
    [SerializeField] private Image handImage;
    
    [SerializeField] private float lerpSpeed = 5f;

    private Vector2 _targetPosition;
    private Vector2 _currentPosition;

    private void Update()
    {
        if (udpDataReceiver.HandData.IsValid)
        {
            _targetPosition = udpDataReceiver.HandData.Position;
            _targetPosition.y = handCanvas.rect.size.y - _targetPosition.y;
            _currentPosition = Vector2.Lerp(_currentPosition, _targetPosition, lerpSpeed * Time.deltaTime);
            handImage.rectTransform.anchoredPosition = _currentPosition;
            handImage.gameObject.SetActive(true);
        }
        else
        {
            handImage.gameObject.SetActive(false);
        }
    }
}