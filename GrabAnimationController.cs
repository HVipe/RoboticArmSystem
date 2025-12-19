using UnityEngine;

public class GrabAnimationController : MonoBehaviour
{
    public GameObject targetObject;

    // 这个方法会在Animation Event中显示
    public void OnGrabStart()
    {
        if (targetObject != null)
        {
            targetObject.transform.SetParent(transform);
            Debug.Log("开始抓取物体");
        }
    }

    // 这个方法也会在Animation Event中显示
    public void OnReleaseObject()
    {
        if (targetObject != null)
        {
            targetObject.transform.SetParent(null);
            Debug.Log("释放物体");
        }
    }
}