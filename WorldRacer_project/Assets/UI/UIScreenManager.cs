using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreenManager : MonoBehaviour
{
    public RectTransform rectTransform;
    public Animation animationComponent;

    public void Animate(Vector2 position, float duration, Vector2 direction)
    {
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        AnimationClip animationClip = new AnimationClip
        {
            legacy = true
        };


        Keyframe[] keysX = new Keyframe[2];
        keysX[0] = new Keyframe(0.0f, position.x * width, 0.0f, 0.0f);
        keysX[1] = new Keyframe(duration, (position.x + direction.x) * width, 0.0f, 0.0f);

        Keyframe[] keysY = new Keyframe[2];
        keysY[0] = new Keyframe(0.0f, position.y * height, 0.0f, 0.0f);
        keysY[1] = new Keyframe(duration, (position.y + direction.y * height), 0.0f, 0.0f);

        AnimationCurve animationCurveX = new AnimationCurve(keysX);
        AnimationCurve animationCurveY = new AnimationCurve(keysY);

        animationClip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.x", animationCurveX);
        animationClip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.y", animationCurveY);

        animationComponent.AddClip(animationClip, animationClip.name);
        animationComponent.Play(animationClip.name);

        Debug.Log(string.Format("Position: {0}, Direction: {1}", position, direction));
    }
}
