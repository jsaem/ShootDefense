﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemNode : MonoBehaviour
{
    public ulong m_UniqueID = 0;

    [HideInInspector] public bool m_SelOnOff = false;
    public Image m_SelectImg;
    public Text m_TextInfo = null;

    static Texture[] m_ItemImg = null;

    // Start is called before the first frame update
    void Start()
    {
        Button a_SelBtn = gameObject.GetComponent<Button>();
        if (a_SelBtn != null)
            a_SelBtn.onClick.AddListener(() =>
            {
                m_SelOnOff = !m_SelOnOff;
                if (m_SelectImg != null)
                    m_SelectImg.gameObject.SetActive(m_SelOnOff);
            });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetItemRsc(ItemValue a_Node)
    {
        if (a_Node == null)
            return;

        if (a_Node.m_Item_Type < Item_Type.IT_armor
            || Item_Type.IT_helmets < a_Node.m_Item_Type)
            return;

        LoadImage();

        Transform a_FindObj = this.gameObject.transform.Find("RawImage");
        if (a_FindObj != null)
            a_FindObj.GetComponent<RawImage>().texture
                        = m_ItemImg[(int)a_Node.m_Item_Type];

        if (m_TextInfo != null)
            m_TextInfo.text = "Lv(" + a_Node.m_ItemLevel.ToString() + ")";

        m_UniqueID = a_Node.UniqueID;
    }// public void SetItemRsc(ItemValue a_Node, Object a_GameMgr)

    void LoadImage()
    {
        if(m_ItemImg == null)
        {
            m_ItemImg = new Texture[6];

            m_ItemImg[0] = Resources.Load("ItemIcon/itemshop_coin") as Texture;
            m_ItemImg[1] = Resources.Load("ItemIcon/item_bomb") as Texture;
            m_ItemImg[2] = Resources.Load("ItemIcon/armor") as Texture;
            m_ItemImg[3] = Resources.Load("ItemIcon/axe") as Texture;
            m_ItemImg[4] = Resources.Load("ItemIcon/boots") as Texture;
            m_ItemImg[5] = Resources.Load("ItemIcon/helmets") as Texture;
        }//if(m_ItemImg == null)
    }//void LoadImage()
}
