using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    Text m_RefText = null;
    float totalEnd = 0.0f;

    public AnimationCurve scaleCurve = new AnimationCurve(
         new Keyframe[] { new Keyframe(0.0f, 0.03f), new Keyframe(0.2f, 0.061f) });
    //캐싱버그에 의해서 public의 값이 인스펙터창에 안바뀌면 
    //스크립트 컴포넌트를 제거 Remove했다가 다시 붙혔더니 변경됐습니다!

    public AnimationCurve moveCurve = new AnimationCurve(
        new Keyframe[] { new Keyframe(0.19f, 0.0f), new Keyframe(0.65f, 2.8f) });

    public AnimationCurve alphaCurve = new AnimationCurve(
        new Keyframe[] { new Keyframe(0.40f, 1.0f), new Keyframe(1.0f, 0.0f) });

    //----------------연출 계산용 변수
    float m_StarTime = 0.0f;
    float m_CurTime = 0.0f;

    Vector3 a_CacScVec = Vector3.zero;
    float   a_CacScale = 0.0f;

    Vector3 a_CacCurPos = Vector3.zero;   //이동 효과를 위한 계산용 변수
    float   a_MvOffset = 0.0f;

    Color a_Color = new Color32(200, 0, 0, 255);
    float a_Alpha = 0.0f;
    //----------------연출 계산용 변수

    [HideInInspector] public float m_DamageVal = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        if(m_RefText == null)
            m_RefText = this.gameObject.GetComponentInChildren<Text>();

        if (m_RefText != null)
        {
            a_Color = m_RefText.color;
            m_RefText.text = "-" + m_DamageVal.ToString(); 
        }

        m_StarTime = Time.realtimeSinceStartup;

        //--------------------------종료 시간 계산 코드 
        //Keyframe[] mAlphas;
        //mAlphas = alphaCurve.keys;
        //float alphaEnd = mAlphas[mAlphas.Length - 1].time;

        Destroy(this.gameObject, 1.5f); //연출 끝 시간에 게임오브젝트 제거
        //--------------------------종료 시간 계산 코드 
    }

    // Update is called once per frame
    void Update()
    {
        m_CurTime = Time.realtimeSinceStartup;

        //-- 펀칭 효과 연출
        a_CacScale = scaleCurve.Evaluate(m_CurTime - m_StarTime); 
        a_CacScVec.x = a_CacScale;
        a_CacScVec.y = a_CacScale;
        a_CacScVec.z = 1.0f;
        m_RefText.transform.localScale = a_CacScVec;
        //-- 펀칭 효과 연출

        //-- 이동 효과 연출
        a_MvOffset = moveCurve.Evaluate(m_CurTime - m_StarTime);  //얼마나 진행 되었는지?
        a_CacCurPos.z = a_MvOffset;
        m_RefText.transform.localPosition = a_CacCurPos;
        //-- 이동 효과 연출

        //-- 투명 효과 연출
        a_Alpha = alphaCurve.Evaluate(m_CurTime - m_StarTime);
        a_Color = m_RefText.color;
        a_Color.a = a_Alpha;
        m_RefText.color = a_Color;
        //-- 투명 효과 연출
    }

    public void DamageTxtSpawn(float a_Damage, Color a_Color)
    {
        m_DamageVal = a_Damage;
        m_RefText = this.gameObject.GetComponentInChildren<Text>();
        if (m_RefText != null)
        {
            m_RefText.color = a_Color;
            m_RefText.text = "-" + m_DamageVal.ToString();
        }
    }//public void DamageTxtSpawn(float a_Damage, Color a_Color)

}
