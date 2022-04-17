using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//ToDo 리스트
// //1, 몬스터 AI
// //2, 총알 화면 밖으로 나가면 제거하기(예외처리)
// //3, 몬스터 사망시 (레벨업) 난이도
// //4, 주인공 사망 처리
// //5, 주인공 스킬 구현 (보호막)
//6, 마우스 피킹으로 몬스터 공격하기...
//7, 캐릭터 애니메이션
//8, 사운드 매니저 만들기...

public class HeroCtrl : MonoBehaviour
{
    [HideInInspector] public float m_MaxHP;
    [HideInInspector] public float m_CurHP;
    public Image m_HPSdBar = null; //using UnityEngine.UI; 필요

    //---------- 키보드 입력값 변수 선언
    float h, v;
    private float m_MoveSpeed = 10.0f;
    //초당 10픽셀을 이동해라 라는 속도 (이동속도)
    Vector3 MoveNextStep;        //보폭을 계산해 주기 위한 변수
    Vector3 MoveHStep;
    Vector3 MoveVStep;
    //---------- 키보드 입력값 변수 선언

    //------------------- LimitMove (주인공 캐릭터가 지형을 벗어나지 못하게 막기)
    Transform  m_HeroMeshTr = null;
    CameraCtrl RefCamCtrl = null;
    Vector3 HalfSize = Vector3.zero;

    float a_LmtBdLeft = 0;
    float a_LmtBdTop = 0;
    float a_LmtBdRight = 0;
    float a_LmtBdBottom = 0;

    Vector3 m_CacCurPos = Vector3.zero;
    //------------------- LimitMove (주인공 캐릭터가 지형을 벗어나지 못하게 막기)

    //---------- 총알 발사 관련 변수 선언
    private Vector3 a_CurPos;
    private Vector3 a_CacEndVec;
    float m_AttSpeed  = 0.1f;  //공격속도(공속)
    float m_CacAtTick = 0.0f;  //기관총 발사 틱 만들기....
    //---------- 총알 발사 관련 변수 선언

    public Text m_NickName = null;

    private Vector3 m_DirVec;              //이동하려는 방향 벡터

    //----- 마우스 피킹 관련 변수들...
    [HideInInspector] public bool m_bMoveOnOff = false;  //현재 마우스피킹으로 이동중인지?의 여부
    private Vector3 m_TargetPos;           //마우스피킹 목표점
    private float   a_CacStep;             //한 스탭 계산용 변수

    float m_AttackDist = 16.0f; //14.0f;            //공격거리
    float m_ShootCool = 1.0f;              //공격 쿨타임 (공격 주기)

    Vector3 a_PickVec = Vector3.zero;
    public ClickMark  m_ClickMark = null;
    //----- 마우스 피킹 관련 변수들...

    //---JoyStick 이동 처리 변수
    private float m_JoyMvLen = 0.0f;
    private Vector3 m_JoyMvDir = Vector3.zero;
    //---JoyStick 이동 처리 변수

    public static float m_ShotRange = 30.0f;  //사거리 

    //------ 쉴드
    float m_SdDuration = 10.0f;
    float m_SdOnTime = 0.0f;
    public GameObject ShieldObj = null;
    SphereCollider ShereColl;
    //------ 쉴드

    //----------------- 몬스터 피킹 공격 관련 변수
    Ray a_MousePos;
    RaycastHit hitInfo;
    private LayerMask  m_layerMask = -1;
    private GameObject m_TargetUnit = null;
    //----------------- 몬스터 피킹 공격 관련 변수

    //----------------- 애니메이션 관련 변수 
    AnimSequence m_AnimSeq;
    Quaternion m_CacRot;
    //----------------- 애니메이션 관련 변수 

    // Start is called before the first frame update
    void Start()
    {
        m_MaxHP = 200.0f;
        m_CurHP = m_MaxHP;

        if (m_NickName != null)
            m_NickName.text = PlayerPrefs.GetString("UserNick", "User");

        //------------------- LimitMove (주인공 캐릭터가 지형을 벗어나지 못하게 막기)
        //GameObject a_MCamObj = GameObject.Find("Main Camera");
        //if(a_MCamObj != null)
        //    RefCamCtrl = a_MCamObj.GetComponent<CameraCtrl>();

        RefCamCtrl = FindObjectOfType<CameraCtrl>();
        m_HeroMeshTr = transform.Find("HeroMesh"); //자식 오브젝트 찾기
        HalfSize.x = m_HeroMeshTr.localScale.x / 2.0f; //나중에 주인공 캐릭터 외형을 바꾸면 다시 계산해 준다.
        HalfSize.y = m_HeroMeshTr.localScale.y / 2.0f;
        HalfSize.z = m_HeroMeshTr.localScale.z / 2.0f;
        //------------------- LimitMove (주인공 캐릭터가 지형을 벗어나지 못하게 막

        ShereColl = GetComponent<SphereCollider>();

        m_layerMask = 1 << LayerMask.NameToLayer("Monster"); //Monster 만 피킹

        m_AnimSeq = this.gameObject.GetComponentInChildren<AnimSequence>();
        //차일드중 첫번째로 나오는 SequenceAni.cs 파일 접근법
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < m_CacAtTick)
            m_CacAtTick = m_CacAtTick - Time.deltaTime;

        //------------------- 총알 발사 코드
        if (Input.GetMouseButton(1)) //마우스 오른쪽 버튼 클릭시... 
        {
            if (m_CacAtTick <= 0.0f)
            {
                Shoot_Fire(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                m_CacAtTick = m_AttSpeed;
            }//if (m_CacAtTick <= 0.0f)

            m_TargetUnit = null;  //수동 공격으로 바뀌었을 때도 즉시 타겟 무효화
        }//if (Input.GetMouseButtonDown(1))
         //------------------- 총알 발사 코드

        //------------------- 마우스 클릭 이동 코드
        if (Input.GetMouseButtonDown(0)) //마우스 왼쪽 버튼 클릭시... (모바일에서 작동함)
        {
            if (IsPointerOverUIObject() == false) //UI를 클릭하지 않았을 때만....
            {

                a_MousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(a_MousePos, out hitInfo, Mathf.Infinity,
                                                                    m_layerMask.value))
                {
                    SetMsPicking(hitInfo.point, hitInfo.collider.gameObject);

                    if (m_ClickMark != null)
                        m_ClickMark.ClickMarkOnOff(false);  //클릭마크 즉시 끄기
                }
                else
                {
                    a_PickVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    SetMsPicking(a_PickVec);

                    if (m_ClickMark != null)
                        m_ClickMark.ResetEff(a_PickVec, this);
                }

            } //if (IsPointerOverUIObject() == false) //UI를 클릭하지 않았을 때만....
        } //if (Input.GetMouseButtonDown(0)) //마우스 왼쪽 버튼 클릭시... (모바일에서 작동함)
        //------------------- 마우스 클릭 이동 코드

        KeyBDMove();
        JoyStickMvUpdate();
        MousePickUpdate();

        MouseAttackUpdate();

        //------------------- LimitMove (주인공 캐릭터가 지형을 벗어나지 못하게 막기)
        LimitMove();
        //------------------- LimitMove (주인공 캐릭터가 지형을 벗어나지 못하게 막기)

        ShieldUpdate();

        //------------------- 애니메이션 셋팅 부분
        //조이스틱으로 움직임도 없고 //키보드 움직임도 없고 //마우스 이동도 없을 때 
        if (m_JoyMvLen <= 0.0f && (0.0f == h && 0.0f == v) && m_bMoveOnOff == false)  
        {
            m_AnimSeq.ChangeAniState(UnitState.Idle);
        }
        else
        {
            if (m_DirVec.magnitude <= 0.0f)
            {
                m_AnimSeq.ChangeAniState(UnitState.Idle);
            }
            else
            {
                // 방향에 따른 애니메이션 구하는곳
                m_CacRot = Quaternion.LookRotation(m_DirVec);
                m_AnimSeq.CheckAnimDir(m_CacRot.eulerAngles.y);
                // 방향에 따른 애니메이션 구하는곳
            }
        }//else
        //------------------- 애니메이션 셋팅 부분

    }//void Update()

    void KeyBDMove()   //키보드 이동처리
    {
        //-------------- 가감속 없이 이동 처리 하는 방법
        //화살표키 좌우키를 눌러주면 -1.0f, 0.0f, 1.0f 사이값을 리턴해 준다.
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");   //화살표 위쪽 : +1.0f, 화살표 아래쪽 : -1.0f
        //-------------- 가감속 없이 이동 처리 하는 방법

        if (0.0f != h || 0.0f != v) //키보드 이동처리
        {
            MoveHStep = Vector3.right * h;
            MoveVStep = Vector3.forward * v;

            MoveNextStep = MoveHStep + MoveVStep;
            m_DirVec = MoveNextStep.normalized;
            MoveNextStep = m_DirVec * m_MoveSpeed * Time.deltaTime;
            transform.Translate(MoveNextStep); //월드축 기준으로의 이동
            //transform.position = transform.position + MoveNextStep;
        }
    }

    void LimitMove()
    {
        if (RefCamCtrl == null)
            return;

        m_CacCurPos = transform.position;

        a_LmtBdLeft  = RefCamCtrl.m_GroundMin.x + 4.0f + HalfSize.x;
        a_LmtBdTop   = RefCamCtrl.m_GroundMin.z + 4.0f + HalfSize.z;
        a_LmtBdRight  = RefCamCtrl.m_GroundMax.x - 4.0f - HalfSize.x;
        a_LmtBdBottom = RefCamCtrl.m_GroundMax.z - 4.0f - HalfSize.z;

        if (m_CacCurPos.x < a_LmtBdLeft)
            m_CacCurPos.x = a_LmtBdLeft;

        if (a_LmtBdRight < m_CacCurPos.x)
            m_CacCurPos.x = a_LmtBdRight;

        if (m_CacCurPos.z < a_LmtBdTop)
            m_CacCurPos.z = a_LmtBdTop;

        if (a_LmtBdBottom < m_CacCurPos.z)
            m_CacCurPos.z = a_LmtBdBottom;

        transform.position = m_CacCurPos;
    }

    public void Shoot_Fire(Vector3 a_TPos) // 클릭이벤트가 발생했을때 이 함수를 호출합니다.
    {
        //Instantiate 복사본 만들어줘~ 요청 함수
        GameObject newObj = (GameObject)Instantiate(InGame_Mgr.m_BulletObj); 
        //오브젝트의 클론(복사체) 생성 함수   

        a_CurPos = this.transform.position;
        a_CacEndVec = a_TPos - a_CurPos;
        a_CacEndVec.y = 0.0f;
        Vector3 a_CacDir = a_CacEndVec.normalized;  //피킹으로 발사하는 경우

        BulletCtrl a_BulletSC = newObj.GetComponent<BulletCtrl>();
        a_BulletSC.BulletSpawn(this.transform, a_CacDir);
    }

    void OnTriggerEnter(Collider other) //Item먹기
    {
        if (other.gameObject.name.Contains("coin_") == true)
        {
            InGame_Mgr.Inst.AddGold(10);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.name.Contains("bomb_") == true)
        {
            InGame_Mgr.Inst.AddSkill();  //스킬 증가
            Destroy(other.gameObject);
        }
        else if (other.gameObject.name.Contains("Item_Obj") == true)
        {
            //인벤토리에 넣기...
            InGame_Mgr.Inst.InvenAddItem(other.gameObject);
            Destroy(other.gameObject);
        }
    }

    public void SetMsPicking(Vector3 a_Pos, GameObject a_PickMon = null) //마우스 클릭시 마우스 좌표를 타겟으로 넘겨 받음
    {
        Vector3 a_CacVec = a_Pos - this.transform.position;
        a_CacVec.y = 0.0f;
        if (a_CacVec.magnitude < 1.0f)
        {
            return;
        }

        m_TargetUnit = a_PickMon; //타겟 초기화 또는 무효화 

        m_bMoveOnOff = true;

        m_DirVec = a_CacVec;
        m_DirVec.Normalize();        // 단위 벡터를 만든다. 
        m_TargetPos = new Vector3(a_Pos.x,
                                 this.transform.position.y,
                                  a_Pos.z);        // 목표점
    }

    void MousePickUpdate()  //마우스 피킹 이동
    {
        if( 0.0f < m_JoyMvLen || (h != 0.0f || v != 0.0f) )  //키보드나 조이스틱으로 움직일 때 
        {
            m_TargetUnit = null;  //키보드, 조이스틱 이동일때도 즉시 타겟 무효화
            m_bMoveOnOff = false; //즉시 마우스 이동 취소...
        }

        if (m_bMoveOnOff == true)
        {
            a_CacStep = Time.deltaTime * m_MoveSpeed; //이번에 한걸음 길이 (보폭)

            a_CurPos = this.transform.position;
            //----m_TargetUnit 존재하면 타겟을 향해 이동
            if (m_TargetUnit != null)
            {
                a_CacEndVec = m_TargetUnit.transform.position - a_CurPos;
                a_CacEndVec.y = 0.0f;
                if (a_CacEndVec.magnitude <= m_AttackDist) //공격거리
                {
                    m_bMoveOnOff = false; //즉시 멈춰서 공격 상태로 변경
                    return;
                }//if (a_CacEndVec.magnitude <= m_AttackDist) //공격거리
            }
            //----m_TargetUnit 존재하면 타겟을 향해 이동
            else //타겟이 존재하지 않으면 그냥 일반 이동
            {
                a_CacEndVec = m_TargetPos - a_CurPos;
                a_CacEndVec.y = 0.0f;
            }

            if (a_CacEndVec.magnitude <= a_CacStep)
            { //목표점까지의 거리보다 보폭이 크거나 같으면 도착으로 본다.
                m_bMoveOnOff = false;
            }
            else
            {
                m_DirVec = a_CacEndVec;
                m_DirVec.Normalize();
                this.transform.position = a_CurPos + (m_DirVec * a_CacStep);
            }

        }//if (m_bMoveOnOff == true)
    }

    PointerEventData a_EDCurPos; // using UnityEngine.EventSystems;
    public bool IsPointerOverUIObject() //UGUI의 UI들이 먼저 피킹되는지 확인하는 함수
    {
        a_EDCurPos = new PointerEventData(EventSystem.current);

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)

			List<RaycastResult> results = new List<RaycastResult>();
			for (int i = 0; i < Input.touchCount; ++i)
			{
				a_EDCurPos.position = Input.GetTouch(i).position;  
				results.Clear();
				EventSystem.current.RaycastAll(a_EDCurPos, results);
                if (0 < results.Count)
                    return true;
			}

			return false;
#else
        a_EDCurPos.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(a_EDCurPos, results);
        return (0 < results.Count);
#endif
    }//public bool IsPointerOverUIObject() 

    public void SetJoyStickMv(float a_JoyMvLen, Vector3 a_JoyMvDir)
    {
        m_JoyMvLen = a_JoyMvLen;
        if (0.0f < a_JoyMvLen)
        {
            m_JoyMvDir = new Vector3(a_JoyMvDir.x, 0.0f, a_JoyMvDir.y);
        }
    }//public void SetJoyStickMv(float a_JoyMvLen, Vector3 a_JoyMvDir)

    public void JoyStickMvUpdate()
    {
        if (0.0f != h || 0.0f != v)
            return;

        ////--- 조이스틱 코드
        if (0.0f < m_JoyMvLen)  //조이스틱으로 움직일 때 
        {
            m_DirVec = m_JoyMvDir;

            float amtToMove = m_MoveSpeed * Time.deltaTime;

            transform.Translate(m_JoyMvDir * m_JoyMvLen * amtToMove, Space.Self); // moving front/back
            //transform.position = transform.position + (m_JoyMvDir * m_JoyMvLen * amtToMove);
            // 방향에 따른 애니메이션 구하는곳
        }
        ////--- 조이스틱 코드

    }//public void JoyStickMvUpdate()

    public void TakeDamage(float a_Value)
    {
        if (0.0f < m_SdOnTime)
            return;

        m_CurHP = m_CurHP - a_Value;
        if (m_CurHP < 0.0f)
            m_CurHP = 0.0f;

        if (0.0f < m_CurHP)
            InGame_Mgr.Inst.DamageTxt((int)a_Value, this.transform);

        if (m_HPSdBar != null)
        {
            m_HPSdBar.fillAmount = m_CurHP / m_MaxHP;
        }

        if (m_CurHP <= 0.0f)
        {
            m_CurHP = 0.0f;

            UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
            //오늘 실습 과제 GameOver 화면 만들기... 
            //GameOver 타이틀
            //게임 플레이 시간 : 00 : 00 : 00
            //몬스터 킬 수 :
            //"확인"버튼 --> 로비로 이동
            //"리플레이"버튼 은 만들어 보실 분들만...
        }
    }

    void ShieldUpdate()
    {
        //------------------- 쉴드 발동
        if (Input.GetKeyDown(KeyCode.F) == true)
        {
            if (0 < GlobalUserData.s_SkillCount)
            {
                if (m_SdOnTime <= 0.0f)
                {
                    m_SdOnTime = m_SdDuration;

                    GlobalUserData.s_SkillCount--;
                    if (GlobalUserData.s_SkillCount < 0)
                        GlobalUserData.s_SkillCount = 0;

                    if (InGame_Mgr.Inst != null && InGame_Mgr.Inst.m_SkillTxt != null)
                    {
                        InGame_Mgr.Inst.m_SkillTxt.text = "x " + 
                                        GlobalUserData.s_SkillCount.ToString();
                    }

                    PlayerPrefs.SetInt("SkillCount", GlobalUserData.s_SkillCount);  //값 저장
                }//if (m_SdOnTime <= 0.0f)
            }//if (0 < GlobalUserData.s_SkillCount)
        }//if (Input.GetKeyDown(KeyCode.F) == ture)
        //------------------- 쉴드 발동

        //------------------- 쉴드 상태 업데이트
        if (0.0f < m_SdOnTime)
        {
            m_SdOnTime = m_SdOnTime - Time.deltaTime;
            if (ShieldObj != null && ShieldObj.activeSelf == false)
                ShieldObj.SetActive(true);

            if (ShereColl != null && ShereColl.radius != 4.0f)
                ShereColl.radius = 4.0f;
        }
        else
        {
            if (ShieldObj != null && ShieldObj.activeSelf == true)
                ShieldObj.SetActive(false);

            if (ShereColl != null && ShereColl.radius != 2.5f)
                ShereColl.radius = 2.5f;
        }
        //------------------- 쉴드 상태 체크
    }

    void MouseAttackUpdate()
    {
        if (m_TargetUnit == null)
            return;

        a_CacEndVec = m_TargetUnit.transform.position - this.transform.position;
        a_CacEndVec.y = 0.0f;
        if (m_AttackDist < a_CacEndVec.magnitude) //아직 공격거리밖이면 공격하지 않는다.
            return;

        if (m_CacAtTick <= 0.0f)
        {
            Shoot_Fire(m_TargetUnit.transform.position);

            m_CacAtTick = m_AttSpeed;
        }//if (m_CacAtTick <= 0.0f)
    }//void MouseAttackUpdate()

}//public class HeroCtrl : MonoBehaviour
