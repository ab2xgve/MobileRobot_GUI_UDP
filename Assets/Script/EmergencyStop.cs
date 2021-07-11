using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmergencyStop : MonoBehaviour
{
    private GameObject estop_button;

    // Start is called before the first frame update
    void Start()
    {
        DataContainer cont = FindObjectOfType<DataContainer>();
        cont.accessMasterStop = false;
        //cont.accessInputData[0] = cont.accessCommand;
        estop_button = GameObject.Find("StopButton");
        estop_button.GetComponent<Image>().color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onButtonClick()
    {
        DataContainer cont = FindObjectOfType<DataContainer>();
        if (cont.accessMasterStop)
        {
            cont.accessMasterStop = false; //run
            estop_button.GetComponent<Image>().color = Color.white;
        }
        else
        {
            cont.accessMasterStop = true; //stop
            estop_button.GetComponent<Image>().color = Color.red;
        }

    }
}
