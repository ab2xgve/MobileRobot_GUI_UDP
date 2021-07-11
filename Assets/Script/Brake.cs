using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO; //System.IO.FileInfo, System.IO.StreamReader, System.IO.StreamWriter
using System; //Exception

public class Brake : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject brake_button;
    private float[]    position;
    private float[]    pre_position;
    private float gain_torq;
    private float gain_vel;

    // Start is called before the first frame update
    void Start()
    {
        DataContainer cont = FindObjectOfType<DataContainer>();
        cont.accessBrake = false;
        brake_button = GameObject.Find("Brake");

        string config;
        config = this.readConfigText();
        this.splitConfigText(config);
        this.position = Enumerable.Repeat<float>(0f, 6).ToArray();
        this.pre_position = Enumerable.Repeat<float>(0f, 6).ToArray();
        brake_button.GetComponent<Image>().color = Color.white;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DataContainer cont = FindObjectOfType<DataContainer>();
        pre_position = position;
        position = cont.accessRobot;

        if(cont.accessBrake)
        {
            float[] velocity = calcVelocity(position, pre_position);
            float[] brake_acc;
            if (cont.accessCommand == 1)
            {
                brake_acc = calcBrake(velocity, this.gain_torq);
            }
            else if (cont.accessCommand == 2)
            {
                brake_acc = calcBrake(velocity, this.gain_vel);
            }
            else
            {
                brake_acc = calcBrake(velocity, 0);
            }

            for (int i = 0; i < brake_acc.Length; i++)
            {
                cont.accessInputData[i + 1] = brake_acc[i];
            }
        }
    }

    public void onButtonClick()
    {
        DataContainer cont = FindObjectOfType<DataContainer>();

        if (!cont.accessBrake)
        {
            cont.accessBrake = true;
            brake_button.GetComponent<Image>().color = Color.red;
        }
        else
        {
            cont.accessBrake = false;
            brake_button.GetComponent<Image>().color = Color.white;
        }
        
    }

    public void offButtonClick()
    {
        
    }

    private float[] calcVelocity(float[] position, float[] pre_position)
    {
        float[] velocity = Enumerable.Repeat<float>(0f, position.Length).ToArray();
        for (int i=0; i<position.Length; i++)
        {
            velocity[i] = position[i] - pre_position[i];
        }

        return velocity;
    }

    private float[] calcBrake(float[] velocity, float Ke)
    {
        float[] brake = Enumerable.Repeat<float>(0f, velocity.Length).ToArray();
        for (int i = 0; i<velocity.Length; i++)
        {
            brake[i] = -Ke * velocity[i];
        }

        return brake;
    }

    private string readConfigText()//設定テキストの読み込み ( /asset/config/UDPConfig.txt)
    {
        string text = "";
        FileInfo udp_config = new FileInfo(Application.dataPath + "/" + "config" + "/" + "BrakeGainConfig.txt"); //udp設定テキストファイルの読み取り

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

        this.gain_torq = float.Parse(str[0]);
        this.gain_vel  = float.Parse(str[1]);
    }
}
