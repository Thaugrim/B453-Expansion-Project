using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Transform cam;

    private bool isInMainMenu = true;

    private GameObject pnlStart;

    [SerializeField] private bool isDeveloper;

    private void Start()
    {
        pnlStart = GameObject.Find("pnlStart");

        isInMainMenu = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!isInMainMenu) GoTo_MainMenu();
        }
    }

    void GoTo_SelectDifficulty() {
        LeanTween.move(pnlStart, new Vector3(240, -300, 0), 2.5f);
        StartCoroutine(SetMainMenu());
    }
    
    void GoTo_MainMenu() {
        LeanTween.move(pnlStart, new Vector3(240, 120, 0), 2.5f);
        StartCoroutine(SetMainMenu());
    }

    IEnumerator SetMainMenu()
    {
        yield return new WaitForSeconds(2.51f);
        isInMainMenu = !isInMainMenu;
    }
    
    public void SetDifficultyLevel(string difficulty) {
        DifficultyLevel.Instance.difficulty = (DifficultyLevel.Difficulty)Enum.Parse(typeof(DifficultyLevel.Difficulty), difficulty);
        SceneManager.LoadScene(1);
        Destroy(this);
    }

    public void OpenPanel(string nome)
    {
        if(nome == "Difficulty")
        {
            GoTo_SelectDifficulty();
        }
    }
    public void ClosePanels()
    {
        isInMainMenu = false;
    }
}
