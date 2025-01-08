using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // 添加这行来引用Text组件

public class FloorManager : MonoBehaviour
{
    [SerializeField] GameObject[] floorPrefabs;
    [SerializeField] private float minHorizontalGap = 0.1f;
    [SerializeField] private float lastSpawnX = 0f;
    [SerializeField] private float moveSpeed = 1f;
    private float speedIncrease = 0.5f;
    private int floorCount = 0;                    // 当前层数计数

    private float lastSpawnY;
    private Text scoreText;  // 使用现有的score文本组件

    void Start()
    {
        // 强制设置初始速度为1
        moveSpeed = 1f;
        Debug.Log($"FloorManager初始速度已设置为: {moveSpeed}");
    }

    public void SpawnFloor()
    {
        if (floorPrefabs.Length == 0)
        {
            Debug.LogError("Floor prefabs array is empty!");
            return;
        }

        float newX;
        if (lastSpawnX > 0)
        {
            // 如果上一个在右边，这个就往左生成
            newX = Random.Range(-3.8f, -minHorizontalGap);
        }
        else
        {
            // 如果上一个在左边，这个就往右生成
            newX = Random.Range(minHorizontalGap, 3.8f);
        }

        int r = Random.Range(0, floorPrefabs.Length);
        GameObject floor = Instantiate(floorPrefabs[r], transform);
        floor.transform.position = new Vector3(newX, -5.5f, 0f);

        lastSpawnX = newX;
    }
    // 重置速度（在游戏重新开始时调用）
    public void ResetSpeed()
    {
        moveSpeed = 1f;  // 重置为基础速度
        floorCount = 0;  // 重置层数计数
        Debug.Log("速度重置为: " + moveSpeed);

        Debug.Log("速度和层数已重置: " + floorCount);
    }
    public void IncreaseSpeed(float increment)
    {
        moveSpeed += increment;
        Debug.Log($"速度增加到: {moveSpeed}");
    }
    // 获取当前移动速度的方法
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
}
