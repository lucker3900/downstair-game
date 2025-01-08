using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance = null;
    [SerializeField] private AudioSource audioSource;  // 音频组件
    [SerializeField] private AudioClip backgroundMusic;  // 背景音乐文件
    [SerializeField] private float musicVolume = 0.5f;  // 音量大小
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        // 获取或添加AudioSource组件
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            // 设置音频
            audioSource.clip = backgroundMusic;
            audioSource.volume = musicVolume;
            audioSource.loop = true;  // 循环播放

            // 开始播放
            audioSource.Play();
            Debug.Log("音乐管理器初始化完成");  // 添加调试日志
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetVolume(float volume)
    {
        musicVolume = volume;
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    // 提供静态方法来控制音乐
    public static void PauseMusic()
    {
        if (instance != null && instance.audioSource != null)
            instance.audioSource.Pause();
    }

    public static void ResumeMusic()
    {
        if (instance != null && instance.audioSource != null)
        {
            // 如果音乐已经停止，重新播放
            if (!instance.audioSource.isPlaying)
            {
                instance.audioSource.Play();
            }
            else
            {
                instance.audioSource.UnPause();
            }
            Debug.Log("音乐已恢复");
        }
    }

    public static void StopMusic()
    {
        if (instance != null && instance.audioSource != null)
            instance.audioSource.Stop();
    }
}
