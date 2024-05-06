using System.Collections;
using UnityEngine;

public class Credit : MonoBehaviour
{
    [SerializeField] private GameObject defeatParticle;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Collider creditCollider;
    [SerializeField] private AudioClip dieSE;
    [SerializeField] private Collider searchedCollider;
    private AudioSource dieSource;
    private CreditScore creditScore;
    private SearchTarget searchTarget;
    private WaitForSeconds defeatDelay = new WaitForSeconds(3);

    private void Awake()
    {
        dieSource = GameObject.FindGameObjectWithTag("CreditSE").GetComponent<AudioSource>();
        creditScore = GameObject.FindGameObjectWithTag("CreditScore").GetComponent<CreditScore>();
        searchTarget = GameObject.FindGameObjectWithTag("Searcher").GetComponent<SearchTarget>();
    }

    private void OnEnable()
    {
        if (!meshRenderer.enabled)
            meshRenderer.enabled = true;
        if (!creditCollider.enabled)
            creditCollider.enabled = true;

    }

    public void Defeat()
    {
        StartCoroutine(Hide());
        searchTarget.ClearTarget(searchedCollider.transform);
    }
    IEnumerator Hide()
    {
        meshRenderer.enabled = false;
        creditCollider.enabled = false;
        defeatParticle.SetActive(true);
        creditScore.AddScore();
        dieSource.PlayOneShot(dieSE);
        yield return defeatDelay;
        gameObject.SetActive(false);
    }
}
