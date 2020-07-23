using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
