using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public enum Item_Type
//{
//    IT_coin,
//    IT_bomb,
//    IT_armor,
//    IT_axe,
//    IT_boots,
//    IT_helmets,
//}

public class RT_ItemNode : MonoBehaviour
{
    [HideInInspector] public int m_UniqueID = -1;
    [HideInInspector] public string m_ItemName = "";
    [HideInInspector] public int m_Level = -1;
    [HideInInspector] public bool m_SelectOnOff = false;
    public Image m_SelectImg;
    public RawImage m_IconImg;
    public Text m_InfoText;

    // Start is called before the first frame update
    void Start()
    {
        m_SelectOnOff = false;
        this.GetComponent<Button>().onClick.AddListener(OnClickFunc);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitInfo(Object a_GameMgr, int a_UniqueID, Item_Type a_ItemType,
                                                      string a_Name, int a_Level)
    {
        m_UniqueID = a_UniqueID;
        m_ItemName = a_Name;
        m_Level = a_Level;
        m_InfoText.text = "Lv ( " + a_Level.ToString() + " )";

        Shop_Mgr a_ShopMgr = a_GameMgr as Shop_Mgr; //형변환
        if(a_ShopMgr != null)
        {
            m_IconImg.texture = a_ShopMgr.m_ItemImg[(int)a_ItemType];
        }

        //InGame_Mgr a_InGameMgr = a_GameMgr as InGame_Mgr; //형변환
        //if(a_InGameMgr != null)
        //{
        //    m_IconImg.texture = a_InGameMgr.m_ItemImg[(int)a_ItemType];
        //}
    }

    void OnClickFunc() //이 버튼 선택시 선택 상태 표시 함수
    {
        m_SelectOnOff = !m_SelectOnOff;
        if (m_SelectImg != null)
            m_SelectImg.gameObject.SetActive(m_SelectOnOff);
    }
}
