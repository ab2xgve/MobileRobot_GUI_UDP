using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ModeSwitch : MonoBehaviour
{
    public ToggleGroup mode_switch;
    public Text toggle1;
    public Text toggle2;
    public Text toggle3;
    public Text toggle4;
    public Text toggle5;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        DataContainer cont = FindObjectOfType<DataContainer>();
        string mode = mode_switch.ActiveToggles().First().GetComponentsInChildren<Text>().First().text;

        if(mode == toggle1.text) cont.accessCommand = 0;
        else if (mode == toggle2.text) cont.accessCommand = 1;
        else if (mode == toggle3.text) cont.accessCommand = 2;
        else if (mode == toggle4.text) cont.accessCommand = 3;
        else if (mode == toggle5.text) cont.accessCommand = 4;
    }

}
