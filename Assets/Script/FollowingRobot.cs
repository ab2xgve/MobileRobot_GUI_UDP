using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingRobot : MonoBehaviour
{ 
    private GameObject robot;   //プレイヤー情報格納用
    private Vector3 offset;      //相対距離取得用

    // Use this for initialization
    void Start()
    {

        //robotの情報を取得
        this.robot = GameObject.Find("Display/Robot/Robot Marker");

        // MainCamera(自分自身)とplayerとの相対距離を求める
        this.offset = transform.position - robot.transform.position;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        //新しいトランスフォームの値を代入する
        transform.position = this.robot.transform.position + this.offset;

    }
}
