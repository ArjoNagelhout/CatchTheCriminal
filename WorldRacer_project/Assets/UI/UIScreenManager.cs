using UnityEngine;
using UnityEngine.UI;

public class UIScreenManager : MonoBehaviour
{
    public RectTransform rectTransform;
    public Animation animationComponent;

    [System.NonSerialized]
    public UIManager uiManager;
    [System.NonSerialized]
    public ServerController serverController;

    

    public void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        serverController = FindObjectOfType<ServerController>();
    }

    public void Animate(Vector2 position, Vector2 direction, float duration, bool destroyOnAnimationEnd, bool activateCurrentScreenOnAnimationEnd)
    {
        // This function animates the screen from a certain position in a certain direction

        // Make sure the user can't use buttons while the animation is playing
        SendDeactivateScreen();


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
        keysY[1] = new Keyframe(duration, (position.y + direction.y) * height, 0.0f, 0.0f);

        AnimationCurve animationCurveX = new AnimationCurve(keysX);
        AnimationCurve animationCurveY = new AnimationCurve(keysY);

        animationClip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.x", animationCurveX);
        animationClip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.y", animationCurveY);

        if (destroyOnAnimationEnd)
        {
            AnimationEvent destroyEvent = new AnimationEvent
            {
                time = duration,
                functionName = "SendDestroyScreen"
            };

            animationClip.AddEvent(destroyEvent);
        }

        if (activateCurrentScreenOnAnimationEnd)
        {
            AnimationEvent activateCurrentScreenEvent = new AnimationEvent
            {
                time = duration,
                functionName = "SendActivateCurrentScreen"
            };

            animationClip.AddEvent(activateCurrentScreenEvent);
        }

        // Make sure the user will be able to use buttons when the animation is finished
        AnimationEvent activateEvent = new AnimationEvent
        {
            time = duration,
            functionName = "SendActivateScreen"
        };
        animationClip.AddEvent(activateEvent);

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


    public void SendDismissBottomOverlay()
    {
        uiManager.DismissBottomOverlay();
    }


    public void SendPresentBottomOverlay(UIScreenManager overlayScreen)
    {
        uiManager.PresentBottomOverlay(overlayScreen);
    }


    // Set button states
    public void SendActivateCurrentScreen()
    {
        uiManager.ActivateCurrentScreen();
    }

    public void SendActivateScreen()
    {
        uiManager.ActivateScreen(gameObject.GetComponent<UIScreenManager>());
    }

    public void SendDeactivateScreen()
    {
        uiManager.DeactivateScreen(gameObject.GetComponent<UIScreenManager>());
    }

    // Communicate upwards to ServerController (sends messages)
    public void SetName(string playerName)
    {
        serverController.playerName = playerName;
    }

    public void CreateGame()
    {
        int time = FindObjectOfType<SetTimeSliderManager>().currentTime;

        Playfield playfield = new Playfield();

        for (int i = 0; i < 10; i++)
        {
            playfield.points.Add(new Vector2(i, i));
        }

        serverController.CreateGame(time, playfield);
    }

    public void JoinGame(Text roomPin)
    {
        serverController.JoinGame(roomPin.text);
    }
}
