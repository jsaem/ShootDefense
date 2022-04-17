using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lobby_Mgr : MonoBehaviour
{
    public Button m_ShopBtn;
    public Button m_MyRoomBtn;
    public Button m_ExitBtn;

    public Button m_InGameBtn;

    public Text m_GoldTxt;

    //------ Fade In 관련 변수들...
    [Header("-------- Fade In_Out --------")]
    public Image m_FadeImg = null;
    private float AniDuring = 0.8f;  //페이드아웃 연출을 시간 설정
    private bool m_StartFade = false;
    private float m_CacTime = 0.0f;
    private float m_AddTimer = 0.0f;
    private Color m_Color;

    private float m_StVal = 1.0f;
    private float m_EndVal = 0.0f;
    //------ Fade In 관련 변수들...

    string SceneName = "";

    //---------------------------- 환경설정 Dlg 관련 변수
    [Header("-------- DialogBox --------")]
    public Button m_CfgBtn = null;
    public GameObject Canvas_Dialog = null;
    private GameObject m_ConfigBoxObj = null;
    //---------------------------- 환경설정 Dlg 관련 변수

    //-------------- 구글 버튼 관련 변수
    [Header("-------- Google Button --------")]
    public Button m_Google_Btn = null;
    //------ Google MaskScroll 관련 변수들...
    private bool m_Google_ScOnOff = false;
    public Transform m_ScrollTr = null;
    private float m_ScSpeed = 800.0f;
    private Vector3 m_ScOnPos = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 m_ScOffPos = new Vector3(122.0f, 0.0f, 0.0f);
    //------ Google MaskScroll 관련 변수들...
    //-------------- 구글 버튼 관련 변수

    // Start is called before the first frame update
    void Start()
    {
        //------ 로비로 들어올 때 Fade In 설정 
        m_StVal = 1.0f;
        m_EndVal = 0.0f;
        m_FadeImg.gameObject.SetActive(true);
        m_StartFade = true;
        //------ 로비로 들어올 때 Fade In 설정 

        GlobalUserData.LoadGameInfo();

        if (m_ShopBtn != null)
            m_ShopBtn.onClick.AddListener(ShopBtnClick);

        if (m_MyRoomBtn != null)
            m_MyRoomBtn.onClick.AddListener(MyRoomBtnClick);

        if(m_ExitBtn != null)
            m_ExitBtn.onClick.AddListener(
                ()=>
                {
                    //UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
                    Application.Quit(); //<--게임 종료 (실행파일에서만 동작함)
                }
            );

        if (m_InGameBtn != null)
            m_InGameBtn.onClick.AddListener(() => {
               //SceneManager.LoadScene("InGame");
               SceneOut("InGame");
            });

        if (m_GoldTxt != null)
        {
            if(GlobalUserData.s_GoldCount <= 0)
                m_GoldTxt.text = "x 00";
            else
                m_GoldTxt.text = "x " + GlobalUserData.s_GoldCount.ToString("N0");
        }

        //---------------------------- 환경설정 Dlg 관련 구현 부분
        if (m_CfgBtn != null)
            m_CfgBtn.onClick.AddListener(() =>
            {
                if (m_ConfigBoxObj == null)
                    m_ConfigBoxObj = Resources.Load("ConfigBox") as GameObject;

                GameObject a_CfgBoxObj = (GameObject)Instantiate(m_ConfigBoxObj);
                a_CfgBoxObj.transform.SetParent(Canvas_Dialog.transform, false);
                //false 로 해야 로컬 프리팹에 설정된 좌표를 유지한체 차일드로 붙게된다.

                Time.timeScale = 0.0f;
            });
        //---------------------------- 환경설정 Dlg 관련 구현 부분

        if (m_Google_Btn != null)
            m_Google_Btn.onClick.AddListener(() => {

                m_Google_ScOnOff = !m_Google_ScOnOff;
            });

    } //void Start()

    // Update is called once per frame
    void Update()
    {
        //-----m_FadeInOut
        if (m_StartFade == true)
        {
            if (m_CacTime < 1.0f)
            {
                m_AddTimer = m_AddTimer + Time.deltaTime;
                m_CacTime = m_AddTimer / AniDuring;
                m_Color = m_FadeImg.color;
                m_Color.a = Mathf.Lerp(m_StVal, m_EndVal, m_CacTime);
                m_FadeImg.color = m_Color;
                if (1.0f <= m_CacTime)
                {
                    if (m_StVal == 1.0f && m_EndVal == 0.0f)// 들어올 때 
                    {
                        m_Color.a = 0.0f;
                        m_FadeImg.color = m_Color;
                        m_FadeImg.gameObject.SetActive(false);
                        m_StartFade = false;
                    }
                    else if (m_StVal == 0.0f && m_EndVal == 1.0f) //나갈 때
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
                    }
                } //if (1.0f <= m_CacTime)
            } //if (m_CacTime < 1.0f)
        } //if (m_StartFade == true)
        //-----m_FadeInOut   

        //------------- Menu Scroll 연출
        if (m_Google_ScOnOff == false)
        {
            if (m_ScrollTr != null)
            {
                if (m_ScrollTr.localPosition.x < m_ScOffPos.x)
                {
                    m_ScrollTr.localPosition = 
                        Vector3.MoveTowards(m_ScrollTr.localPosition,
                               m_ScOffPos, m_ScSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            if (m_ScrollTr != null)
            {
                if (m_ScOnPos.x < m_ScrollTr.localPosition.x)
                {
                    m_ScrollTr.localPosition = 
                        Vector3.MoveTowards(m_ScrollTr.localPosition,
                            m_ScOnPos, m_ScSpeed * Time.deltaTime);
                }
            }
        }
        //------------- Menu Scroll 연출
    }

    void ShopBtnClick()
    {
        //UnityEngine.SceneManagement.SceneManager.LoadScene("ShopScene");
        SceneOut("ShopScene");
    }

    void MyRoomBtnClick()
    {
        //UnityEngine.SceneManagement.SceneManager.LoadScene("MyRoomScene");
        SceneOut("MyRoomScene");
    }

    void ExitBtnClick()
    {
        //UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
        Application.Quit(); //Editor에서는 동작하지 않음, 실행파일에서만 동작함
    }

    void SceneOut(string a_ScName)
    {
        SceneName = a_ScName;

        m_CacTime = 0.0f;
        m_AddTimer = 0.0f;
        m_StVal = 0.0f;
        m_EndVal = 1.0f;
        m_FadeImg.gameObject.SetActive(true);
        m_StartFade = true;
    }
}
