/*
Host  : 通信相手
Uniyt : Interface PC
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO; //System.IO.FileInfo, System.IO.StreamReader, System.IO.StreamWriter
using System; //Exception
using System.Linq;
using UnityEngine.SceneManagement;

public class UDPConnection : MonoBehaviour
{
    //Host PC のIP
    static IPAddress  host_adress; //IP アドレス

    //IP Port
    static int send_port;   //送信 IP ポート 
    static int recv_port;   //受信 IP ポート
    static IPEndPoint send_EP;     //送信 エンドポイント
    static IPEndPoint recv_EP;     //受信 エンドポイント

    static UdpClient unity_client; //UDP通信クライアント

    //送受信用コンテナ
    static private float[] trans_float;
    static private float[] recv_float;

    private string message;

    //受信用スレッド
    private Thread thread;
    static private bool is_recving;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneUnloaded += SceneUnloaded;
        connexionUDP();
        Debug.Log("UDP Connected");
    }

    void connexionUDP() // x is cool
    {
        trans_float = Enumerable.Repeat<float>(0f, 7).ToArray();
        recv_float = Enumerable.Repeat<float>(0f, 12).ToArray();
        is_recving = false;
        message = "Disconnect";

        //textファイルからUDP設定(IPAdress, IPPort)を読み込み
        string config;
        config = this.readConfigText();
        this.splitConfigText(config);

        message = "Connecting...";
        //エンドポイント、接続の設定
        send_EP = new IPEndPoint(host_adress, send_port);
        recv_EP = new IPEndPoint(IPAddress.Any, recv_port);
        DataContainer cont = FindObjectOfType<DataContainer>();
        cont.accessSendIPEndPoint = send_EP;
        cont.accessRecvIPEndPoint = recv_EP;
        //Unity と Host PC の接続
        unity_client = new UdpClient(recv_EP);
        message = "Connected";

        //受信スレッドの立ち上げ
        is_recving = true;
        thread = new Thread(new ThreadStart(ReceiveThread));
        thread.Start();
    }

    /*終了処理*/
    void OnApplicationQuit()
    {
        closeUDP();
        Debug.Log("UDP QUIT");
    }

    void closeUDP()
    {
        is_recving = false;

        trans_float = Enumerable.Repeat<float>(0f, 13).ToArray();
        //送信データ変換 (float型配列 -> byte型配列)
        byte[] trans_bytes = convertTransBytes(trans_float);
        //Host PC へのデータ送信
        unity_client.SendAsync(trans_bytes, trans_bytes.Length, send_EP);

        if (thread != null) thread.Abort();             //スレッドの終了
        if (unity_client != null) unity_client.Close(); //UDP通信の切断
        Debug.Log("UDP DisConnected");

        SceneManager.sceneUnloaded -= SceneUnloaded;
    }

    void SceneUnloaded(Scene thisScene)
    {
        closeUDP();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log( Time.deltaTime.ToString());
        DataContainer cont = FindObjectOfType<DataContainer>();

        if (cont.accessMasterStop)
        {
            cont.accessInputData = Enumerable.Repeat<float>(0f, 13).ToArray();
            cont.accessInputData[0] = cont.accessCommand;
        }

        trans_float = cont.accessInputData;
        //送信データ変換 (float型配列 -> byte型配列)
        byte[] trans_bytes = convertTransBytes(trans_float);
        //Host PC へのデータ送信
        unity_client.SendAsync(trans_bytes, trans_bytes.Length, send_EP);
        //unity_client.Send(trans_bytes, trans_bytes.Length, send_EP);

        cont.accessDesiredValues = new float[6] { recv_float[0], recv_float[1], recv_float[2], recv_float[3], recv_float[4], recv_float[5] }.Select(i => i / 1.0f).ToArray();
        cont.accessRobot         = new float[6] { recv_float[6], recv_float[7], recv_float[8], recv_float[9], recv_float[10], recv_float[11] }.Select(i => i / 1.0f).ToArray();
        cont.accessUDPMesseage   = message;
        //※DataContainerはメインスレッドのみでしか使えない

    }

    /* 受信スレッド処理 */
    void ReceiveThread()
    {
        IPEndPoint remote_EP = new IPEndPoint(IPAddress.Any, send_port); //or null; //受信先アドレス, 受信ポート
        while (is_recving)
        {
            try
            {
                //double 型　データの受信
                byte[] recv_bytes = unity_client.Receive(ref remote_EP);
                Interlocked.Exchange(ref recv_float, convertRecvFloat(recv_bytes));
                Interlocked.Exchange(ref message, "Connection Succesed ");
            }
            catch (Exception e){
                //Debug.Log("Receive Fail : " + e.ToString());
                Interlocked.Exchange(ref message, "Connection Failed ");
            }
        }
    }


    /* IPアドレス, IPポート読み込み処理 */
    private string readConfigText()//設定テキストの読み込み ( /asset/config/UDPConfig.txt)
    {
        string text = "";
        FileInfo udp_config = new FileInfo(Application.dataPath + "/" + "config" + "/" + "UDPConfig.txt"); //udp設定テキストファイルの読み取り

        try
        {
            using (StreamReader text_conf = new StreamReader(udp_config.OpenRead(), Encoding.UTF8)) //using の使用でDisposeの記述を省略
            {
                text = text_conf.ReadToEnd();
            }

        }
        catch (Exception e)
        {
            text += "\n"; //改行
        }

        return text;
    }
    
    private void splitConfigText(string text)//テキストの分割処理
    {
        string[] sep = { "\r\n" }; //セパレータ（改行コード）
        string[] str = text.Split(sep, StringSplitOptions.None); //分割処理
        host_adress  = IPAddress.Parse(str[0]); //ホスト(通信相手)IPアドレス
        send_port    = int.Parse(str[1]);       //ホスト(通信相手)IPポート
        recv_port   = int.Parse(str[2]);       //Unity 側 PC IPポート

        Debug.Log("Host Adress : "  + str[0]);
        Debug.Log("Host Port : "    + str[1]);
        Debug.Log("Unity Port : "   + str[2]);
    }

    /* データ変換 */
    private byte[]  convertTransBytes(float[] origin) // float -> byte 変換
    {
        byte[] trans_bytes = new byte[trans_float.Length * sizeof(Single)];

        for (int i = 0; i < origin.Length; i++)
        {
            if (origin[i] != null)
            {
                byte[] tmp = BitConverter.GetBytes(origin[i]);

                trans_bytes[i * 4]     = tmp[0];
                trans_bytes[i * 4 + 1] = tmp[1];
                trans_bytes[i * 4 + 2] = tmp[2];
                trans_bytes[i * 4 + 3] = tmp[3];
            }
            else
            {
                trans_bytes[i] = 0;
            }
        }

        return trans_bytes;
    }

    private float[] convertRecvFloat(byte[] recv_bytes)
    {
        float[] recv_val = new float[recv_bytes.Length / sizeof(Single)];

        for (int i = 0; i < recv_val.Length; i++)
        {
            float tmp = BitConverter.ToSingle(recv_bytes, i * 4);

            if (tmp != null)
                recv_val[i] = tmp;
            else
                recv_val[i] = .0f;
        }

        return recv_val;
    }
}
