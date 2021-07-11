using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Linq;

public class DataContainer : MonoBehaviour
{
//共有変数
    private float[] desired_values; //目標位置姿勢(6*1) [x y z rx ry rz]
    private float[] robot;          //平均位置速度偏差(6*1)[x y z rx ry rz]
    private float[] input;          //送信先 PC への入力値 [mode x y z rx ry rz] (7*1)

    private bool master_stop; //完全停止指令
    private bool brake; //ブレーキ指令
    private int command; //モード指令 
                           /*
                            -1 : Error
                             0 : Stop
                             1 : Position & Velocity
                             2 : Velocity & Acceseleration
                             3 : UDP通信テスト
                             4 : その他
                           */

    private float test;           // 送受信テスト用
    private string messege;       // オブジェクト間メッセージ
    private string udp_messeage;  // UDP通信の通信状態表示用メッセージ
    private IPEndPoint send_EP; // 送受信先 PC の UDP 通信エンドポイント
    private IPEndPoint recv_EP;   // Interface を動作させる PC の UDP 通信エンドポイント
    private bool on_show_target;

    // Start is called before the first frame update
    private void Start()
    {
        initDataContainer();
    }

    private void FixedUpdate()
    {
        if(!this.master_stop)
        {
            this.input[0] = this.command;
        }  
        else if(this.master_stop)
        {
            this.input[0] = 0;
            this.input[1] = 0;
            this.input[2] = 0;
            this.input[3] = 0;
            this.input[4] = 0;
            this.input[5] = 0;
            this.input[6] = 0;
        }

        //if (this.brake)
    }
    
    public void initDataContainer()
    {
        // 変数の初期化
        this.master_stop = false; // 1: 停止状態
        this.brake = false;
        this.desired_values = Enumerable.Repeat<float>(0f, 6).ToArray();  //6*1 のベクトルを 0 で初期化
        this.desired_values[0] = 1.0f;this.desired_values[1] = 1.0f;
        this.robot = Enumerable.Repeat<float>(0f, 6).ToArray();  //6*1 のベクトルを 0 で初期化
        this.input = Enumerable.Repeat<float>(0f, 7).ToArray();  //7*1 のベクトルを 0 で初期化
        this.command = 0; //0:run , -1:stop
        this.send_EP = null;
        this.recv_EP = null;
        this.udp_messeage = "Disconnect";
        this.on_show_target = false;
    }



// プロパティ
    public float[] accessDesiredValues 
    {
        get { return this.desired_values; }
        set { this.desired_values = value; }
    }

    public float[] accessRobot
    {
        get { return this.robot; }
        set { this.robot = value; }
    }

    public float[] accessInputData 
    {
      get { return this.input; } 
      set { this.input = value; } 
    }

    public bool accessMasterStop
    {
        get { return this.master_stop; }
        set { this.master_stop = value; }
    }

    public int accessCommand
    {
        get { return this.command; }
        set { this.command = value; }
    }

    public bool accessBrake
    {
        get { return this.brake; }
        set { this.brake = value; }
    }

    public IPEndPoint accessSendIPEndPoint
    {
        get { return this.send_EP; }
        set { this.send_EP = value; }
    }

    public IPEndPoint accessRecvIPEndPoint
    {
        get { return this.recv_EP; }
        set { this.recv_EP = value; }
    }

    public string accessUDPMesseage
    {
        get { return this.udp_messeage; }
        set { this.udp_messeage = value; }
    }

    public float accessTest
    {
        get { return this.test; }
        set { this.test = value; }
    }

    public string accessMesseage
    {
        get { return this.messege; }
        set { this.messege = value; }
    }

    public bool accessOnShowTarget
    {
        get { return this.on_show_target; }
        set { this.on_show_target = value; }
    }

}
