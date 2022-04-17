using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyRoom_Mgr : MonoBehaviour
{
    public Button m_BackBtn;
    public Button m_ReSet_Save_Btn;

    // Start is called before the first frame update
    void Start()
    {
        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(BackBtnClick);

        if (m_ReSet_Save_Btn != null)
            m_ReSet_Save_Btn.onClick.AddListener(() =>
            {
                GlobalUserData.ClearGameInfo();
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BackBtnClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
    }
}
