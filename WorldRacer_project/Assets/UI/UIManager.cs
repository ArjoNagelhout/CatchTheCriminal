using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    public UIScreenManager startScreen;

    private UIScreenManager currentScreen;

    public float duration = 1f;

    void Start()
    {
        InitializeScreen(startScreen);
    }

    // Quicker notations
    public void NextScreen(UIScreenManager nextScreen)
    {
        SwitchScreen(nextScreen, new Vector2(-1, 0), duration);
    }

    public void PreviousScreen(UIScreenManager previousScreen)
    {
        SwitchScreen(previousScreen, new Vector2(1, 0), duration);
    }


    public void SwitchScreen(UIScreenManager nextScreen, Vector2 direction, float duration)
    {
        // Creates a new screen, animates this into the view and animates the current screen out of view
        // The new screen is now the current screen

        UIScreenManager nextScreenInstance = Instantiate(nextScreen, transform);
        nextScreenInstance.Animate(-direction, direction, duration, false);
        nextScreenInstance.uiManager = gameObject.GetComponent<UIManager>();

        currentScreen.Animate(Vector2.zero, direction, duration, true);

        currentScreen = nextScreenInstance;
    }

    public void InitializeScreen(UIScreenManager screen)
    {
        // Delete all screens
        UIScreenManager[] previousScreens = FindObjectsOfType<UIScreenManager>();
        foreach (UIScreenManager previousScreen in previousScreens)
        {
            Destroy(previousScreen.gameObject);
        }

        UIScreenManager screenInstance = Instantiate(screen, transform);
        screenInstance.uiManager = gameObject.GetComponent<UIManager>();

        currentScreen = screenInstance;
    }


    public void OverlayScreen(UIScreenManager overlayScreen, Vector2 direction, float duration)
    {
        // Creates a new screen, animates this into the view on top of the existing screen
    }


    public void DestroyScreen(UIScreenManager toDestroyScreen)
    {
        Destroy(toDestroyScreen.gameObject);
    }


}
