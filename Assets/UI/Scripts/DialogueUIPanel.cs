using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueUIPanel : MonoBehaviour
{
    [SerializeField] private RectTransform m_DialogueParent;
    [SerializeField] private CanvasGroup m_Group;

    public GameObject SubtitlePrefab;
    public GameObject DialogueOptionPrefab;

    void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_DialogueParent);
        m_Group.alpha = 0;
        UIManager.EnableCanvasGroup(m_Group, false);
    }

    public GameObject AddLine(string text)
    {
        GameObject subtitleObj = Instantiate(SubtitlePrefab);
        subtitleObj.transform.SetParent(m_DialogueParent, false);

        Subtitle subtitle = subtitleObj.GetComponent<Subtitle>();
        subtitle.SubtitleText.text = text;

        return subtitleObj;
    }

    public GameObject AddOption(string text, UnityAction onClick)
    {
        GameObject optionObj = Instantiate(DialogueOptionPrefab);
        optionObj.transform.SetParent(m_DialogueParent, false);

        DialogueOption subtitle = optionObj.GetComponent<DialogueOption>();
        subtitle.OptionText.text = text;
        Button optButton = subtitle.OptionButton;

        optButton.onClick.AddListener(() => HandleButtonClick(subtitle));
        optButton.onClick.AddListener(onClick);

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_DialogueParent);

        return optionObj;
    }

    private void HandleButtonClick(DialogueOption subtitle)
    {
        Destroy(subtitle.gameObject);
    }

    public void ClearUI()
    {
        for (int i = 0; i < m_DialogueParent.transform.childCount; i++)
        {
            Destroy(m_DialogueParent.transform.GetChild(i).gameObject);
        }
    }

    public void ShowUI(bool show)
    {
        m_Group.alpha = show ? 1 : 0;
        UIManager.EnableCanvasGroup(m_Group, show);
    }
}
