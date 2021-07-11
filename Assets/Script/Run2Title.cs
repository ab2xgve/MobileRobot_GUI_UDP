using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Run2Title : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onButtonClick()
    {
        DataContainer cont = FindObjectOfType<DataContainer>();
        cont.accessMasterStop = true;
        SceneManager.LoadScene("Title");

    }


}
