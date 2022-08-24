using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    //смена сцены
    public void ChangeSceneInt(int numberScene)
    {
        SceneManager.LoadSceneAsync(numberScene);
    }
}
