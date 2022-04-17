using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : G_Singleton<SoundMgr>
{
    [HideInInspector] public AudioSource m_AudioSrc = null;
    Dictionary<string, AudioClip> m_ADClipList = new Dictionary<string, AudioClip>();

    float m_bgmVolume = 0.2f;
    [HideInInspector] public float m_SoundVolume = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBGM(string a_FileName, float fVolume = 0.2f)
    {
        AudioClip a_GAudioClip = null;
        if (m_ADClipList.ContainsKey(a_FileName))
        {
            a_GAudioClip = m_ADClipList[a_FileName] as AudioClip;
        }
        else
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_ADClipList.Add(a_FileName, a_GAudioClip);
        }

        if (m_AudioSrc == null)
            return;

        m_AudioSrc.clip = a_GAudioClip;

    }
}
