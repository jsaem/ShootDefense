using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title_Mgr : MonoBehaviour
{
    public Button m_StartBtn;

    //------ Fade Out 관련 변수들...
    public Image m_FadeImg = null;
    private float AniDuring = 0.8f;  //페이드아웃 연출을 시간 설정
    private bool m_StartFade = false;
    private float m_CacTime = 0.0f;
    private float m_AddTimer = 0.0f;
    private Color m_Color;
    //------ Fade Out 관련 변수들... 

    // Start is called before the first frame update
    void Start()
    {
        if (m_StartBtn != null)
            m_StartBtn.onClick.AddListener(GameStart);

        SoundMgr.Instance.PlayBGM("sound_bgm_title_001", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_StartFade == true)
        {
            if (m_CacTime < 1.0f)
            {
                m_AddTimer = m_AddTimer + Time.deltaTime;
                m_CacTime = m_AddTimer / AniDuring;
                m_Color = m_FadeImg.color;
                m_Color.a = m_CacTime;   
                m_FadeImg.color = m_Color;
                if (1.0f <= m_CacTime)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
                }
            }
        }//if (a_OneClick == false)           
    }

    void GameStart()
    {
        //Debug.Log("GameStart Button Click!!");
        //UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
        m_FadeImg.gameObject.SetActive(true);
        m_StartFade = true;
    }
}
