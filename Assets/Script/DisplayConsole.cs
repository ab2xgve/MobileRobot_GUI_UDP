using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class DisplayConsole : MonoBehaviour
{
    public Text text1;
    public Text text2;
    public Text text3;
    public Text text4;
    public Text text5;

    IPEndPoint recv_EP;
    IPEndPoint send_EP;

    private string udp_message;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        DataContainer cont = FindObjectOfType<DataContainer>();
        float x = cont.accessDesiredValues[0];
        float y = cont.accessDesiredValues[1];
        float z = cont.accessDesiredValues[2];
        float theta = cont.accessDesiredValues[5];

        send_EP = cont.accessSendIPEndPoint;
        recv_EP = cont.accessRecvIPEndPoint;

        udp_message = cont.accessUDPMesseage;
        

        this.text1.text = "UDP |"+ " Status:" + udp_message;
        this.text2.text = "  Send:" + send_EP.ToString() + "| Recv:" + recv_EP.ToString();
        if(cont.accessOnShowTarget)
            this.text3.text = "Desired Pose | x:"+x.ToString("F2") +", y:"+y.ToString("F2") +", θ:"+ theta.ToString("F2");
        else
            this.text3.text = " ";

        this.text4.text = " ";
        this.text5.text = " ";
    }
}
