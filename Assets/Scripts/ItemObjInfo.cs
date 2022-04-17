using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Item_Type
{
    IT_coin,
    IT_bomb,
    IT_armor,
    IT_axe,
    IT_boots,
    IT_helmets,
}

public class ItemValue
{
    public ulong UniqueID = 0;
    public Item_Type m_Item_Type;
    public string m_ItemName = "";
    public int m_ItemLevel = 0; //레벨
    public int m_ItemStar = 0; //성급

    public float m_AddAttack = 0.0f;
    public float m_AddAttSpeed = 0.0f;
    public float m_AddDefence = 0.0f;
    //public MyItemNode m_RefMyItemInfo = null;
}

public class ItemObjInfo : MonoBehaviour
{
    [HideInInspector] public ItemValue m_ItemValue = new ItemValue();

    //외형
    //애니메이션

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void InitItem(Item_Type a_Item_Type, string a_Name,
                                int a_ItemLevel, int a_ItemStar)
    {
        m_ItemValue.UniqueID = GlobalUserData.GetUnique();
        m_ItemValue.m_Item_Type = a_Item_Type;
        m_ItemValue.m_ItemName = a_Name;
        m_ItemValue.m_ItemLevel = a_ItemLevel;
        m_ItemValue.m_ItemStar = a_ItemStar;
    }

}
