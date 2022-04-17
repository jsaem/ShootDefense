using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigBox : MonoBehaviour
{
    public Button m_OK_Btn = null;
    public Button m_Close_Btn = null;

    public InputField IDInputField = null;

    public Toggle m_Sound_Toggle = null;
    public Slider m_Sound_Slider = null;

    HeroCtrl m_refHero = null;

    // Start is called before the first frame update
    void Start()
    {
        if (m_OK_Btn != null)
            m_OK_Btn.onClick.AddListener(OKBtnFunction);

        if (m_Close_Btn != null)
            m_Close_Btn.onClick.AddListener(CloseBtnFunction);

        //체크 상태가 변경되었을 때 호출되는 함수를 대기하는 코드
        if (m_Sound_Toggle != null)
            m_Sound_Toggle.onValueChanged.AddListener(SoundOnOff);

        //슬라이드 상태가 변경 되었을 때 호출되는 함수 대기하는 코드
        if (m_Sound_Slider != null)
            m_Sound_Slider.onValueChanged.AddListener(SliderChanged);

        GameObject a_HeroObj = GameObject.Find("HeroRoot");
        if(a_HeroObj != null)
        {
            m_refHero = a_HeroObj.GetComponent<HeroCtrl>();
        }

        //--- 체크상태, 슬라이드상태, 닉네임 로딩 후 UI컨트롤에 적용
        int a_SoundOnOff = PlayerPrefs.GetInt("SoundOnOff", 1);
        if (m_Sound_Toggle != null)
        {
            //m_Sound_Toggle.isOn = (a_SoundOnOff == 1) ? true : false;
            if (a_SoundOnOff == 1)
                m_Sound_Toggle.isOn = true;
            else
                m_Sound_Toggle.isOn = false;
        }

        if (m_Sound_Slider != null)
            m_Sound_Slider.value = PlayerPrefs.GetFloat("SoundVolume", 1.0f);

        Text a_Placeholder = null;
        if(IDInputField != null)
        {
            //IDInputField 기준으로 하위에 있는 오브젝트 중 "Placeholder" 이름의 오브젝트를 찾는다.
            Transform a_PLHTr = IDInputField.transform.Find("Placeholder");
            a_Placeholder = a_PLHTr.GetComponent<Text>();
            if(a_Placeholder != null)
                a_Placeholder.text = PlayerPrefs.GetString("UserNick", "User");
        }//if(IDInputField != null)
        //--- 체크상태, 슬라이드상태, 닉네임 로딩 후 UI컨트롤에 적용



    }//void Start()

    // Update is called once per frame
    void Update()
    {
        
    }

    void OKBtnFunction()
    {
        ////----- 체크상태 저장 ("확인" 버튼 누를 때만 저장)
        //if (m_Sound_Toggle != null)
        //{
        //    if(m_Sound_Toggle.isOn == true)
        //        PlayerPrefs.SetInt("SoundOnOff", 1);
        //    else
        //        PlayerPrefs.SetInt("SoundOnOff", 0);
        //}//if (m_Sound_Toggle != null)

        ////----- 슬라이드 상태 저장 ("확인" 버튼 누를 때만 저장)
        //if(m_Sound_Slider != null)       
        //    PlayerPrefs.SetFloat("SoundVolume", m_Sound_Slider.value);

        //----- 닉네임 주인공 머리위에 적용
        if (IDInputField != null && IDInputField.text.Trim() != "")
        {
            string NickStr = IDInputField.text.Trim();

            if(m_refHero != null && m_refHero.m_NickName != null)
            {
                m_refHero.m_NickName.text = NickStr;
            } //if(m_refHero != null && m_refHero.m_NickName != null)

            GlobalUserData.s_NickName = NickStr;
            PlayerPrefs.SetString("UserNick", NickStr);
        } //if (IDInputField != null && IDInputField.text.Trim() != "")

        Time.timeScale = 1.0f;    //일시정지 풀어주기
        Destroy(this.gameObject);
    }

    void CloseBtnFunction()
    {
        Time.timeScale = 1.0f;    //일시정지 풀어주기
        Destroy(this.gameObject);
    }

    public void SoundOnOff(bool value) //체크 상태가 변경되었을 때 호출되는 함수
    {
        //----- 체크상태 저장
        if (m_Sound_Toggle != null)
        {
            if (value == true)
                PlayerPrefs.SetInt("SoundOnOff", 1);
            else
                PlayerPrefs.SetInt("SoundOnOff", 0);
        }//if (m_Sound_Toggle != null)
    }

    public void SliderChanged(float value)
    {   //value  0.0 ~ 1.0f //슬라이드 상태가 변경 되었을 때 호출되는 함수
        //----- 슬라이드 상태 저장
        if (m_Sound_Slider != null)
            PlayerPrefs.SetFloat("SoundVolume", value);
    }
}
