using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public UIScreenManager startScreen;

    [System.NonSerialized]
    public UIScreenManager currentScreen;
    public UIScreenManager currentOverlayScreen;

    public GameObject uiPopupObject;

    public float duration = 1f;
    public int popupDuration = 2;

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

    public void PresentBottomOverlay(UIScreenManager overlayScreen)
    {
        PresentOverlay(overlayScreen, new Vector2(0, 1), duration);
    }

    public void DismissBottomOverlay()
    {
        DismissOverlay(new Vector2(0, -1), duration);
    }



    public void SwitchScreen(UIScreenManager nextScreen, Vector2 direction, float duration)
    {
        // Creates a new screen, animates this into the view and animates the current screen out of view
        // The new screen is now the current screen

        UIScreenManager nextScreenInstance = Instantiate(nextScreen, transform);
        nextScreenInstance.Animate(-direction, direction, duration, false, false);

        currentScreen.Animate(Vector2.zero, direction, duration, true, false);

        currentScreen = nextScreenInstance;

        if (currentOverlayScreen != null)
        {
            currentOverlayScreen.Animate(Vector2.zero, direction, duration, true, false);
        }
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

        currentScreen = screenInstance;
    }


    public void PresentOverlay(UIScreenManager overlayScreen, Vector2 direction, float duration)
    {
        if (currentOverlayScreen != null)
        {
            return;
        }

        UIScreenManager overlayScreenInstance = Instantiate(overlayScreen, transform);
        overlayScreenInstance.Animate(-direction, direction, duration, false, false);

        currentOverlayScreen = overlayScreenInstance;

        // Deactivate buttons on underlying screen
        DeactivateScreen(currentScreen);
    }


    public void DismissOverlay(Vector2 direction, float duration)
    {
        if (currentOverlayScreen == null) {
            return;
        }

        currentOverlayScreen.Animate(Vector2.zero, direction, duration, true, true);
        currentOverlayScreen = null;
    }


    public void DestroyScreen(UIScreenManager toDestroyScreen)
    {
        Destroy(toDestroyScreen.gameObject);
    }


    public void ActivateCurrentScreen()
    {
        ActivateScreen(currentScreen);
    }

    public void ActivateScreen(UIScreenManager toActivateScreen)
    {
        Button[] buttons = toActivateScreen.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }


    public void DeactivateScreen(UIScreenManager toDeactivateScreen)
    {
        Button[] buttons = toDeactivateScreen.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

    public void ShowPopup(string displayString, int displayTime)
    {
        GameObject uiPopupInstance = Instantiate(uiPopupObject, gameObject.transform);
        UIPopup uiPopup = uiPopupInstance.GetComponent<UIPopup>();
        uiPopup.displayString = displayString;
        uiPopup.displayTime = displayTime;
    }
}
