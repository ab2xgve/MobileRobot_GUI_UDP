using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Threading;
using System.IO; //System.IO.FileInfo, System.IO.StreamReader, System.IO.StreamWriter
using System; //Exception

public class JoyStick : MonoBehaviour
{
    public FixedJoystick LeftJoystick;
    public FixedJoystick RightJoystick;

    private float gain_forcex; //force x
    private float gain_forcey; //force y
    private float gain_torqz; //torque
    private float gain_velx;  // velocity x
    private float gain_vely;  // velocity y
    private float gain_rvelz; // rotation velocity

    static int count_round = 0;
    static float theta_pre = 0f;
    static float x_pre = 0f;
    static float y_pre = 0f;//previous  y axis
    static float rv_pre = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //textファイルからGain値(位置, 速度, 加速度, 角度, 角速度, 角加速度)を読み込み
        string config;
        config = this.readConfigText();
        this.splitConfigText(config);
    }

    // Update is called once per frame
    void Update()
    {
        //指令値計算
        DataContainer cont = FindObjectOfType<DataContainer>();
        float theta_on_world = cont.accessRobot[5];
        float lx, ly;
        float rx, ry;

        float xacc, yacc;
        float rz;
        float rvel;
        float racc;

        if (!cont.accessMasterStop) {
            if (!cont.accessBrake)
            {
                switch (cont.accessCommand)
                {
                    case 0: // stop
                        cont.accessInputData[1] = .0f;
                        cont.accessInputData[2] = .0f;
                        cont.accessInputData[6] = .0f;
                        break;

                    case 1: // Torque
                        (lx, ly) = getLeftJoyStick();
                        //(lx, ly) = convertCoordinateWorld2Local(lx, ly, theta_on_world); // x, y 並進
                        (rx, ry) = getRightJoyStick(); // z まわり旋回
                        rvel = calcRotVelocity(rx, ry);

                        cont.accessInputData[1] = this.gain_forcex * lx;
                        cont.accessInputData[2] = this.gain_forcey * ly;
                        cont.accessInputData[6] = this.gain_torqz * rvel;
                        break;

                    case 2: // velocity
                        (lx, ly) = getLeftJoyStick(); // x, y 並進
                        //(lx, ly) = convertCoordinateWorld2Local(lx, ly, theta_on_world);
                        (rx, ry) = getRightJoyStick(); // z まわり旋回
                        rvel = calcRotVelocity(rx, ry);

                        cont.accessInputData[1] = this.gain_velx * lx;
                        cont.accessInputData[2] = this.gain_vely * ly;
                        cont.accessInputData[6] = this.gain_rvelz * rvel;
                        break;

                    case 3: // UDP Connection Test
                        (lx, ly) = getLeftJoyStick(); // x, y 並進
                        (rx, ry) = getRightJoyStick(); // z まわり旋回

                        cont.accessInputData[1] = .0f;
                        cont.accessInputData[2] = .0f;
                        cont.accessInputData[6] = .0f;
                        break;

                    case 4: // other
                        (lx, ly) = getLeftJoyStick(); // x, y 並進
                        (rx, ry) = getRightJoyStick(); // z まわり旋回

                        cont.accessInputData[1] = .0f;
                        cont.accessInputData[2] = .0f;
                        cont.accessInputData[6] = .0f;
                        break;

                    default:
                        cont.accessInputData[1] = .0f;
                        cont.accessInputData[2] = .0f;
                        cont.accessInputData[6] = .0f;
                        break;
                }
            }
        }
    }

    private (float, float) getLeftJoyStick()
    {
        return (LeftJoystick.Horizontal, LeftJoystick.Vertical);
    }

    private (float, float) getRightJoyStick()
    {
        return (RightJoystick.Horizontal, RightJoystick.Vertical);
    }

    private (float, float) convertCoordinateWorld2Local(float wx, float wy, float wtheta)
    {
        //return (-wx * Mathf.Cos(wtheta), -wy * Mathf.Sin(wtheta));
        return (wx*Mathf.Cos(wtheta) + wy*Mathf.Sin(wtheta), -wx*(Mathf.Sin(wtheta)) + wy*Mathf.Cos(wtheta));
    }

    private (float, float) calcLStickDiff(float lx, float ly)
    {
        float diff_x = lx - x_pre;
        float diff_y = ly - y_pre;

        x_pre = lx;
        y_pre = ly;

        return (diff_x, diff_y);
    }

    private float calcRotation(float rx, float ry)
    {
        float theta = Mathf.Atan2(rx, ry);
        return Angle2Round(theta * (float)(180f / Math.PI));
    }

    private float Angle2Round(float theta) // (0 -> 180, -0 -> -180 ) to 0 -> 360
    {
        const float thr_round = 180f; //1回転したことを判断するための閾値
        if (theta < 0) {
            theta += 360f;
        }
        if (Mathf.Abs(theta - theta_pre) > thr_round) {
            if (theta >= 0) count_round++;
            else count_round--;
        }
        return theta + (360f * count_round);
    }

    private float calcRotVelocity(float rx, float ry)
    {
        float theta_stick = Mathf.Atan2(rx, ry);
        if (theta_stick >= 0)
            return -Mathf.Sqrt(rx * rx + ry * ry);
        else
            return Mathf.Sqrt(rx * rx + ry * ry);
    }

    private float calcRotAcceseleration(float rv)
    {
        return rv - rv_pre;
    }

    private string readConfigText()//設定テキストの読み込み ( /asset/config/UDPConfig.txt)
    {
        string text = "";
        FileInfo udp_config = new FileInfo(Application.dataPath + "/" + "config" + "/" + "GainConfig.txt"); //udp設定テキストファイルの読み取り

        try {
            using (StreamReader text_conf = new StreamReader(udp_config.OpenRead(), Encoding.UTF8)) //using の使用でDisposeの記述を省略
            {
                text = text_conf.ReadToEnd();
            }
        }
        catch (Exception e) {
            text += "\n"; //改行
        }

        return text;
    }

    private void splitConfigText(string text)//テキストの分割処理
    {
        string[] sep = { "\r\n" }; //セパレータ（改行コード）
        string[] str = text.Split(sep, StringSplitOptions.None); //分割処理

        this.gain_forcex = float.Parse(str[0]);
        this.gain_forcey = float.Parse(str[1]);
        this.gain_torqz = float.Parse(str[2]);
        this.gain_velx = float.Parse(str[3]);
        this.gain_vely = float.Parse(str[4]);
        this.gain_rvelz = float.Parse(str[5]);
    }
}
