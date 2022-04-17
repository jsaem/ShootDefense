using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDrapMgr : MonoBehaviour
{
    public SlotScript[] m_SlotSc;
    public RawImage a_MsObj = null;

    int m_SaveIndex = -1;
    bool m_IsPick = false;

    //-------- 아이콘 투명하게 사라지게 하기 연출용 변수
    private float AniDuring  = 0.8f;  //페이드아웃 연출을 시간 설정
    private float m_CacTime  = 0.0f;
    private float m_AddTimer = 0.0f;
    private Color m_Color;
    //-------- 아이콘 투명하게 사라지게 하기 연출용 변수

    [Header("-------- Buy Item --------")]
    public Text m_GoldTxt;
    public Text m_SkillTxt;

    [Header("-------- Info Txt --------")]
    public Text m_InfoTxt;
    private float m_InfoDuring = 1.5f;  //페이드아웃 연출을 시간 설정
    private float m_InfoAddTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        GlobalUserData.LoadGameInfo();

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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //왼쪽 마우스 버튼을 클릭한 순간
        {
            m_SaveIndex = -1;

            if (0 < m_SlotSc.Length && IsCollSlot(m_SlotSc[0].gameObject) == true)
            { //시작 슬롯을 클릭한 경우
                m_SaveIndex = 0;
                m_SlotSc[0].ItemImg.gameObject.SetActive(false);
                m_IsPick = true;
                a_MsObj.gameObject.SetActive(true);

            }//if (0 < m_SlotSc.Length && IsCollSlot(m_SlotSc[0]) == true)
        }//if (Input.GetMouseButtonDown(0))

        if (Input.GetMouseButton(0)) //왼쪽 마우스를 누르고 있는 동안
        {
            if (m_IsPick == true)
            {
                a_MsObj.transform.position = Input.mousePosition;
            }
        }//if (Input.GetMouseButton(0)) 

        if (Input.GetMouseButtonUp(0)) //왼쪽 마우스를 누르고 있다가 뗀 순간
        {
            if (m_IsPick == true)
            {
                if (1 < m_SlotSc.Length && IsCollSlot(m_SlotSc[1].gameObject) == true)
                { //도착 슬롯 위에서 마우스를 놓은 경우
                    m_SlotSc[1].ItemImg.gameObject.SetActive(true);
                    m_SlotSc[1].ItemImg.color = Color.white;
                    m_AddTimer = AniDuring;
                    m_IsPick = false;
                    a_MsObj.gameObject.SetActive(false);

                    //--------- 구매 허가
                    if (100 < GlobalUserData.s_GoldCount)
                    {
                        GlobalUserData.s_GoldCount = GlobalUserData.s_GoldCount - 100;
                        m_GoldTxt.text = "x " + GlobalUserData.s_GoldCount.ToString("N0");
                        //"N0" 천단위마다 쉼표 표시
                        PlayerPrefs.SetInt("GoldCount", GlobalUserData.s_GoldCount); //값 저장

                        GlobalUserData.s_SkillCount = GlobalUserData.s_SkillCount + 1;
                        m_SkillTxt.text = "x " + GlobalUserData.s_SkillCount.ToString();
                        PlayerPrefs.SetInt("SkillCount", GlobalUserData.s_SkillCount);  //값 저장
                    }
                    else //구매 불가
                    {
                        m_InfoTxt.gameObject.SetActive(true);
                        m_InfoTxt.color = Color.white;
                        m_InfoAddTimer = m_InfoDuring;
                    }
                    //--------- 구매 허가
                }//if (1 < m_SlotSc.Length && IsCollSlot(m_SlotSc[1].gameObject) == true)

                if (0 <= m_SaveIndex) //&& m_IsPick == true)
                {
                    m_SlotSc[m_SaveIndex].ItemImg.gameObject.SetActive(true);
                    m_IsPick = false;
                    a_MsObj.gameObject.SetActive(false);
                }

            }//if (m_IsPick == true)
        }//if (Input.GetMouseButtonUp(0)) 

        //---------- 장착된 아이콘이 서서히 사라지게 처리하는 연출
        if (0.0f < m_AddTimer)
        {
            m_AddTimer = m_AddTimer - Time.deltaTime;
            m_CacTime = m_AddTimer / AniDuring;
            m_Color = m_SlotSc[1].ItemImg.color;
            m_Color.a = m_CacTime;
            m_SlotSc[1].ItemImg.color = m_Color;

            if (m_AddTimer <= 0.0f)
            {
                m_SlotSc[1].ItemImg.gameObject.SetActive(false);
            }

        }//if (0.0f < m_AddTimer)
        //---------- 장착된 아이콘이 서서히 사라지게 처리하는 연출

        //---------- 구매불가 텍스트 서서히 사라지게 처리하는 연출
        if (0.0f < m_InfoAddTimer)
        {
            m_InfoAddTimer = m_InfoAddTimer - Time.deltaTime;
            m_CacTime = m_InfoAddTimer / (m_InfoDuring - 1.0f);
            if (1.0f < m_CacTime)
                m_CacTime = 1.0f;
            m_Color = m_InfoTxt.color;
            m_Color.a = m_CacTime;
            m_InfoTxt.color = m_Color;

            if (m_InfoAddTimer <= 0.0f)
            {
                m_InfoTxt.gameObject.SetActive(false);
            }
        }//if (0.0f < m_InfoAddTimer)
        //---------- 구매불가 텍스트 서서히 사라지게 처리하는 연출

    }//void Update()

    bool IsCollSlot(GameObject a_CkObj)  //마우스가 UI 슬롯 오브젝트 위에 있느냐? 판단하는 함수
    {
        Vector3[] v = new Vector3[4];
        a_CkObj.GetComponent<RectTransform>().GetWorldCorners(v);
        if (v[0].x <= Input.mousePosition.x && Input.mousePosition.x <= v[2].x &&
           v[0].y <= Input.mousePosition.y && Input.mousePosition.y <= v[2].y)
        {
            return true;
        }

        return false;
    }

}
