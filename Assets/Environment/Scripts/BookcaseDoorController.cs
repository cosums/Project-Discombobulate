using UnityEngine;

public class BookcaseDoorController : MonoBehaviour
{
    private DoorController doorController;
    [SerializeField] private GameObject hintVfx;
    [SerializeField] private Dialogue dialogueToPlayOnApproach;
    private bool canPlayDialogue = false;
    private bool hasOpened = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        doorController = GetComponent<DoorController>();
        if (doorController != null) {
            doorController.OnTryOpenDoor += () => { if (!hasOpened) OnFirstOpen(); };
        }else
        {
            Debug.LogWarning("bookcase door cant find door controller");
        }
        if (hintVfx == null) Debug.LogWarning("bookcase door cant find hint vfx");
        // TODO determine and subscribe to game state change
        AllowOpening();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AllowOpening()
    {
        if(doorController) doorController.enabled = true;
        if(hintVfx) hintVfx.SetActive(true);
        doorController.OpenDoor(false);
        canPlayDialogue=true;
    }

    void OnFirstOpen()
    {
        if(hintVfx) hintVfx.SetActive(false);
        doorController.OpenDoor(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canPlayDialogue)
        {
            if(other.tag == "player")
            {
                if(dialogueToPlayOnApproach) DialogueSystem.Instance.PlayDialogue(dialogueToPlayOnApproach);
                canPlayDialogue=false;
            }
        }
    }
}
