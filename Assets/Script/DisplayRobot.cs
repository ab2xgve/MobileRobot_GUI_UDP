using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // <---- 追加1
using System.Collections;
using TMPro;

public class DisplayRobot : MonoBehaviour
{
    private Transform marker;
    public TextMeshProUGUI name;
    public TextMeshProUGUI position;
    public TextMeshProUGUI rotation;

    public float text_pos_x;
    public float text_pos_y;
   
    // Start is called before the first frame update
    void Start()
    {
        this.marker = transform.Find("Robot Marker");

    }

    // Update is called once per frame
    void Update()
    {
        DataContainer cont = FindObjectOfType<DataContainer>();
        float x = cont.accessRobot[0];
        float y = cont.accessRobot[1];
        float z = cont.accessRobot[2];
        float theta = cont.accessRobot[5];

        this.marker.transform.position = new Vector3(x, y, z);
        this.marker.transform.rotation = Quaternion.AngleAxis(theta, new Vector3(0, 0, 1));

        this.position.text = "(x:" + x.ToString("F2") + ", y:" + y.ToString("F2") + ")";
        this.rotation.text = "θ:" + theta.ToString("F1") + "";

        this.name.transform.position = new Vector3(x, y + 0.5f, z);
        this.position.transform.position = new Vector3(x + 1.8f, y - 0.3f, z);
        this.rotation.transform.position = new Vector3(x + 1.8f, y - 0.5f, z);
    }
}
