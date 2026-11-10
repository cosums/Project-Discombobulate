using UnityEngine;
using UnityEngine.UI;

public class DialogueUIPanel : MonoBehaviour
{
    [SerializeField] private RectTransform m_DialogueParent;

    void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_DialogueParent);
    }

    void Update()
    {
        
    }
}
