using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleShowTarget : MonoBehaviour
{
    public Toggle show_target;
    // Start is called before the first frame update
    void Start()
    {
        DataContainer cont = FindObjectOfType<DataContainer>();
        //show_target = GetComponent<Toggle>();
        show_target.isOn = false;
        cont.accessOnShowTarget = show_target.isOn;
        Debug.Log(cont.accessOnShowTarget);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void onValueChanged()
    {
        DataContainer cont      = FindObjectOfType<DataContainer>();
        cont.accessOnShowTarget = show_target.isOn;
        Debug.Log(cont.accessOnShowTarget);

    }
}
