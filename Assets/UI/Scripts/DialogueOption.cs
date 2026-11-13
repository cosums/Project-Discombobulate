using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueOption : MonoBehaviour
{
    public TMP_Text OptionText;
    public Button OptionButton;

    void OnDestroy()
    {
        OptionButton.onClick.RemoveAllListeners();
    }
}
