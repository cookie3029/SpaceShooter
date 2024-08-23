using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 게임 개발 디자인 패턴 
    // 싱글턴 디자인 패턴 (Singlton Desing Pattern)
    public static GameManager Instance = null;

    public TMP_Text scoreText;

    public List<Transform> points = new List<Transform>();

    public GameObject monsterPrefab;

    private bool isGameOver = false;

    // 오브젝트 풀 정의(선언)
    public List<GameObject> monsterPool = new List<GameObject>();

    // 오브젝트 풀 갯수
    public int maxPool = 10;

    private int score = 0;

    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score += value;
            PlayerPrefs.SetInt("SCORE", score);
            scoreText.text = $"SCORE : {score:0000000}";
        }
    }

    // 프로퍼티 선언
    public bool IsGameOver
    {
        get
        {
            return isGameOver;
        }
        set
        {
            isGameOver = value;
            // if (isGameOver)
            // {
            //     Debug.Log("게임 종료");
            //     // 엔딩 타이틀 UI 표현
            //     CancelInvoke(nameof(CreateMonster));
            // }
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 다른 씬이 오픈되어도 지속하도록하는 메소드
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this)
        {
            // 중복해서 생성된 GameManager 삭제
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        score = PlayerPrefs.GetInt("SCORE", 0);
        scoreText.text = $"SCORE : {score:0000000}";

        var spawnPointGroup = GameObject.Find("SpawnPointGroup");
        spawnPointGroup.GetComponentsInChildren<Transform>(points);

        CreateMonsterPool();

        // InvokeRepeating(nameof(CreateMonster), 2.0f, 3.0f);
        StartCoroutine(CreateMonster());
    }

    private void CreateMonsterPool()
    {
        for (int i = 0; i < maxPool; i++)
        {
            GameObject monster = Instantiate(monsterPrefab);
            monster.name = $"Monster_{i:00}";
            monster.SetActive(false);
            monsterPool.Add(monster);
        }
    }

    IEnumerator CreateMonster()
    {
        yield return new WaitForSeconds(2.0f);

        while (!isGameOver)
        {
            // 난수 발생
            int index = UnityEngine.Random.Range(1, points.Count);

            foreach (var monster in monsterPool)
            {
                if (monster.activeSelf == false)
                {
                    monster.transform.position = points[index].position;
                    monster.SetActive(true);
                    break;
                }
            }

            // Instantiate(monsterPrefab, points[index].position, Quaternion.identity);

            yield return new WaitForSeconds(3.0f);
        }
    }

}
