using UnityEditor;
using UnityEngine;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

public class AttPermissionRequest : MonoBehaviour
{
    private void Awake()
    {
        if (PlayerPrefs.GetInt("PlayCount", 0) > 0) return;
#if UNITY_IOS
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif
    }
}
