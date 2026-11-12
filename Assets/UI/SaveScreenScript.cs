using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveScreenScript : MonoBehaviour
{

    public Canvas saveScreen;
    public Canvas titleScreen;

    public Button backButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backButton.onClick.AddListener(OnBackClick);
    }

    void OnBackClick()
    {
        Debug.Log("Button clicked!");
        titleScreen.enabled = true;
        saveScreen.enabled = false;
    }
}
