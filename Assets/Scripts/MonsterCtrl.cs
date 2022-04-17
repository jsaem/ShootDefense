using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonAIState
{
    MAI_Idle,          //숨쉬기 상태
    MAI_Patrol,        //패트롤 상태
    MAI_AggroTrace,    //적으로부터 공격을 당했을 때 추적 상태
    MAI_NormalTrace,   //일반 추적 상태
    MAI_ReturnPos,     //추적 놓쳤을 때 재자리로 돌아오는 상태
    MAI_Attack,        //공격 상태
}

public class MonsterCtrl : MonoBehaviour
{
    float m_MaxHP = 100.0f;
    [HideInInspector] public float m_CurHP = 100.0f;
    public Image m_HPSdBar = null; //using UnityEngine.UI; 필요

    //---------- 몬스터 AI 변수들...
    MonAIState m_AIState = MonAIState.MAI_Patrol;  //상태변수

    GameObject m_AggroTarget = null;        //추적해야할 타켓 캐릭터(주인공)
    Vector3 a_CacVLen = Vector3.zero;       //계산용 벡터
    float a_CacDist = 0.0f;                 //거리 계산용 변수

    float m_AttackDist = 19.5f; //15.0f;    //공격거리
    float m_TraceDist  = 20.0f;             //추적거리

    float m_MoveVelocity = 2.0f;            //평면 초당 이동 속도...

    Vector3 m_MoveDir      = Vector3.zero;  //평면 진행 방향
    Vector3 a_MoveNextStep = Vector3.zero;  //이동 계산용 변수
    float m_NowStep = 0.0f;                 //이동 계산용 변수

    //Patrol시 계산용 변수
    Vector3 m_BasePos = Vector3.zero;  //몬스터의 초기 스폰 위치(기준점이 된다.)
    bool  m_bMvPtOnOff = false;        //Patrol Move OnOff
    float m_WaitTime = 0.0f; //Patrol시에 목표점에 도착하면 잠시 대기시키기 위한 랜덤 시간변수

    float m_CacNowStep = 0.0f;              //Patrol시 이동 보폭 계산용 변수
    Vector3 m_PatrolTarget = Vector3.zero;  //Patrol시 움직여야될 다음 목표 좌표
    Vector3 m_CacEndVec;
    Vector3 m_DirMvVec = Vector3.zero;      //Patrol시 움직여야될 방향 벡터
    double m_MoveDurTime = 0.0;             //목표점까지 도착하는데 걸리는 시간
    double m_AddTimeCount = 0.0;            //누적시간 카운트 

    Vector3 a_DirPtVec = Vector3.zero;
    Quaternion a_CacPtRot;
    Vector3 a_CacPtAngle = Vector3.zero;
    Vector3 a_Vect;
    int     a_AngleRan;
    int     a_LengthRan;
    //Patrol시 계산용 변수
    //---------- 몬스터 AI 변수들...

    float m_ShootCool = 1.0f;   //공격 쿨타임 (공격 주기)
    float m_AttackSpeed = 0.5f; //공속

    [HideInInspector] public int m_SpawnIdx = -1;  //vector<SpawnPos>  m_SpawnPos;의 인덱스
    int m_Level = 1;    //몬스터 레벨

    // Start is called before the first frame update
    void Start()
    {
        m_MaxHP = 100.0f;
        m_CurHP = 100.0f;

        m_BasePos = this.transform.position;
        m_bMvPtOnOff = false; //Patrol Move
        m_WaitTime = Random.Range(0.5f, 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (a_isSetLevel == true)
        {
            m_MaxHP = a_BU_MaxHP;
            m_CurHP = a_BU_MaxHP;
            m_AttackSpeed = a_BU_AttSp;
            m_MoveVelocity = a_BU_MvSpeed;

            a_isSetLevel = false;
        }//if(a_isSetLevel == true)

        MonsterAI();
    }

    public void TakeDamage(float a_Value)
    {
        if (m_CurHP <= 0.0f)
            return;

        InGame_Mgr.Inst.DamageTxt((int)a_Value, this.transform);

        m_CurHP = m_CurHP - a_Value;
        if (m_CurHP < 0.0f)
            m_CurHP = 0.0f;

        if (m_HPSdBar != null)
            m_HPSdBar.fillAmount = m_CurHP / m_MaxHP;

        m_AggroTarget = InGame_Mgr.m_refHero.gameObject;
        m_AIState = MonAIState.MAI_AggroTrace;

        if (m_CurHP <= 0.0f) //몬스터 사망 처리
        {
            // 보상
            InGame_Mgr.Inst.AddMonKill(); //몬스터 Kill Count +1
            // InGame_Mgr.Inst.AddGold();

            ItemDrop();

            if (m_Level < 4 && Monster_Mgr.RefMonMgr != null)
                Monster_Mgr.RefMonMgr.ReSetSpawn(m_SpawnIdx);

            Destroy(gameObject); //<--몬스터 GameObject 제거됨
        }//if (m_CurHP <= 0.0f) //몬스터 사망 처리

    }//public void TakeDamage(float a_Value)

    public void ItemDrop()
    {
        int a_Rnd = Random.Range(0, 6);  //7);   0 ~ 5

        if (a_Rnd < 0 || 5 < a_Rnd)
            return;
  
        GameObject m_Itme = null;
        m_Itme = (GameObject)Instantiate(Resources.Load("Item_Obj"));
        m_Itme.transform.position = this.transform.position;
        if (a_Rnd == 0)
        {
            m_Itme.name = "coin_Item_Obj";
        }
        else if (a_Rnd == 1)
        {
            m_Itme.name = "bomb_Item_Obj";
        }
        else
        {
            Item_Type a_ItType = (Item_Type)a_Rnd;
            m_Itme.name = a_ItType.ToString() + "_Item_Obj"; 
        }

        Renderer a_RefRender = m_Itme.GetComponent<Renderer>();
        a_RefRender.material.SetTexture("_MainTex", InGame_Mgr.Inst.m_ItemImg[a_Rnd]);

        ItemObjInfo a_RefItemInfo = m_Itme.GetComponent<ItemObjInfo>();
        if (a_RefItemInfo != null)
        {
            a_RefItemInfo.InitItem((Item_Type)a_Rnd, m_Itme.name,
                                    Random.Range(1, 6), Random.Range(1, 6));
        }

        Destroy(m_Itme, 15.0f);  //15초내에 먹어야 한다.
    }

    void MonsterAI()
    {
        if (0.0f < m_ShootCool)
            m_ShootCool = m_ShootCool - Time.deltaTime;

        if (m_AIState == MonAIState.MAI_Patrol)
        {
            //---패트롤 상태라고 하더라도 20m 안쪽으로 적이 접근하면 추적하겠다는 코드
            if (InGame_Mgr.m_refHero != null)
            {
                a_CacVLen = InGame_Mgr.m_refHero.transform.position
                                        - this.transform.position;
                a_CacVLen.y = 0.0f;

                a_CacDist = a_CacVLen.magnitude;

                if (a_CacDist < m_TraceDist) //추적거리
                {
                    m_AIState = MonAIState.MAI_NormalTrace;   //일반 추적모드로 돌아가면 공격범위안에 있기 때문에 바로 공겨할 것이다.
                    m_AggroTarget = InGame_Mgr.m_refHero.gameObject;

                    return;
                }
            }
            //---패트롤 상태라고 하더라도 20m 안쪽으로 적이 접근하면 추적하겠다는 코드

            //---패트롤 
            if (m_bMvPtOnOff == true)
            {
                m_CacNowStep = Time.deltaTime * m_MoveVelocity; //이번에 한걸음 길이 (보폭)

                m_CacEndVec = m_PatrolTarget - this.transform.position;
                m_CacEndVec.y = 0.0f;
                m_DirMvVec = m_CacEndVec.normalized;

                m_AddTimeCount = m_AddTimeCount + Time.deltaTime;
                if (m_MoveDurTime <= m_AddTimeCount) //목표점에 도착한 것으로 판정한다.
                {
                    m_bMvPtOnOff = false;
                }
                else
                {
                    this.transform.position = this.transform.position +
                                                    (m_DirMvVec * m_CacNowStep);
                }
            }
            else
            {
                m_WaitTime = m_WaitTime - Time.deltaTime;
                if (0.0f < m_WaitTime)
                {
                    //숨쉬기 애니메이션으로 바꿔주는 부분
                }
                else
                {
                    m_WaitTime = 0.0f;
                    a_AngleRan = Random.Range(30, 301);
                    a_LengthRan = Random.Range(3, 8);

                    a_DirPtVec = this.transform.position - m_BasePos;
                    a_DirPtVec.y = 0.0f;
                    if (a_DirPtVec.magnitude < 1.0f)
                    {
                        a_CacPtRot = Quaternion.LookRotation(this.transform.forward);
                    }
                    else
                    {
                        a_CacPtRot = Quaternion.LookRotation(a_DirPtVec);
                    }
                    a_CacPtAngle = a_CacPtRot.eulerAngles;
                    a_CacPtAngle.y = a_CacPtAngle.y + (float)a_AngleRan;
                    a_CacPtRot.eulerAngles = a_CacPtAngle;
                    a_Vect = new Vector3(0, 0, 1);   //a_CacPtRot 로 방향벡터를 만든다.
                    a_Vect = a_CacPtRot * a_Vect;    // Vector3 값
                    a_Vect.Normalize();

                    m_PatrolTarget = m_BasePos + (a_Vect * (float)a_LengthRan);

                    m_DirMvVec = m_PatrolTarget - this.transform.position;
                    m_DirMvVec.y = 0.0f;
                    m_MoveDurTime = m_DirMvVec.magnitude / m_MoveVelocity;
                    //도착하는데 걸리는 시간
                    m_AddTimeCount = 0.0;
                    m_DirMvVec.Normalize();

                    m_WaitTime = Random.Range(0.2f, 3.0f);
                    m_bMvPtOnOff = true;
                }
            } //else
            //---패트롤 
        }
        else if (m_AIState == MonAIState.MAI_NormalTrace) //추적상태
        {
            if (m_AggroTarget == null)
            {
                m_AIState = MonAIState.MAI_Patrol;
                return;
            }

            a_CacVLen = m_AggroTarget.transform.position - this.transform.position;
            a_CacVLen.y = 0.0f;

            a_CacDist = a_CacVLen.magnitude;

            if (a_CacDist < m_AttackDist) //공격거리
            {
                m_AIState = MonAIState.MAI_Attack;
            }

            if (a_CacDist < m_TraceDist) //추적거리
            {
                m_MoveDir = a_CacVLen.normalized;

                m_NowStep = m_MoveVelocity * 1.5f * Time.deltaTime; //한걸음 크기
                a_MoveNextStep = m_MoveDir * m_NowStep;      //한걸음 벡터
                a_MoveNextStep.y = 0.0f;

                this.transform.position = this.transform.position + a_MoveNextStep;
            }
            else //추적거리 밖이면
            {
                m_AIState = MonAIState.MAI_Patrol;
            }
        }//else if (m_AIState == MonAIState.MAI_NormalTrace) //추적상태
        else if (m_AIState == MonAIState.MAI_AggroTrace) //어그로 추적상태 (어그로 상대를 향해 돌진)
        {
            if (m_AggroTarget == null)
            {
                m_AIState = MonAIState.MAI_Patrol;
                return;
            }

            a_CacVLen = m_AggroTarget.transform.position - this.transform.position;
            a_CacVLen.y = 0.0f;

            a_CacDist = a_CacVLen.magnitude;
            m_MoveDir = a_CacVLen.normalized;

            if (a_CacDist < m_AttackDist) //공격거리
            {
                m_AIState = MonAIState.MAI_Attack; 
            }

            if ((m_AttackDist - 2.0f) < a_CacDist) //공격거리 //else //추적거리이면서 공격거리가 아닐 때... 
            {
                m_NowStep = m_MoveVelocity * 10.0f * Time.deltaTime; //한걸음 크기
                a_MoveNextStep = m_MoveDir * m_NowStep;      //한걸음 벡터
                a_MoveNextStep.y = 0.0f;

                this.transform.position = this.transform.position + a_MoveNextStep;
            }//else if (m_AttackDist <= a_CacDist ) //공격거리가 아닐 때  

        }
        else if (m_AIState == MonAIState.MAI_Attack) //공격상태
        {
            if (m_AggroTarget == null)
            {
                m_AIState = MonAIState.MAI_Patrol;
                return;
            }

            a_CacVLen = m_AggroTarget.transform.position - this.transform.position;
            a_CacVLen.y = 0.0f;

            a_CacDist = a_CacVLen.magnitude;

            if ((m_AttackDist - 2.0f) < a_CacDist) //공격을 위해 아직 이동해야 하는 상황이면...  
            {
                m_MoveDir = a_CacVLen.normalized;

                m_NowStep = m_MoveVelocity * 1.5f * Time.deltaTime; //한걸음 크기
                a_MoveNextStep = m_MoveDir * m_NowStep;      //한걸음 벡터
                a_MoveNextStep.y = 0.0f;

                this.transform.position = this.transform.position + a_MoveNextStep;

            } //if ((m_AttackDist - 2.0f) < a_CacDist) //공격을 위해 아직 이동해야 하는 상황이면...  

            if (a_CacDist < m_AttackDist) //공격거리
            {
                if (m_ShootCool <= 0.0f)
                {
                    Shooting();
                    m_ShootCool = m_AttackSpeed;
                }
            } //if (a_CacDist < m_AttackDist) //공격거리
            else
            {
                m_AIState = MonAIState.MAI_NormalTrace;
            }
        }//else if(m_AIState == MonAIState.MAI_Attack) //공격상태


    } //void MonsterAI()

    public void Shooting()
    {
        if (m_AggroTarget == null)
            return;

        a_CacVLen = m_AggroTarget.transform.position - this.transform.position;
        a_CacVLen.y = 0.0f;
        Vector3 a_CacDir = a_CacVLen.normalized;

        //------------------ 랜덤한 각도로 발사하기...
        if (3 < m_Level)
        {
            Quaternion a_CacRot = Quaternion.LookRotation(a_CacDir);
            float a_CacRan = Random.Range(-15.0f, 15.0f);
            a_CacRot.eulerAngles = new Vector3(a_CacRot.eulerAngles.x,
                            a_CacRot.eulerAngles.y + a_CacRan,
                            a_CacRot.eulerAngles.z);
            Vector3 a_DirVec = new Vector3(0, 0, 1);
            a_DirVec = a_CacRot * a_DirVec;
            a_DirVec.Normalize();
            a_CacDir = a_DirVec;
        }//if (3 < m_Level)
        //------------------ 랜덤한 각도로 발사하기...

        GameObject newObj = (GameObject)Instantiate(InGame_Mgr.m_BulletObj); //오브젝트의 클론(복사체) 생성 함수   
        BulletCtrl a_BulletSC = newObj.GetComponent<BulletCtrl>();
        a_BulletSC.BulletSpawn(this.transform, a_CacDir, AllyType.AT_Enemy);
    }


    bool a_isSetLevel = false;
    float a_BU_MaxHP = 100.0f; //BackUp_MaxHP
    float a_BU_AttSp = 0.5f;
    float a_BU_MvSpeed = 2.0f;
    public void SetSpawnInfo(int Idx,
                             int a_Lv, float a_MaxHP, float a_AttSpeed, float a_MvSpeed)
    {
        m_SpawnIdx = Idx;
        m_Level = a_Lv;
        m_MaxHP = a_MaxHP;
        m_CurHP = a_MaxHP;
        m_AttackSpeed = a_AttSpeed;
        m_MoveVelocity = a_MvSpeed;

        a_isSetLevel = true;
        a_BU_MaxHP = a_MaxHP;
        a_BU_AttSp = a_AttSpeed;
        a_BU_MvSpeed = a_MvSpeed;

        //-----------이미지 교체 코드
        if (Monster_Mgr.RefMonMgr == null)
            return;

        int ImgIdx = m_Level - 1;
        ImgIdx = Mathf.Clamp(ImgIdx, 0, 3);
        Texture a_RefMonImg = Monster_Mgr.RefMonMgr.m_MonImg[ImgIdx];

        MeshRenderer[] m_MeshList = gameObject.GetComponentsInChildren<MeshRenderer>();
        if (0 < m_MeshList.Length)
            m_MeshList[0].material.SetTexture("_MainTex", a_RefMonImg);
        //-----------이미지 교체 코드
    }

} //public class MonsterCtrl : MonoBehaviour
