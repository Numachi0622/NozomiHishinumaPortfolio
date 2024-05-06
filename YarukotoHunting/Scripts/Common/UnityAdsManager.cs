using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Advertisements;
using Unity.Advertisement.IosSupport;

public class UnityAdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
#if UNITY_IOS
    public const string gameID = "5441942";
    public const string interstitialID = "Interstitial_iOS";
#elif UNITY_ANDROID
    public const string gameID = "5441943";
    public const string interstitialID = "Interstitial_Android";
#endif
    private Action<UnityAdsShowCompletionState> finish;
    public static UnityAdsManager Instance { get; private set; }

    private void Start()
    {
#if UNITY_IOS
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED)
        { 
            MetaData gdprMetaData = new MetaData("gdpr");
            gdprMetaData.Set("consent", "true");
            Advertisement.SetMetaData(gdprMetaData);
        }
        else
        {
            MetaData gdprMetaData = new MetaData("gdpr");
            gdprMetaData.Set("consent", "false");
            Advertisement.SetMetaData(gdprMetaData);
        }
#endif
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Advertisement.Initialize(gameID, true, this);
        }
    }

    public void ShowInterstitial(Action<UnityAdsShowCompletionState> _finish)
    {
        Advertisement.Load(interstitialID,this);
        Advertisement.Show(interstitialID,this);
        finish = _finish;
    }

    public void OnInitializationComplete()
    {
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
    }

    public void OnUnityAdsShowStart(string placementId)
    {
    }

    public void OnUnityAdsShowClick(string placementId)
    {
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        finish.Invoke(showCompletionState);
    }
}