using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HandController))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;
    
    private HandController _handController;

    private void Awake()
    {
        _handController = GetComponent<HandController>();
        Application.runInBackground = true;
    }

    private void FixedUpdate()
    {
        CheckTriggerDetection(leftImage);
        CheckTriggerDetection(rightImage);
    }

    private void CheckTriggerDetection(Image image)
    {
        if (IsRectTransformOverlapping(image.rectTransform, _handController.HandRectTransform))
            image.color = Color.green;
        else
            image.color = Color.white;
    }
    
    private bool IsRectTransformOverlapping(RectTransform rect1, RectTransform rect2)
    {
        Rect rectA = GetWorldRect(rect1);
        Rect rectB = GetWorldRect(rect2);

        return rectA.Overlaps(rectB);
    }

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float xMin = corners[0].x;
        float yMin = corners[0].y;
        float width = corners[2].x - xMin;
        float height = corners[2].y - yMin;

        return new Rect(xMin, yMin, width, height);
    }
}
