using UnityEngine;
using UnityEngine.SceneManagement;

public class switchscript : MonoBehaviour
{
    public void SwitchScene(string sceneName)
    {
        Debug.Log("fjkasnhoa");
        SceneManager.LoadScene(sceneName);
    }
}