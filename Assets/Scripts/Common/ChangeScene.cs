using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void ChangeSceneInt(int numberScene)
    {
        SceneManager.LoadSceneAsync(numberScene);
    }
}
