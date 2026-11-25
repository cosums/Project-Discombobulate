using UnityEngine;

public class BookcaseDoorController : MonoBehaviour
{
    private DoorController doorController;
    [SerializeField] private GameObject hintVfx;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        doorController = GetComponent<DoorController>();
        if (doorController == null) Debug.LogWarning("bookcase door cant find door controller");
        if (hintVfx == null) Debug.LogWarning("bookcase door cant find hint vfx");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AllowOpening()
    {
        if(doorController) doorController.enabled = true;
        if(hintVfx) hintVfx.SetActive(true);
    }

    void OnFirstOpen()
    {
        if(hintVfx) hintVfx.SetActive(false);
    }
}
