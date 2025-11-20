using System.Collections;
using UnityEngine;

public class Token : MonoBehaviour
{
    [HideInInspector] public GameManager gameManager;
    public MeshRenderer mr;
    public bool isRevealed = false;
    public bool isMatched = false;

    private bool isFlipping = false;

    void Start()
    {
        if (gameManager == null)
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    void OnMouseDown()
    {
        if (!isFlipping && !isMatched)
            gameManager.TokenPressed(gameObject.name);
    }

    public void ShowToken()
    {
        if (!isRevealed)
            StartCoroutine(FlipTo(true));
    }

    public void HideToken()
    {
        if (isRevealed)
            StartCoroutine(FlipTo(false));
    }

    public void MatchToken()
    {
        isMatched = true;
        StartCoroutine(Disappear());
    }

    private IEnumerator FlipTo(bool show)
    {
        isFlipping = true;
        float duration = 0.3f;
        float time = 0f;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = show ? Quaternion.Euler(180, 0, 0) : Quaternion.identity;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRot, endRot, time / duration);
            yield return null;
        }

        transform.rotation = endRot;
        isRevealed = show;
        isFlipping = false;
    }

    private IEnumerator Disappear()
    {
        float t = 0f;
        float duration = 0.5f;
        Vector3 start = transform.localScale;
        Vector3 end = Vector3.zero;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }
        Destroy(gameObject);
    }
}