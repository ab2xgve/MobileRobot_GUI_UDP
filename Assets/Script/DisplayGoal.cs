using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // <---- 追加1
using TMPro;

public class DisplayGoal : MonoBehaviour
{
    public GameObject goal;
    public TextMeshProUGUI name;
    public TextMeshProUGUI position;
    public TextMeshProUGUI rotation;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        DataContainer cont = FindObjectOfType<DataContainer>();
        this.goal.SetActive(cont.accessOnShowTarget);

        if (goal.activeSelf == true)
        {
            //this.gameObject.SetActive(cont.accessOnShowTarget);
            float x = cont.accessDesiredValues[0];
            float y = cont.accessDesiredValues[1];
            float z = cont.accessDesiredValues[2];
            float theta = cont.accessDesiredValues[5];

            transform.position = new Vector3(x, y, z);
            transform.rotation = Quaternion.AngleAxis(theta, new Vector3(0, 0, 1));

            this.name.text = "Goal";
            this.position.text = "(x:" + x.ToString("F2") + ", y:" + y.ToString("F2") + ")";
            this.rotation.text = "θ:" + theta.ToString("F1") + "";

            this.name.transform.position = new Vector3(x - 1.8f, y + 0.8f, z);
            this.position.transform.position = new Vector3(x - 1.8f, y + 0.5f, z);
            this.rotation.transform.position = new Vector3(x - 1.8f, y + 0.3f, z);

            this.name.transform.rotation     = Quaternion.AngleAxis(0, new Vector3(0, 0, 1));
            this.position.transform.rotation = Quaternion.AngleAxis(0, new Vector3(0, 0, 1));
            this.rotation.transform.rotation = Quaternion.AngleAxis(0, new Vector3(0, 0, 1));
        }
        else
        {
            this.name.text = "";
            this.position.text = "";
            this.rotation.text = "";

        }

    }
}
