using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    GameObject currentFloor;
    [SerializeField] int Hp;
    [SerializeField] GameObject HpBar;
    [SerializeField] Text scoreText;
    [SerializeField] private FloorManager floorManager;  // 引用FloorManager
    private float speedIncrease = 0.5f;  // 每20层增加的速度
    int score;
    float scoreTime;
    Animator anim;
    SpriteRenderer render;
    AudioSource deathSound;
    [SerializeField] GameObject replayButton;
    // Start is called before the first frame update
    [SerializeField] GameObject startButton; // 修改：将变量名改为正确的声明
    private bool isGameStarted = false; // 修改：添加private修饰符
    private bool isDead = false;  // 添加死亡状态标志
    private ButtonManager buttonManager;

    void Start()
    {
        Hp = 10;
        score = 0;
        scoreTime = 0f;
        anim = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
        deathSound = GetComponent<AudioSource>();
        Time.timeScale = 0f; // 游戏开始时暂停
        startButton.SetActive(true); // 显示开始按钮

        floorManager = FindObjectOfType<FloorManager>();
        if (floorManager == null)
        {
            Debug.LogWarning("FloorManager 引用为空! 请确保场景中有FloorManager对象");

            // 可以尝试在场景中查找特定对象
            GameObject floorManagerObj = GameObject.Find("FloorManager");
            if (floorManagerObj != null)
            {
                floorManager = floorManagerObj.GetComponent<FloorManager>();
            }
        }
        else
        {
            Debug.Log("成功获取FloorManager引用");
        }
        // 在开始时找到ButtonManager
        buttonManager = FindObjectOfType<ButtonManager>();
        if (buttonManager == null)
        {
            Debug.LogError("场景中找不到ButtonManager!");
        }
        // 检查是否是从继续游戏进入的
        if (PlayerPrefs.GetInt("ContinueGame", 0) == 1)
        {
            // 如果是继续游戏，直接开始
            isGameStarted = true;
            Debug.Log("从继续游戏进入");  // 确认进入此分支
            if (startButton != null)
            {
                startButton.SetActive(false);
            }
            else
            {
                Debug.LogError("startButton is null!");  // 检查按钮引用是否存在
            }
            Time.timeScale = 1f;
            // 重置标志
            PlayerPrefs.SetInt("ContinueGame", 0);
            Debug.Log("重置ContinueGame标记为0");
        }
        else
        {
            // 如果是新游戏，显示开始按钮
            isGameStarted = false;
            if (startButton != null)
            {
                startButton.SetActive(true);
            }
            Time.timeScale = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameStarted) return;

        // 如果开始按钮是显示状态，空格键无效
        if (startButton != null && startButton.activeSelf)
        {
            return;  // 直接返回，不处理任何输入
        }

        if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Translate(moveSpeed*Time.deltaTime,0,0);
            render.flipX = false;
            anim.SetBool("run",true);
        }
        else if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Translate(-moveSpeed*Time.deltaTime,0,0);
            render.flipX = true;
            anim.SetBool("run",true);
        }
        else
        {
            anim.SetBool("run",false);
        }
        UpdateScore();

        if(Input.GetKey(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            startButton.GetComponent<Button>().onClick.Invoke();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Normal")
        {
            if(other.contacts[0].normal == new Vector2(0f,1f))
            {
                currentFloor = other.gameObject;
                modifyHp(1);
                other.gameObject.GetComponent<AudioSource>().Play();
            }
        }
        else if(other.gameObject.tag == "Nails")
        {
            if(other.contacts[0].normal == new Vector2(0f,1f))
            {
                currentFloor = other.gameObject;
                modifyHp(-3);
                anim.SetTrigger("hurt");
                other.gameObject.GetComponent<AudioSource>().Play();
            }

        }
        else if(other.gameObject.tag == "Ceiling")
        {
            currentFloor.GetComponent<BoxCollider2D>().enabled = false;
            Debug.Log("碰到顶了!");
            modifyHp(-3);
            anim.SetTrigger("hurt");
            other.gameObject.GetComponent<AudioSource>().Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("DeathLine"))
        {
            if (buttonManager != null)
            {
                buttonManager.OnPlayerDeath();
            }
            else
            {
                Debug.LogError("ButtonManager reference is missing!");
            }

            isDead = true;  // 设置死亡状态
            isGameStarted = false;  // 游戏结束
            Time.timeScale = 0f;

            Debug.Log("玩家死亡,isDead = " + isDead);
            Debug.Log("你输了!");
            Die();
        }
    }

    void modifyHp(int num)
    {
        Hp += num;
        if(Hp > 10)
        {
            Hp = 10;
        }
        else if(Hp <= 0)
        {
            Hp = 0;
            Die();
        }
        UpdateHpBar();
    }

    void UpdateHpBar()
    {
        for(int i=0; i<HpBar.transform.childCount; i++)
        {
            if(Hp>i)
            {
                HpBar.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                HpBar.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    void UpdateScore()
    {
        scoreTime += Time.deltaTime;
        if(scoreTime>2f)
        {
            score++;
            scoreTime = 0f;
            // 添加空检查
            if (scoreText != null)
            {
                scoreText.text = "地下" + score.ToString() + "层";
            }

            if (floorManager != null)
            {
            // 每20层增加速度
                if (score % 20 == 0)
                {
                    floorManager.IncreaseSpeed(speedIncrease);  // 调用FloorManager中的方法增加速度
                    Debug.Log($"第{score}层，速度增加{speedIncrease}");
                }
            }
            else
            {
                Debug.LogWarning("FloorManager 引用为空！");
            }

            // 可以添加日志来确认层数
            Debug.Log($"当前UI显示层数: {score}");
        }
    }

    void Die()
    {
        deathSound.Play();
        Time.timeScale = 0f;
        replayButton.SetActive(true);
        MusicManager.StopMusic();
    }

    public void Replay()
    {
        Time.timeScale = 1f;

        // 确保音乐播放
        MusicManager.ResumeMusic();

        SceneManager.LoadScene("SampleScene");
        // 直接开始游戏，不显示开始按钮
        isGameStarted = true;
        // 设置继续游戏标志
        PlayerPrefs.SetInt("ContinueGame", 1);
        // 加载场景
        SceneManager.LoadScene("SampleScene");

        Floors floors = FindObjectOfType<Floors>();
        floors.currentMoveSpeed = 1f;  // 重置为基础速度

        moveSpeed = 1f;

        startButton.SetActive(false);
    }

    public void StartGame()
    {
        Debug.Log($"StartGame被调用,调用者:{gameObject.name}");
        isGameStarted = true;
        Time.timeScale = 1f;
        if (startButton != null)
        {
           Debug.Log($"正在隐藏按钮：{startButton.name}");
           startButton.SetActive(false);
        }
        // 如果音乐被停止了，重新开始播放
        MusicManager.ResumeMusic();
    }

    public void OnContinueGameClick()
    {
        // 设置继续游戏标志
        PlayerPrefs.SetInt("ContinueGame", 1);
        // 加载场景
        SceneManager.LoadScene("SampleScene");
    }
}
