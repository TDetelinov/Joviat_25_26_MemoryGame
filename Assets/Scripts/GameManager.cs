using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject prefabToken;
    public int rows = 4;
    public int cols = 4;
    public float spacing = 2.5f; // Ajusta segons mida fitxa

    public GameObject[,] tokens;
    public Material[] materials;

    private int numTokensOpened;
    private string token1Name;
    private string token2Name;

    private int pairsFound = 0;

    private bool revealUsed = false;
    private bool blockClicks = false;

    [Header("Sound Effects")]
    public AudioClip sfxClick;
    public AudioClip sfxMatch;
    public AudioClip sfxFail;
    public AudioClip sfxWin;
    public AudioClip sfxNewRecord;
    public AudioClip backgroundMusic;

    private AudioSource audioSource;

    [Header("End Panel")]
    public GameObject endPanel;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI attemptsText;
    public TextMeshProUGUI bestScoreText;

    void Start()
    {
        // Ajustar dificultat
        int diff = MainMenuManager.selectedDifficulty;
        switch (diff)
        {
            case 0: rows = 3; cols = 2; break;
            case 1: rows = 4; cols = 2; break;
            case 2: rows = 4; cols = 3; break;
        }

        numTokensOpened = 0;
        tokens = new GameObject[rows, cols];

        // Crear llistat de materials (parelles)
        int totalTokens = rows * cols;
        int numPairs = totalTokens / 2;
        List<Material> listMaterials = new List<Material>();

        for (int i = 0; i < numPairs; i++)
        {
            listMaterials.Add(materials[i]);
            listMaterials.Add(materials[i]);
        }

        // Shuffle
        for (int i = 0; i < listMaterials.Count; i++)
        {
            int rnd = Random.Range(0, listMaterials.Count);
            Material temp = listMaterials[i];
            listMaterials[i] = listMaterials[rnd];
            listMaterials[rnd] = temp;
        }

        // Generar tauler
        Vector3 startPos = new Vector3(
            -((cols - 1) * spacing) / 2f,
            0,
            ((rows - 1) * spacing) / 2f
        );

        int indexM = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector3 pos = startPos + new Vector3(j * spacing, 0, -i * spacing);
                GameObject o = Instantiate(prefabToken, pos, Quaternion.identity, this.transform);
                o.name = $"Token_{i}_{j}";

                // Assignar material i animador
                Token t = o.GetComponent<Token>();
                t.mr.material = listMaterials[indexM];
                indexM++;

                tokens[i, j] = o;
            }
        }

        // Inici cronòmetre
        HUDManager.instance.StartTimer();
        audioSource = GetComponent<AudioSource>();
        
        // Música de fons
        AudioSource musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = 0.4f;
        musicSource.Play();
    }

    public void TokenPressed(string name)
    {
        if (blockClicks) return;

        if (numTokensOpened < 2)
        {
            if (numTokensOpened == 0)
            {
                token1Name = name;
            }
            else if (numTokensOpened == 1)
            {
                if (token1Name == name) return;
                token2Name = name;
            }

            Token token = GetTokenByName(name);
            token.ShowToken();
            audioSource.PlayOneShot(sfxClick);

            numTokensOpened++;
        }

        if (numTokensOpened == 2)
        {
            HUDManager.instance.AddAttempt();
            Invoke("CheckTokens", 1.2f); // temps perquè es vegi el gir
            numTokensOpened = 3;
        }
    }

    private Token GetTokenByName(string name)
    {
        string[] parts = name.Split('_');
        int i = int.Parse(parts[1]);
        int j = int.Parse(parts[2]);
        return tokens[i, j].GetComponent<Token>();
    }

    private void CheckTokens()
    {
        Token t1 = GetTokenByName(token1Name);
        Token t2 = GetTokenByName(token2Name);

        if (t1.mr.material.name == t2.mr.material.name)
        {
            t1.MatchToken();
            t2.MatchToken();
            audioSource.PlayOneShot(sfxMatch);
            pairsFound++;

            if (pairsFound == (rows * cols) / 2)
                EndGame();
        }
        else
        {
            t1.HideToken();
            t2.HideToken();
            audioSource.PlayOneShot(sfxFail);
        }

        numTokensOpened = 0;
    }

    private void EndGame()
    {
        HUDManager.instance.StopTimer();
        float currentTime = HUDManager.instance.GetTime();
        float bestBefore = HUDManager.instance.GetBestScore();
        HUDManager.instance.SaveBestScore(currentTime);

        if (currentTime < bestBefore)
            audioSource.PlayOneShot(sfxNewRecord);
        else
            audioSource.PlayOneShot(sfxWin);

        // Panell final
        if (endPanel != null)
        {
            endPanel.SetActive(true);
            timeText.text = "Time: " + currentTime.ToString("F2") + "s";
            attemptsText.text = "Attempts: " + HUDManager.instance.GetAttempts();
            bestScoreText.text = "Best: " + HUDManager.instance.GetBestScore().ToString("F2") + "s";
        }

        blockClicks = true;
    }

    // Reveal All (una vegada per partida)
    private IEnumerator RevealCoroutine()
    {
        blockClicks = true;
        foreach (GameObject t in tokens)
        {
            if (t != null)
                t.GetComponent<Token>().ShowToken();
        }
        yield return new WaitForSeconds(1f);
        foreach (GameObject t in tokens)
        {
            if (t != null)
                t.GetComponent<Token>().HideToken();
        }
        blockClicks = false;
    }

    public void RevealAll()
    {
        if (revealUsed) return;
        revealUsed = true;
        StartCoroutine(RevealCoroutine());
    }
    
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
