using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) Quit(); //EscapeキーでUnity終了

        if (Input.GetKeyDown(KeyCode.Return)) Reboot();
    }

    private void Quit()
    {
        #if UNITY_EDITOR //Editor時にTure
             UnityEditor.EditorApplication.isPlaying = false;   
        #elif UNITY_STANDALONE //.exe起動時（プラットフォーム不問にTrue
             UnityEngine.Application.Quit();
        #endif
    }

    private void Reboot()
    {
        #if UNITY_EDITOR
            Debug.Log(" not implementation editer mode ");

        #elif UNITY_STANDALONE //.exe起動時（プラットフォーム不問にTrue
            System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe"));
            Application.Quit();
        #endif
    }
}