using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AllyType
{
    AT_Ally,   //아군
    AT_Enemy,  //적군
}

public class BulletCtrl : MonoBehaviour
{
    AllyType m_AllyType = AllyType.AT_Ally;

    //--이동 관련 변수들
    float m_LifeTime = 4.0f; //총알 생존 시간
    Vector3 m_OwnTrPos;      //주인공의 위치
    Vector3 m_DirTgVec;      //날아갈 방향 벡터
    Vector3 a_StartPos = new Vector3(0, 0, 1);  //스폰 위치 계산용 변수

    Vector3 a_MoveNextStep;  // 한플레임당 이동 벡터 계산용 변수
    private float m_MoveSpeed = 35.0f;   // 한플레임당 이동 시키고 싶은 거리 
    //--이동 관련 변수들

    CameraCtrl m_MainCam = null;

    TrailRenderer m_TRenderer;

    // Start is called before the first frame update
    void Start()
    {
        m_MainCam = Camera.main.GetComponent<CameraCtrl>();
        m_TRenderer = GetComponentInChildren<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_LifeTime = m_LifeTime - Time.deltaTime;
        if (m_LifeTime <= 0.0f)
        {
            Destroy(this.gameObject);  //게임오브젝트를 파괴하고자 할 때 사용하는 함수
        }

        if (m_TRenderer != null)
        {
            if (m_AllyType == AllyType.AT_Enemy)
            {
                if (m_TRenderer.time != 1.0f)
                    m_TRenderer.time = 1.0f;      //총알이 움직이기 시작할때에만 꼬리를 복원한다.
            }
            else if (m_AllyType == AllyType.AT_Ally)
            {
                m_TRenderer.time = -1.0f;  //지금 나는 그냥 트레일 끄기
            }
        }

        a_MoveNextStep = m_DirTgVec * (Time.deltaTime * m_MoveSpeed);
        a_MoveNextStep.y = 0.0f;

        transform.position = transform.position + a_MoveNextStep;

        //총알이 화면 밖에서 나가면 즉시 제거
        if (m_AllyType == AllyType.AT_Ally) //주인공편이 쏜 총알일때만 
        if (transform.position.x < m_MainCam.m_CamWMin.x ||
            m_MainCam.m_CamWMax.x < transform.position.x ||
            transform.position.z < m_MainCam.m_CamWMin.z ||
            m_MainCam.m_CamWMax.z < transform.position.z)
        {
            m_LifeTime = 0.0f;
            Destroy(this.gameObject);  //게임오브젝트를 파괴하고자 할 때 사용하는 함수
        }
        else
        {
            float a_Length = Vector3.Distance(transform.position, a_StartPos);
            if (HeroCtrl.m_ShotRange < a_Length) //피격거리제한
            {
                 m_LifeTime = 0.0f;
                 Destroy(this.gameObject);  //게임오브젝트를 파괴하고자 할 때 사용하는 함수
            }
        }// else 
        //총알이 화면 밖에서 나가면 즉시 제거

    } //void Update()

    public void BulletSpawn(Transform a_OwnTr, Vector3 a_DirVec,
                                                AllyType a_AllyType = AllyType.AT_Ally)
    {
        m_OwnTrPos = a_OwnTr.position;

        a_DirVec.y = 0.0f;
        m_DirTgVec = a_DirVec;
        m_DirTgVec.Normalize();

        a_StartPos = m_OwnTrPos + (m_DirTgVec * 2.5f);
        a_StartPos.y = transform.position.y;

        transform.position = new Vector3(a_StartPos.x,
                                        transform.position.y, a_StartPos.z);
        transform.rotation = Quaternion.LookRotation(m_DirTgVec);
        //<--총알이 날아가는 방향을 바라보게 회전시켜주는 부분
        m_LifeTime = 4.0f;

        m_AllyType = a_AllyType;

        //--- Trail 초기화
        if (m_TRenderer == null)
        {
            m_TRenderer = GetComponentInChildren<TrailRenderer>();
        }
        if (m_TRenderer != null)
        {
            m_TRenderer.time = -1.0f;      //이렇게 하면 꼬리를 완전히 없앨 수 있다.
        }
        //--- Trail 초기화

    }

    void OnTriggerEnter(Collider other)
    {   //총알이 뭔가에 충돌 되었을 때 발생되는 함수

        if(other.gameObject.name.Contains("Monster") == true) //맞은 객체
        {
            if (m_AllyType == AllyType.AT_Ally) //유저편이 쏜 총알(소속)
            {
                MonsterCtrl a_MonCtrl = other.gameObject.GetComponent<MonsterCtrl>();
                if (a_MonCtrl != null)
                    a_MonCtrl.TakeDamage(10.0f);

                m_LifeTime = 0.0f;
                Destroy(this.gameObject);  //게임오브젝트를 파괴하고자 할 때 사용하는 함수
            }
        }//if(other.gameObject.name.Contains("Monster") == true) //맞은 객체

        if (other.gameObject.name.Contains("HeroRoot") == true) //맞은 객체
        {
            if (m_AllyType == AllyType.AT_Enemy)
            {
                m_LifeTime = 0.0f;
                Destroy(this.gameObject);  //자기 자신(총알)도 삭제   메모리풀이 아니라면... Destroy(gameObject);   

                //HeroCtrl a_refHero = other.gameObject.GetComponent<HeroCtrl>();
                if (InGame_Mgr.m_refHero != null)
                    InGame_Mgr.m_refHero.TakeDamage(2.0f);
            }//if (m_AllyType == AllyType.AT_Enemy)
        }//if (other.gameObject.name.Contains("HeroRoot") == true) //맞은 객체

    }//void OnTriggerEnter(Collider other)

}
