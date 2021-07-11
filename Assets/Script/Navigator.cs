using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Navigator : MonoBehaviour
{
    public Sprite guide;
    public Sprite isOn;
    public Sprite right;
    public Sprite left;
    //public GameObject Goal;
    //public Sprite Goal;

    SpriteRenderer render;
    Transform navi;
    GameObject robot;
    GameObject goal;

    private Sprite navigator;

    float r = 0.5f;
    float r_thr = 5 * Mathf.PI / 180;
    float p_thr = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        render = this.gameObject.GetComponent<SpriteRenderer>();
        navi = this.gameObject.GetComponent<Transform>();
        robot = GameObject.Find("Display/Robot/Robot Marker");
        goal = GameObject.Find("Display/Goal");
        this.navigator = this.guide;
    }

    // Update is called once per frame
    void Update()
    {
        DataContainer cont = FindObjectOfType<DataContainer>();

        if (cont.accessOnShowTarget == true)
        {
            float x = robot.transform.position.x;
            float y = robot.transform.position.y;
            float theta = robot.transform.rotation.z;

            float x_d = goal.transform.position.x;
            float y_d = goal.transform.position.y;
            float theta_d = goal.transform.rotation.z;
            render.enabled = true;

            transNaviSprite(x, y, theta, x_d, y_d, theta_d);

            render.sprite = this.navigator;
        }
        else
        {
            render.enabled = false;
        }

    }

    private void transNaviSprite(float x, float y, float theta, float x_d, float y_d, float theta_d)
    {
        float dx = x_d - x;
        float dy = y_d - y;
        float direction = Mathf.Atan2(dx, dy);// * 180/Mathf.PI;
        if (float.IsInfinity(direction) | float.IsNaN(direction)) direction = 0;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        float theta_e = theta_d - theta;
               
        if (distance >= p_thr)
        {
            this.navigator = this.guide;
            navi.transform.position = new Vector3(x + r * Mathf.Sin(direction), y + r * Mathf.Cos(direction), 0);
            navi.transform.rotation = Quaternion.FromToRotation(Vector3.up, new Vector3(x_d - x, y_d - y, 0));
            //this.transform.rotation = Quaternion.AngleAxis(direction, new Vector3(0, 0, 1));
        }
        else if(theta_e > r_thr)
        {
            this.navigator = this.left;
            navi.transform.position = new Vector3(x + r * Mathf.Sin(theta + Mathf.PI / 2), y + r * Mathf.Cos(theta + Mathf.PI / 2), 0); ;
            navi.transform.rotation = Quaternion.AngleAxis(0f, Vector3.one);
        }
        else if(theta_e < -r_thr)
        {
            this.navigator = this.right;

            navi.transform.position = new Vector3(x + r * Mathf.Sin(theta + Mathf.PI/2), y + r * Mathf.Cos(theta + Mathf.PI / 2), 0); ;
            navi.transform.rotation = Quaternion.AngleAxis(0f, Vector3.one);
        }
        else
        {
            this.navigator = this.isOn;
            navi.transform.position = new Vector3(x, y, 0);
        }
    }
}
