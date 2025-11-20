using System.Collections;
using UnityEngine;

public class Token : MonoBehaviour
{
    private GameManager gameManager;
    public MeshRenderer mr;
    public Animator animator;

    private bool isOpened = false;

    void Awake()
    {
        if (mr == null)
        {
            mr = GetComponent<MeshRenderer>();
            if (mr == null)
                mr = GetComponentInChildren<MeshRenderer>();
        }

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Start()
    {
        GameObject o = GameObject.FindGameObjectWithTag("GameManager");
        if (o != null)
            gameManager = o.GetComponent<GameManager>();
    }

    void OnMouseDown()
    {
        gameManager.TokenPressed(gameObject.name);
    }

    public void ShowToken()
    {
        if (!isOpened)
        {
            isOpened = true;
            animator.SetBool("isOpen", true);
        }
    }

    public void HideToken()
    {
        if (isOpened)
        {
            isOpened = false;
            animator.SetBool("isOpen", false);
        }
    }

    public void MatchToken()
    {
        Destroy(gameObject);
    }
}