using UnityEngine;

public class CoreSystems : MonoBehaviour
{
    public static CoreSystems Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
