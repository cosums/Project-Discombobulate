using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleScreenScript : MonoBehaviour
{

    public Canvas titleScreen;
    public Canvas saveScreen;

    public Button newGameButton;
    public Button contGameButton;
    public Button loadGameButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Add event listeners
        newGameButton.onClick.AddListener(OnNewClick);
        contGameButton.onClick.AddListener(OnContClick);
        loadGameButton.onClick.AddListener(OnLoadClick);
    }

    void OnNewClick()
    {
        Debug.Log("Button clicked!");
        saveScreen.enabled = true;
        titleScreen.enabled = false;
    }

    void OnContClick()
    {
        Debug.Log("Button clicked!");
        saveScreen.enabled = true;
        titleScreen.enabled = false;
    }
    
    void OnLoadClick()
    {
        Debug.Log("Button clicked!");
        saveScreen.enabled = true;
        titleScreen.enabled = false;
    }

}
