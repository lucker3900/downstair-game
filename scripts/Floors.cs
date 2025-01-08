using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floors : MonoBehaviour
{
    [SerializeField] public float initialMoveSpeed = 1f;  // 初始速度设为1
    [SerializeField] float speedIncrement = 0.5f;  // 每20层增加的速度
    public float currentMoveSpeed;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] private float verticalGap = 6f;
    private bool isDestroying = false; // 添加一个布尔变量
    private FloorManager floorManager;
    private float printTimer = 0f;
    private float printInterval = 2f;  // 每2秒打印一次
    // Start is called before the first frame update
    void Start()
    {
        floorManager = FindObjectOfType<FloorManager>();
        currentMoveSpeed = initialMoveSpeed;  // 从1开始

        Debug.Log("初始速度：" + currentMoveSpeed);  // 添加调试日志
    }

    // Update is called once per frame
    void Update()
    {
        // 如果正在销毁，就不再执行移动
        if (isDestroying) return;

        int currentFloor = int.Parse(GameObject.Find("score").GetComponent<UnityEngine.UI.Text>().text.Replace("地下", "").Replace("层", ""));
        // 计算速度阶段（每20层一个阶段）
        int speedStage = currentFloor / 20;
        // 更新当前速度
        currentMoveSpeed = initialMoveSpeed + (speedStage * speedIncrement);
        // 当速度改变时输出日志
        if (currentFloor % 20 == 0 && currentFloor > 0)
        {
            Debug.Log($"到达{currentFloor}层，当前速度：{currentMoveSpeed}");
        }

        if (floorManager != null)
        {
            // 使用FloorManager中的速度
            float currentSpeed = floorManager.GetMoveSpeed();
            transform.Translate(Vector3.up * currentSpeed * Time.deltaTime);
        }

        printTimer += Time.deltaTime;
        if (printTimer >= printInterval)
        {
            Debug.Log($"当前移动速度currentMoveSpeed: {currentMoveSpeed:F2}");
            Debug.Log($"当前移动速度moveSpeed: {moveSpeed:F2}");
            printTimer = 0f;
        }

        if(transform.position.y > 6f && !isDestroying)
        {
            isDestroying = true;
            Destroy(gameObject);
        }

        if (isDestroying)
        {
            StartCoroutine(SpawnFloorsCoroutine());
            isDestroying = false;
        }
    }

    IEnumerator SpawnFloorsCoroutine()
    {
            transform.parent.GetComponent<FloorManager>().SpawnFloor();
            // 计算生成下一块楼梯所需的时间
            // 时间 = verticalGap / moveSpeed
            float waitTime = verticalGap / moveSpeed;
            Debug.Log($"Waiting for {waitTime} seconds before spawning next floor.");
            yield return new WaitForSeconds(waitTime);
    }
}
