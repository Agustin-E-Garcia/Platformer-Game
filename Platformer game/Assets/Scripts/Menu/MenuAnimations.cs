using DG.Tweening;
using UnityEngine;

public class MenuAnimations : MonoBehaviour
{
    RectTransform myTransform;
    public Vector2 desiredPosition;

    [Range(0.0f, 1.0f)]
    public float duration = 1.0f;

    private void Start()
    {
        myTransform = transform as RectTransform;
        myTransform.DOAnchorPos(desiredPosition , duration);
    }
}
