using UnityEngine;
using UnityEngine.UI;

public class UIScreenManager : MonoBehaviour
{
    public RectTransform rectTransform;
    public Animation animationComponent;
    public UIManager uiManager;


    public void Animate(Vector2 position, Vector2 direction, float duration, bool destroyOnAnimationEnd)
    {
        // This function animates the screen from a certain position in a certain direction

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

        if (destroyOnAnimationEnd)
        {
            AnimationEvent destroyEvent = new AnimationEvent();
            destroyEvent.time = duration;
            destroyEvent.functionName = "SendDestroyScreen";

            animationClip.AddEvent(destroyEvent);
        }

        animationComponent.AddClip(animationClip, animationClip.name);
        animationComponent.Play(animationClip.name);
    }

    // Communicate upwards to UIManager (sends messages)

    public void SendDestroyScreen()
    {
        uiManager.DestroyScreen(gameObject.GetComponent<UIScreenManager>());
    }

    public void SendNextScreen(UIScreenManager nextScreen)
    {
        uiManager.NextScreen(nextScreen);
    }

    public void SendPreviousScreen(UIScreenManager previousScreen)
    {
        uiManager.PreviousScreen(previousScreen);
    }
}
