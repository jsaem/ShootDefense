using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

enum JoyStickType
{
    Fixed = 0,
    //m_JoySBackObj.activeSelf == true && m_JoystickPickPanel.activeSelf == false
    Flexible = 1,
    //m_JoySBackObj.activeSelf == true && m_JoystickPickPanel.activeSelf == true
    FlexibleOnOff = 2
    //m_JoySBackObj.activeSelf == false && m_JoystickPickPanel.activeSelf == true
}

public class InGame_Mgr : MonoBehaviour
{
    public static InGame_Mgr Inst;

    public Button m_BackBtn;

    public static GameObject m_BulletObj = null;
    public static HeroCtrl m_refHero = null;

    //----------------- 머리위에 데미지 띄우기용 변수 선언
    Vector3 a_StCacPos = Vector3.zero;
    [Header("-------- Damage Text --------")]
    public Transform m_HUD_Canvas = null;
    public GameObject m_DamageObj = null;
    //----------------- 머리위에 데미지 띄우기용 변수 선언

    //---------------------------- UserInfo UI 관련 변수
    bool m_UInfo_OnOff = false;
    [Header("-------- UserInfo UI --------")]
    [SerializeField] private Button m_UserInfo_Btn = null;
    public GameObject m_UserInfoPanel = null;
    public Text m_UserHPTxt;
    public Text m_SkillTxt;
    public Text m_MonKillTxt;
    public Text m_GoldTxt;

    int m_MonKillCount = 0;         // 몬스터 킬수 변수
    //---------------------------- UserInfo UI 관련 변수

    //---------------------------- 환경설정 Dlg 관련 변수
    [Header("-------- Config DialogBox --------")]
    public Button m_CfgBtn = null;
    public GameObject Canvas_Dialog = null;
    private GameObject m_ConfigBoxObj = null;
    //---------------------------- 환경설정 Dlg 관련 변수

    [Header("-------- ItemIconImg --------")]
    public Texture[] m_ItemImg = null;

    //---------------------------- Inventory ScrollView
    [Header("-------- Inventory ScrollView OnOff --------")]
    public Button m_InVen_Btn = null;
    public Transform m_InVenScrollTr = null;
    private bool m_InVen_ScOnOff = false;
    private float m_ScSpeed = 1800.0f;
    private Vector3 m_ScOnPos = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 m_ScOffPos = new Vector3(320.0f, 0.0f, 0.0f);

    public Transform m_MkInvenContent = null;
    public GameObject m_MkItemMyNode = null;
    public Button m_ItemSell_Btn = null;
    //---------------------------- Inventory ScrollView

    //-----------Fixed JoyStick 처리 부분
    JoyStickType m_JoyStickType = JoyStickType.Fixed;

    [Header("-------- JoyStick --------")]
    public GameObject m_JoySBackObj = null;
    public Image m_JoyStickImg = null;
    float m_Radius = 0.0f;
    Vector3 m_OrignPos = Vector3.zero;
    Vector3 m_Axis = Vector3.zero;
    Vector3 m_JsCacVec = Vector3.zero;
    float m_JsCacDist = 0.0f;
    //-----------Fixed JoyStick 처리 부분

    //-----------Flexible JoyStick 처리 부분
    public  GameObject m_JoystickPickPanel = null;
    private Vector2 posJoyBack;
    private Vector2 dirStick;
    //-----------Flexible JoyStick 처리 부분

    void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GlobalUserData.LoadGameInfo();
        ReflashInGameItemScV();

        m_refHero = FindObjectOfType<HeroCtrl>(); //<-- Hierarchy쪽에서
        //HeroCtrl 컴포넌트가 붙어 있는 게임 오브젝트를 찾아서 객체를 찾아오는 방법

        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(() => {
                UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
            });

        m_BulletObj = Resources.Load("BulletObj") as GameObject;

        //---- UserInfoPanel On/Off 구현 코드
        m_UInfo_OnOff = m_UserInfoPanel.activeSelf;

        if(m_UserInfo_Btn != null && m_UserInfoPanel != null)
        {
            m_UserInfo_Btn.onClick.AddListener(() =>
               {
                   m_UInfo_OnOff = !m_UInfo_OnOff;
                   m_UserInfoPanel.SetActive(m_UInfo_OnOff);

                   //if (m_UserInfoPanel.activeSelf == true)
                   //    m_UserInfoPanel.SetActive(false);
                   //else if (m_UserInfoPanel.activeSelf == false)
                   //    m_UserInfoPanel.SetActive(true);
               });
        }
        //---- UserInfoPanel On/Off 구현 코드

        if (m_GoldTxt != null)
        {
            if (GlobalUserData.s_GoldCount <= 0)
                m_GoldTxt.text = "x 00";
            else
                m_GoldTxt.text = "x " + GlobalUserData.s_GoldCount.ToString("N0");
        }

        if (m_SkillTxt != null)
        {
            if (GlobalUserData.s_SkillCount <= 0)
                m_SkillTxt.text = "x 00";
            else
                m_SkillTxt.text = "x " + GlobalUserData.s_SkillCount.ToString();
        }//if(m_SkillTxt != null)

        //---------------------------- 환경설정 Dlg 관련 구현 부분
        if (m_CfgBtn != null)
            m_CfgBtn.onClick.AddListener(() =>
            {
                if (m_ConfigBoxObj == null)
                    m_ConfigBoxObj = Resources.Load("ConfigBox") as GameObject;

                GameObject a_CfgBoxObj = (GameObject)Instantiate(m_ConfigBoxObj);
                a_CfgBoxObj.transform.SetParent(Canvas_Dialog.transform, false);
                //false 로 해야 로컬 프리랩에 설정된 좌표를 유지한채 차일드로 붙게된다.
                Time.timeScale = 0.0f; //일시정지
            });
        //---------------------------- 환경설정 Dlg 관련 구현 부분

        //-------- 인벤토리 판넬 OnOff
        if (m_InVen_Btn != null)
        {
            m_InVen_Btn.onClick.AddListener(() =>
            {
                m_InVen_ScOnOff = !m_InVen_ScOnOff;
                if(m_ItemSell_Btn != null)
                    m_ItemSell_Btn.gameObject.SetActive(m_InVen_ScOnOff);
            });
        }
        //-------- 인벤토리 판넬 OnOff

        //----------------------------- 아이템 판매 버튼 처리
        if (m_ItemSell_Btn != null)
            m_ItemSell_Btn.onClick.AddListener(ItemSellMethod);
        //----------------------------- 아이템 판매 버튼 처리

        //-----------Fixed JoyStick 처리 부분
        if (m_JoySBackObj != null && m_JoyStickImg != null &&
            m_JoySBackObj.activeSelf == true &&
            m_JoystickPickPanel.activeSelf == false)
        {
            m_JoyStickType = JoyStickType.Fixed;

            Vector3[] v = new Vector3[4];
            m_JoySBackObj.GetComponent<RectTransform>().GetWorldCorners(v);
            //[0]:좌측하단 [1]:좌측상단 [2]:우측상단 [3]:우측하단
            //v[0] 촤측하단이 0, 0 좌표인 스크린 좌표(Screen.width, Screen.height)를 기준으로   
            //RectTransform : 즉 UGUI 좌표 기준
            m_Radius = v[2].y - v[0].y;
            m_Radius = m_Radius / 3.0f;

            m_OrignPos = m_JoyStickImg.transform.position;

            //스크립트로만 대기하고자 할 때 //using UnityEngine.EventSystems;
            EventTrigger trigger = m_JoySBackObj.GetComponent<EventTrigger>();
            // Inspector에서 GameObject.Find("Button"); 에 꼭 
            // AddComponent--> EventTrigger 가 되어 있어야 한다.
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener((data) =>
            {
                OnDragJoyStick((PointerEventData)data);
            });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.EndDrag;
            entry.callback.AddListener((data) =>
            {
                OnEndDragJoyStick((PointerEventData)data);
            });
            trigger.triggers.Add(entry);

        } //if (m_JoySBackObj != null ...)
        //-----------Fixed JoyStick 처리 부분

        //-----------Flexible JoyStick 처리 부분
        if (m_JoystickPickPanel != null && m_JoySBackObj != null
            && m_JoyStickImg != null
            && m_JoystickPickPanel.activeSelf == true)
        {
            if (m_JoySBackObj.activeSelf == true)
            {
                m_JoyStickType = JoyStickType.Flexible;
            }
            else
            {
                m_JoyStickType = JoyStickType.FlexibleOnOff;
            }

            EventTrigger a_JBTrigger = m_JoySBackObj.GetComponent<EventTrigger>();
            if (a_JBTrigger != null)
            {
                Destroy(a_JBTrigger);
            }// 조이스틱 백에 설치되어 있는 이벤트 트리거는 제거한다.

            Vector3[] v = new Vector3[4];
            m_JoySBackObj.GetComponent<RectTransform>().GetWorldCorners(v);
            m_Radius = v[2].y - v[0].y;
            m_Radius = m_Radius / 3.0f;

            m_OrignPos = m_JoyStickImg.transform.position;
            m_JoySBackObj.GetComponent<Image>().raycastTarget = false;
            m_JoyStickImg.raycastTarget = false;

            EventTrigger trigger = m_JoystickPickPanel.GetComponent<EventTrigger>(); // Inspector에서 m_JoystickPickPanel 에 꼭 AddComponent--> EventTrigger 가 되어 있어야 한다.
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) => {
                OnPointerDown_Flx((PointerEventData)data);
            });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener((data) => {
                OnPointerUp_Flx((PointerEventData)data);
            });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener((data) => {
                OnDragJoyStick_Flx((PointerEventData)data);
            });
            trigger.triggers.Add(entry);
        }
        //-----------Flexible JoyStick 처리 부분

        SoundMgr.Instance.PlayBGM("sound_bgm_island_001", 0.2f);

    }//void Start()

    // Update is called once per frame
    void Update()
    {
        InvenScOnOffUpdate();
    }

    public void DamageTxt(int a_Value, Transform  a_OwnerTr)
    {
        GameObject a_DamClone = (GameObject)Instantiate(m_DamageObj);
        if (a_DamClone != null && m_HUD_Canvas != null)
        {
            Vector3 a_StCacPos
                = new Vector3(a_OwnerTr.position.x, 0.8f, a_OwnerTr.position.z + 4.0f);

            a_DamClone.transform.SetParent(m_HUD_Canvas);
            DamageText a_DamageTx = a_DamClone.GetComponent<DamageText>();
            a_DamageTx.DamageTxtSpawn(a_Value, new Color32(200, 0, 0, 255));
            a_DamClone.transform.position = a_StCacPos;
        }
    }//public void DamageTxt(int a_Value, Transform  a_OwnerTr)

    public void AddMonKill(int a_Val = 1)
    {
        m_MonKillCount = m_MonKillCount + a_Val;
        m_MonKillTxt.text = "x " + m_MonKillCount.ToString();
    }

    public void AddGold(int a_Val = 5)
    {
        GlobalUserData.s_GoldCount = GlobalUserData.s_GoldCount + a_Val;
        m_GoldTxt.text = "x " + GlobalUserData.s_GoldCount.ToString("N0"); 
        //"N0" 천단위마다 쉼표 표시

        PlayerPrefs.SetInt("GoldCount", GlobalUserData.s_GoldCount); //값 저장
    }

    public void AddSkill(int a_Val = 1)
    {
        GlobalUserData.s_SkillCount = GlobalUserData.s_SkillCount + a_Val;
        m_SkillTxt.text = "x " + GlobalUserData.s_SkillCount.ToString();

        PlayerPrefs.SetInt("SkillCount", GlobalUserData.s_SkillCount);  //값 저장
    }

    void InvenScOnOffUpdate()
    {    //------------- 인벤토리 판넬 OnOff 연출

        if (m_InVenScrollTr == null)
            return;

        if (m_InVen_ScOnOff == false)
        {
            if (m_InVenScrollTr.localPosition.x < m_ScOffPos.x)
            {
                m_InVenScrollTr.localPosition =
                    Vector3.MoveTowards(m_InVenScrollTr.localPosition,
                                        m_ScOffPos, m_ScSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (m_ScOnPos.x < m_InVenScrollTr.localPosition.x)
            {
                m_InVenScrollTr.localPosition =
                    Vector3.MoveTowards(m_InVenScrollTr.localPosition,
                                        m_ScOnPos, m_ScSpeed * Time.deltaTime);
            }
        } //else
    } //void InvenScOnOffUpdate()

    public void InvenAddItem(GameObject a_Obj)
    {
        ItemObjInfo a_RefItemInfo = a_Obj.GetComponent<ItemObjInfo>();
        if (a_RefItemInfo != null)
        {
            ItemValue a_Node = new ItemValue();
            a_Node.UniqueID    = a_RefItemInfo.m_ItemValue.UniqueID;
            a_Node.m_Item_Type = a_RefItemInfo.m_ItemValue.m_Item_Type;
            a_Node.m_ItemName  = a_RefItemInfo.m_ItemValue.m_ItemName;
            a_Node.m_ItemLevel = a_RefItemInfo.m_ItemValue.m_ItemLevel;
            a_Node.m_ItemStar  = a_RefItemInfo.m_ItemValue.m_ItemStar;
            GlobalUserData.g_ItemList.Add(a_Node);

            AddNodeScrollView(a_Node); //스크롤 뷰에 추가
            GlobalUserData.ReflashItemSave();  //파일 저장
        }//if (a_RefItemInfo != null)
    }//public void InvenAddItem(GameObject a_Obj)

    public void AddNodeScrollView(ItemValue a_Node)
    {
        //---------- ScrollView UI 에 Node 추가
        GameObject m_ItemObj = (GameObject)Instantiate(m_MkItemMyNode);
        //Resources.Load("Prefab/ItemMyNode"));
        m_ItemObj.transform.SetParent(m_MkInvenContent, false);
        //false일 경우 : 로컬 기준의 정보를 유지한 채 차일드화된다.
        ItemNode a_MyItemInfo = m_ItemObj.GetComponent<ItemNode>();

        if (a_MyItemInfo != null)
            a_MyItemInfo.SetItemRsc(a_Node);
        //---------- ScrollView UI 에 Node 추가
    }

    void ItemSellMethod()
    {
        //스크롤뷰의 노드를 모두 돌면서 선택되어 있는 것들만 판매하고
        //해당 유니크ID를 g_ItemList에서 찾아서 제거해 준다.
        ItemNode[] a_MyNodeList =
            m_MkInvenContent.GetComponentsInChildren<ItemNode>(true); //true : Active 꺼져 있는 오브젝트까지 모두 가져오게 됨
        for (int ii = 0; ii < a_MyNodeList.Length; ii++)
        {
            if (a_MyNodeList[ii].m_SelOnOff == false)
                continue;

            for (int a_bb = 0; a_bb < GlobalUserData.g_ItemList.Count; a_bb++)
            {
                if (a_MyNodeList[ii].m_UniqueID ==
                                      GlobalUserData.g_ItemList[a_bb].UniqueID)
                {
                    GlobalUserData.g_ItemList.RemoveAt(a_bb);
                    break;
                }
            }//for (int a_bb = 0; a_bb < GlobalUserData.Instance.g_ItemList.Count; a_bb++)

            Destroy(a_MyNodeList[ii].gameObject);

            AddGold(100); //골드 증가

        }//for(int ii = 0; ii < a_MyNodeList.Length; ii++)

        GlobalUserData.ReflashItemSave(); //리스트 다시 저장

    }

    public void ReflashInGameItemScV()  //<---- InGame의 ScrollView  갱신
    { //GlobalUserData.g_ItemList 저장된 값을 ScrollView에 복원해 주는 함수

        ItemNode[] a_MyNodeList =
              m_MkInvenContent.GetComponentsInChildren<ItemNode>(true);
        for (int ii = 0; ii < a_MyNodeList.Length; ii++)
        {
            Destroy(a_MyNodeList[ii].gameObject);

        }//for(int ii = 0; ii < a_MyNodeList.Length; ii++)

        for (int a_ii = 0; a_ii < GlobalUserData.g_ItemList.Count; a_ii++)
        {
            AddNodeScrollView(GlobalUserData.g_ItemList[a_ii]); //In Game Scroll View 아이템 추가 함수)
        }
    }//public void ReflashInGameItemScV() 

    //-----------Fixed JoyStick 처리 부분
    //using UnityEngine.EventSystems;
    void OnDragJoyStick(PointerEventData _data) //Delegate
    {
        if (m_JoyStickImg == null)
            return;

        m_JsCacVec = Input.mousePosition - m_OrignPos;
        m_JsCacVec.z = 0.0f;
        m_JsCacDist = m_JsCacVec.magnitude;
        m_Axis = m_JsCacVec.normalized;

        //조이스틱 백그라운드를 벗어나지 못하게 막는 부분
        if (m_Radius < m_JsCacDist)
        {
            m_JoyStickImg.transform.position =
                                    m_OrignPos + m_Axis * m_Radius;
        }
        else
        {
            m_JoyStickImg.transform.position =
                                    m_OrignPos + m_Axis * m_JsCacDist;
        }

        if (1.0f < m_JsCacDist)
            m_JsCacDist = 1.0f;

        //캐릭터 이동 처리
        if (m_refHero != null)
            m_refHero.SetJoyStickMv(m_JsCacDist, m_Axis);
    }

    void OnEndDragJoyStick(PointerEventData _data) //Delegate
    {
        if (m_JoyStickImg == null)
            return;

        m_Axis = Vector3.zero;
        m_JoyStickImg.transform.position = m_OrignPos;

        m_JsCacDist = 0.0f;

        //캐릭터 정지 처리
        if (m_refHero != null)
            m_refHero.SetJoyStickMv(0.0f, m_Axis);
    }
    //-----------Fixed JoyStick 처리 부분

    //-----------Flexible JoyStick 처리 부분
    void OnPointerDown_Flx(PointerEventData eventData) //마우스 클릭시
    {
        if (eventData.button != PointerEventData.InputButton.Left) //마우스 왼쪽 버튼만
            return;

        if (m_JoySBackObj == null)
            return;

        if (m_JoyStickImg == null)
            return;

        m_JoySBackObj.transform.position = eventData.position;
        m_JoyStickImg.transform.position = eventData.position;

        m_JoySBackObj.SetActive(true);
    }

    void OnPointerUp_Flx(PointerEventData eventData)   //마우스 클릭 해제시
    {
        if (eventData.button != PointerEventData.InputButton.Left) //마우스 왼쪽 버튼만
            return;

        if (m_JoySBackObj == null)
            return;

        if (m_JoyStickImg == null)
            return;

        m_JoySBackObj.transform.position = m_OrignPos;
        m_JoyStickImg.transform.position = m_OrignPos;

        if (m_JoyStickType == JoyStickType.FlexibleOnOff)
        {
            m_JoySBackObj.SetActive(false);  //<---꺼진 상태로 시작하는 방식일 때는 활성화필요
        }

        m_Axis = Vector3.zero;
        m_JsCacDist = 0.0f;
        //m_JoyStickImg.gameObject.SetActive(false);
        //캐릭터 정지 처리
        if (m_refHero != null)
        {
            m_refHero.SetJoyStickMv(0.0f, Vector3.zero);
        }
    }

    void OnDragJoyStick_Flx(PointerEventData eventData) //마우스 드래그
    {
        if (eventData.button != PointerEventData.InputButton.Left) //마우스 왼쪽 버튼만
            return;

        //eventData.position 현재 마우스의 UI 기준의 월드 좌표 : 
        //좌측 하단 0, 0 //우측 상단 Screen.width, Screen.height

        if (m_JoyStickImg == null)
            return;

        posJoyBack = (Vector2)m_JoySBackObj.transform.position;
        //조이스틱 백 그라운드 현재 위치 기준
        m_JsCacDist = Vector2.Distance(posJoyBack, eventData.position); //거리 
        dirStick = eventData.position - posJoyBack;  //방향

        if (m_Radius < m_JsCacDist)
        {
            m_JsCacDist = m_Radius;
            m_JoyStickImg.transform.position =
                (Vector3)(posJoyBack + (dirStick.normalized * m_Radius));
        }
        else
        {
            m_JoyStickImg.transform.position = (Vector3)eventData.position;
        }

        if (1.0f < m_JsCacDist)
            m_JsCacDist = 1.0f;

        m_Axis = (Vector3)dirStick.normalized;

        if (m_refHero != null)
        {
            m_refHero.SetJoyStickMv(m_JsCacDist, m_Axis);
        }
        ////캐릭터 이동 처리  
    }
    //-----------Flexible JoyStick 처리 부분
}
