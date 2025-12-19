using UnityEngine;

public class CameraFollowBody : MonoBehaviour
{
    public Transform playerBody; // 物理玩家
    public Transform xrOrigin;   // XR Origin 根对象

    void LateUpdate()
    {
        if (playerBody != null && xrOrigin != null)
        {
            // 只同步水平位置
            Vector3 pos = playerBody.position;
            pos.y = xrOrigin.position.y; // 保持相机的高度
            xrOrigin.position = pos;
        }
    }
}
