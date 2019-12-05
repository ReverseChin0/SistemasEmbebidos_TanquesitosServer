using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenuM;
    public bool canPause = true;

    public void MyPause()
    {
        Cursor.visible = !pauseMenuM.activeSelf;
        Cursor.lockState = CursorLockMode.None;
        pauseMenuM.SetActive(!pauseMenuM.activeSelf);
    }

    private void Update()
    {
        if (canPause && Input.GetKeyDown(KeyCode.P))
        {
            MyPause();
        }
    }

    public void backToMain()
    {
        if (FindObjectOfType<TCPClient>() != null)
        {
            FindObjectOfType<TCPClient>().EndThreadsComunications();
        }
        else
        {
            FindObjectOfType<TCPServer>().EndThreadsComunications();
        }
        SceneManager.LoadScene("Menu"); 
    }
}
