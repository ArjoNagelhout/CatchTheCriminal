using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    public UIScreenManager currentScreen;

    public UIScreenManager nextScreen;

    private Vector2 directionNext = new Vector2(-1, 0);
    private Vector2 directionPrevious = new Vector2(1, 0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            SwitchScreen(nextScreen, directionNext, 0.5f);
        }
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

    public void DestroyScreen(UIScreenManager toDestroyScreen)
    {
        Destroy(toDestroyScreen.gameObject);
    }
}
