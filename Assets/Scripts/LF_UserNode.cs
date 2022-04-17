using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LF_UserNode : MonoBehaviour
{
    [HideInInspector] public int m_UniqueID = -1;           //유저의 고유번호
    [HideInInspector] public string m_UserName = "";        //유저의 닉네임
    [HideInInspector] public int m_ULevel = -1;             //유저의 레벨
    [HideInInspector] public bool m_SelectOnOff = false;    //선택 상태 변수
    public RawImage m_SelectImg;
    public Text m_InfoText;

    // Start is called before the first frame update
    void Start()
    {
        m_SelectOnOff = false;
        this.GetComponent<Button>().onClick.AddListener(OnClickFunc);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void InitInfo(int a_UniqueID, string a_Name, int a_Level)
    {
        m_UniqueID = a_UniqueID;
        m_UserName = a_Name;
        m_ULevel = a_Level;
        m_InfoText.text = a_Name + " : Lv( " + a_Level.ToString() + " )";
    }

    void OnClickFunc() //이 버튼 선택시 선택 상태 표시 함수
    {
        m_SelectOnOff = !m_SelectOnOff;
        if (m_SelectImg != null)
            m_SelectImg.gameObject.SetActive(m_SelectOnOff);
    }
}
