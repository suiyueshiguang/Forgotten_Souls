using TMPro;
using UnityEngine;

public class Blackhole_HotKey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;
    private TextMeshProUGUI myTest;

    private Transform myEnemy;
    private Blackhole_Skill_Controller blackhole;

    public void SetupHotKey(KeyCode _myNewHotKey, Transform _myEnemy, Blackhole_Skill_Controller _myBlackhole)
    {
        sr = GetComponent<SpriteRenderer>();
        myHotKey = _myNewHotKey;
        myEnemy = _myEnemy;
        blackhole = _myBlackhole;

        myTest = GetComponentInChildren<TextMeshProUGUI>();
        myTest.text = myHotKey.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(myHotKey))
        {
            blackhole.AddEnemyToList(myEnemy);

            myTest.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
