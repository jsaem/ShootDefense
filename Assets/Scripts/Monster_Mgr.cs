using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------- SpawnPos
class SpawnPos //벡터로 관리하고 벡터이기 때문에 인덱스를 갖게 된다.
{
    public Vector3 m_Pos = Vector3.zero;
    public float m_SpDelay = 0.0f;
    public int m_Level = 1;  //스폰될 몬스터의 레벨은 증가될 것이다.

    public SpawnPos()
    {
        m_SpDelay = 0.0f;
    }

    public bool Update_SpPos(float a_DeltaTime)
    {
        if (0.0f < m_SpDelay)
        {
            m_SpDelay = m_SpDelay - a_DeltaTime;
            if (m_SpDelay <= 0.0f)
            {
                //메모리풀에서 몬스터 한마리 스폰시키고 몬스터쪽에 스폰 위치 인덱스를 갖게 한다.
                m_SpDelay = 0.0f;
                return true;
            }
        }//if(0.0f < m_SpDelay)

        return false;
    }//void Update_SpPos(float a_DeltaTime)
};
//----------------------------- SpawnPos

public class Monster_Mgr : MonoBehaviour
{
    Transform m_EnemyGroup = null;
    GameObject m_MonPrefab = null;
    List<SpawnPos> m_SpawnPosList = new List<SpawnPos>();

    public static Monster_Mgr RefMonMgr;
    public Texture[] m_MonImg = null;

    // Start is called before the first frame update
    void Start()
    {
        RefMonMgr = this;

        m_EnemyGroup = this.transform;
        m_MonPrefab = Resources.Load("MonsterRoot") as GameObject;

        MonsterCtrl[] m_MonsterList;
        m_MonsterList = this.transform.GetComponentsInChildren<MonsterCtrl>();
        // MonsterCtrl가 붙어있는 애들만 찾기
        for (int a_ii = 0; a_ii < m_MonsterList.Length; a_ii++)
        {
            SpawnPos a_Node = new SpawnPos();
            a_Node.m_Pos = m_MonsterList[a_ii].gameObject.transform.position;
            m_MonsterList[a_ii].m_SpawnIdx = a_ii; //m_SpawnPos; 의 인덱스 셋팅해 준다.
            m_SpawnPosList.Add(a_Node);
        }//for (int a_ii = 0; a_ii < m_MonsterList.Length; a_ii++)       
    }

    // Update is called once per frame
    void Update()
    {
        for (int ii = 0; ii < m_SpawnPosList.Count; ii++)
        {
            if (m_SpawnPosList[ii].Update_SpPos(Time.deltaTime) == false)
                continue;

            //새로 스폰 시킨다.
            GameObject newObj = (GameObject)Instantiate(m_MonPrefab);
            newObj.transform.SetParent(m_EnemyGroup);
            newObj.transform.position = m_SpawnPosList[ii].m_Pos;

            //----------- 레벨에 따른 몬스터 능력치 셋팅
            int a_Lv = m_SpawnPosList[ii].m_Level;
            float a_MaxHP = 100.0f + (m_SpawnPosList[ii].m_Level * 100.0f);
            if (1000.0f < a_MaxHP)
                a_MaxHP = 1000.0f; //100.0f; ~ 1000.0f까지
            float a_AttSpeed = 0.5f - (m_SpawnPosList[ii].m_Level * 0.1f);
            if (a_AttSpeed < 0.1f)
                a_AttSpeed = 0.1f; //공속  0.5f -> 0.1f; 까지
            float a_MvSpeed = 2.0f + (m_SpawnPosList[ii].m_Level * 0.5f); // 2.0f ~ 5.0f;
            if (5.0f < a_MvSpeed)
                a_MvSpeed = 5.0f;  //이속  2.0f ~ 5.0f; 까지
            newObj.GetComponent<MonsterCtrl>().SetSpawnInfo(ii,
                                                a_Lv, a_MaxHP, a_AttSpeed, a_MvSpeed);
            //----------- 레벨에 따른 몬스터 능력치 셋팅

        }//for (int ii = 0; ii < m_SpawnPosList.Count; ii++)
    } // void Update()

    public void ReSetSpawn(int idx)
    {
        if (idx < 0)
            return;

        m_SpawnPosList[idx].m_Level++;
        m_SpawnPosList[idx].m_SpDelay = Random.Range(4.0f, 6.0f); //Random.Range(1.5f, 5.0f);
    }

} //public class Monster_Mgr : MonoBehaviour
