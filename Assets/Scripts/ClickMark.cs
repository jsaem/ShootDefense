using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMark : MonoBehaviour
{
    HeroCtrl m_RefHero = null;
    Vector3 a_CacVLen = Vector3.zero;

    float m_AddTimer = 0.0f;
    bool m_IsOnOff = true;
    Renderer m_RefRender;
    Color32 m_WtColor = new Color32(255, 255, 255, 200);
    Color32 m_BrColor = new Color32(0, 130, 255, 200);

    // Start is called before the first frame update
    void Start()
    {
        m_RefRender = this.gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //---깜빡임 연출
        if (m_RefRender != null)
        {
            m_AddTimer = m_AddTimer + Time.deltaTime;
            if (0.25f <= m_AddTimer)
            {
                m_IsOnOff = !m_IsOnOff;

                if (m_IsOnOff == true)
                    m_RefRender.material.SetColor("_TintColor", m_WtColor);
                else
                    m_RefRender.material.SetColor("_TintColor", m_BrColor);

                m_AddTimer = 0.0f;
            }//if (0.5f <= m_AddTimer)
        }//if (m_RefRender != null)
        //---깜빡임 연출


        //---클릭마크 끄기
        if (m_RefHero == null)
            return;

        if (gameObject.activeSelf == true)
        {
            if (m_RefHero.m_bMoveOnOff == false)
            {
                gameObject.SetActive(false);
            }

            a_CacVLen = m_RefHero.transform.position - transform.position;
            a_CacVLen.y = 0.0f;
            if (a_CacVLen.magnitude < 1.0f) //주인공이 클릭 마크에 도착했을 때
            {
                gameObject.SetActive(false);
            }

            if (m_RefHero.m_CurHP < 0.0f)// 주인공 사망시에도 꺼줘야 한다.
            {
                gameObject.SetActive(false);
            }
        }//if (gameObject.activeSelf == true)
        //---클릭마크 끄기
    }

    public void ResetEff(Vector3 a_PickVec, HeroCtrl a_RefHero)
    {
        m_RefHero = a_RefHero;

        transform.position = new Vector3(a_PickVec.x, 0.8f, a_PickVec.z);
        gameObject.SetActive(true);
    }

    public void ClickMarkOnOff(bool val)
    {
        gameObject.SetActive(val);
    }
}
