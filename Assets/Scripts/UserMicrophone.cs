using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserMicrophone : MonoBehaviour
{
    private float lastTime = 0;                                     //开始录制的时间
    private int lengthClip = 120;                                   //录音最长时间（秒）
    private AudioSource sound;
    public Text describeText;
    private bool isHavaMicrophone = false;                          //是否有麦克风
    private string[] device;                                        //麦克风设备
    private float minRecordTime = 0.5f;                             //最少录音时间
    private float recordTime = 0.0f;                                //录音时间
    private Coroutine playSoundCor = null;                          //录音播放协程

    // Start is called before the first frame update
    void Start()
    {
        //获取组件 获取麦克风设备
        sound = GetComponent<AudioSource>();
        device = Microphone.devices;

        if (device.Length > 0)
        {
            isHavaMicrophone = true;
            describeText.text = "请按下录音按钮进行录音";
        }
        else
            describeText.text = "您当前设备没有麦克风，无法进行录音";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //开始录制
    public void OnClickStart()
    {
        //判断是否含有麦克风设备
        if(isHavaMicrophone)
        {
            //如果还有录音在播放，停止录音的播放，停止协程
            if(sound.isPlaying)
            {
                sound.Stop();
                StopCoroutine(playSoundCor);
                describeText.text = "播放已停止，请再次按下录音";
                return;
            }
            //开始进行录音
            lastTime = Time.time;
            describeText.text = "录音中...";
            sound.clip = Microphone.Start(device[0], false, lengthClip, 44100);
        }
    }

    //结束录制
    public void OnClickEnd()
    {
        if (isHavaMicrophone)
        {
            Microphone.End(device[0]);
            //获取录音时间
            recordTime = Time.time - lastTime;
            if (recordTime > minRecordTime)
                describeText.text = "按下播放按钮进行播放";
            else
            {
                describeText.text = "录音时间过短，请重试";
                sound.clip = null;
            }
        }
    }
    //播放录制的音乐
    public void OnClickPlay()
    {
        //判断是否有麦克风设备，是否正在录音，如果不在录音则开始播放
        if (isHavaMicrophone && !Microphone.IsRecording(device[0]))
        {
            //如果播放协程进行中，先终止这个协程
            if(playSoundCor != null)
                StopCoroutine(playSoundCor);
            //判断是否已经录音
            if (sound.clip != null)
                playSoundCor = StartCoroutine(PlaySound());               //调用播放录音协程方法
            else
                describeText.text = "请首先进行录音";
        }
    }
    //播放音乐协程方法
    IEnumerator PlaySound()
    {
        sound.Play();
        describeText.text = "播放中...";
        yield return new WaitForSeconds(recordTime);
        sound.Stop();
        describeText.text = "请按下录音按钮进行录音";
    }
}
