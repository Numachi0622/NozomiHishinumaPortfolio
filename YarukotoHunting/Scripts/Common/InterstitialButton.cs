using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InterstitialButton : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private bool isTutorial;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            var networkState = Application.internetReachability;
            if(networkState == NetworkReachability.NotReachable)
            {
                Restart();
                return;
            }
            UnityAdsManager.Instance.ShowInterstitial(result =>
            {
                Restart();
            });
        });
    }

    private void Restart()
    {
        playerStatus.ContinueSetting();
        if (isTutorial)
            SceneManager.LoadScene("Tutorial");
        else
            SceneManager.LoadScene("Game");
    }

}
