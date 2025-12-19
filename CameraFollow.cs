using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Player物体
    public Vector3 offset = new Vector3(0, 1.6f, 0); // 眼睛位置偏移

    [Header("Follow Settings")]
    public float followSpeed = 10f; // 跟随速度
    public bool smoothFollow = true; // 是否平滑跟随
    public bool lockToTarget = true; // 是否锁定到目标

    [Header("Look Settings")]
    public float mouseSensitivity = 2f; // 鼠标灵敏度
    public float maxLookAngle = 80f; // 最大仰角
    public bool invertY = false; // 是否反转Y轴

    private float rotationX = 0f;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start()
    {
        // 如果没有设置目标，尝试自动查找Player
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log($"自动绑定到Player: {player.name}");
            }
            else
            {
                Debug.LogError("找不到Player物体！请设置Target或给Player添加'Player'标签。");
                return;
            }
        }

        // 设置初始位置
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = target.rotation;
        }

        // 锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (target == null) return;

        HandleMouseLook();
        HandleFollow();
    }

    void HandleMouseLook()
    {
        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 水平旋转（旋转Player）
        if (lockToTarget)
        {
            target.Rotate(0, mouseX, 0);
        }
        else
        {
            transform.Rotate(0, mouseX, 0);
        }

        // 垂直旋转（只旋转相机）
        if (invertY)
            mouseY = -mouseY;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxLookAngle, maxLookAngle);

        // 应用垂直旋转到相机
        Vector3 cameraRotation = transform.localEulerAngles;
        cameraRotation.x = rotationX;
        transform.localEulerAngles = cameraRotation;
    }

    void HandleFollow()
    {
        // 计算目标位置
        targetPosition = target.position + offset;

        if (smoothFollow)
        {
            // 平滑跟随
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        else
        {
            // 直接跟随
            transform.position = targetPosition;
        }

        // 如果锁定到目标，相机会跟随Player的旋转
        if (lockToTarget)
        {
            targetRotation = Quaternion.Euler(rotationX, target.eulerAngles.y, 0);
            transform.rotation = targetRotation;
        }
    }

    // 公共方法：设置目标
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            Debug.Log($"相机目标设置为: {target.name}");
        }
    }

    // 公共方法：设置偏移
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
        Debug.Log($"相机偏移设置为: {offset}");
    }

    // 公共方法：切换跟随模式
    public void ToggleFollowMode()
    {
        lockToTarget = !lockToTarget;
        Debug.Log($"跟随模式: {(lockToTarget ? "锁定到目标" : "独立控制")}");
    }

    // 公共方法：重置相机位置
    public void ResetCameraPosition()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            rotationX = 0f;
            Debug.Log("相机位置已重置");
        }
    }

    // 可视化
    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(target.position + offset, 0.1f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(target.position, target.position + offset);
        }
    }
}