using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //simply just re-loads main scene
    public void ResetLevel()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
    //closes application
    public void EndGame()
    {
        Application.Quit();
    }
}
