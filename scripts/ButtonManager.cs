using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button startButton;    // 开始游戏按钮
    [SerializeField] private Button continueButton; // 继续游戏按钮
    [SerializeField] private AudioSource bgMusic;
    [SerializeField] private GameObject pausePanel; // 暂停面板
    [SerializeField] private FloorManager floorManager;  // 注意这里的变量名是floorManager
    [SerializeField] private Floors floors;
    private bool isPaused = false;                  // 暂停状态

    void Start()
    {
        // 添加按钮点击事件监听
        startButton.onClick.AddListener(() => {
            Debug.Log("开始按钮被点击");
            OnStartClick();
        });
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartClick);
        }
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClick);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // 如果继续游戏按钮存在且可见，优先触发继续游戏
            if (continueButton != null && continueButton.gameObject.activeSelf)
            {
                continueButton.onClick.Invoke();
            }
            // 否则如果开始游戏按钮存在且可见，触发开始游戏
            else if (startButton != null && startButton.gameObject.activeSelf)
            {
                startButton.onClick.Invoke();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 如果继续按钮处于激活状态，直接返回
            /*if (continueButton != null && (continueButton.gameObject.activeSelf || startButton.gameObject.activeSelf))
            {
                Debug.Log("开始/继续按钮激活中，空格键禁用");
                return;
            }*/

            Debug.Log("触发暂停/继续");
            TogglePause();
        }
    }
    // 添加开始游戏按钮点击事件处理
    public void OnStartClick()
    {
        Debug.Log("点击开始游戏按钮");

        if (floorManager != null)
        {
            floorManager.ResetSpeed();
        }
        else
        {
            Debug.LogError("FloorManager reference is missing!");
        }

        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);  // 隐藏开始按钮
            Time.timeScale = 1f;  // 开始游戏
            //MusicManager.PlayMusic();  // 播放音乐
        }
    }
    public void OnPlayerDeath()
    {
        Debug.Log("ButtonManager收到死亡通知");
        Time.timeScale = 0f;
    }
    // 继续按钮点击事件处理
    public void OnContinueClick()
    {
        // 在这里确保继续按钮被隐藏
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
            Debug.Log("继续按钮已隐藏");  // 添加调试日志
        }

        if (floorManager != null)
        {
            floorManager.ResetSpeed();
            Debug.Log("速度已重置为初始值");
        }
        else
        {
            Debug.LogError("FloorManager reference is missing!");
        }

        // 设置继续游戏标志
        PlayerPrefs.SetInt("ContinueGame", 1);

        floors.currentMoveSpeed = 1f;

        // 确保开始按钮不会再显示
        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
        }

        TogglePause();  // 如果是暂停状态，调用继续
        MusicManager.ResumeMusic();

        // 在加载新场景前再次确认按钮状态
        Debug.Log($"加载场景前继续按钮状态: {continueButton.gameObject.activeSelf}");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TogglePause()
    {
        // 如果继续按钮激活，不允许暂停
        if (continueButton != null && (continueButton.gameObject.activeSelf || startButton.gameObject.activeSelf))
        {
            Debug.Log($"继续按钮状态: {continueButton.gameObject.activeSelf}");
            Debug.Log($"开始按钮状态: {startButton.gameObject.activeSelf}");
            Debug.Log("开始/继续按钮激活中，不能暂停");
            return;
        }

        isPaused = !isPaused;
        Debug.Log("暂停状态: " + isPaused);  // 添加调试日志

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);  // 显示/隐藏暂停面板
        }
        // 使用MusicManager的静态方法控制音乐
        if (isPaused)
        {
            Debug.Log("尝试暂停音乐");  // 添加调试日志
            MusicManager.PauseMusic();
            Time.timeScale = 0f;
        }
        else
        {
            Debug.Log("尝试恢复音乐");  // 添加调试日志
            MusicManager.ResumeMusic();
            Time.timeScale = 1f;
        }

        // 暂停/继续游戏
        Time.timeScale = isPaused ? 0f : 1f;
        Debug.Log(isPaused ? "游戏暂停" : "游戏继续");

        // 暂停/继续音乐
        if (bgMusic != null)
        {
            if (isPaused)
                bgMusic.Pause();
            else
                bgMusic.UnPause();
        }
    }
}
