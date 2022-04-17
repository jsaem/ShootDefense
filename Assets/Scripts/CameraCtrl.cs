using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    //-------------- 카메라가 주인공을 따라다니게 하기 위한 변수
    public GameObject m_HeroObj = null;
    Vector3 newPosition = Vector3.zero;
    private float smoothTime = 0.2f;
    private float xVelocity = 0.0f;
    private float zVelocity = 0.0f;
    //-------------- 카메라가 주인공을 따라다니게 하기 위한 변수

    //----- 카메라 줌 인아웃
    private float maxDist   = 50.0f;
    private float minDist   = 5.0f;
    private float zoomSpeed = 1.0f;
    private float distance  = 10.0f;
    //----- 카메라 줌 인아웃

    Camera RefCam = null;

    //------------------- LimitMoveCam(카메라가 지형 밖으로 나갈 수 없도록 막기)
    [HideInInspector] public Vector3 m_GroundMin = Vector3.zero;
    [HideInInspector] public Vector3 m_GroundMax = Vector3.zero;

    [HideInInspector] public Vector3 m_CamWMin = Vector3.zero;
    [HideInInspector] public Vector3 m_CamWMax = Vector3.zero;
    Vector3 m_ScWdHalf = Vector3.zero;

    float a_LmtBdLeft = 0;
    float a_LmtBdTop = 0;
    float a_LmtBdRight = 0;
    float a_LmtBdBottom = 0;
    //------------------- LimitMoveCam(카메라가 지형 밖으로 나갈 수 없도록 막기)

    // Start is called before the first frame update
    void Start()
    {
        RefCam = this.GetComponent<Camera>();

        if (RefCam != null)
            distance = RefCam.orthographicSize;

        //1, 지형의 사이즈
        GameObject a_GroundObj = GameObject.Find("GroundObj");

        Vector3 a_GrdHalfSize = Vector3.zero;
        a_GrdHalfSize.x = a_GroundObj.transform.localScale.x / 2.0f;
        a_GrdHalfSize.y = a_GroundObj.transform.localScale.y / 2.0f;
        a_GrdHalfSize.z = a_GroundObj.transform.localScale.z / 2.0f;

        //--좌측하단 (전체 지형의 꼭지점 구하기)
        m_GroundMin.x = a_GroundObj.transform.position.x - a_GrdHalfSize.x;
        m_GroundMin.y = a_GroundObj.transform.position.y - a_GrdHalfSize.y;
        m_GroundMin.z = a_GroundObj.transform.position.z - a_GrdHalfSize.z;

        //--우측상단 (전체 지형의 꼭지점 구하기)
        m_GroundMax.x = a_GroundObj.transform.position.x + a_GrdHalfSize.x;
        m_GroundMax.y = a_GroundObj.transform.position.y + a_GrdHalfSize.y;
        m_GroundMax.z = a_GroundObj.transform.position.z + a_GrdHalfSize.z;

        //2, 카메라의 위치 화면의 사이즈
        //3, 주인공의 좌표 주인공의 사이즈
    }

    void Update()
    {
        //카메라 화면 좌측하단 코너의 월드 좌표
        m_CamWMin = Camera.main.ViewportToWorldPoint(Vector3.zero);
        //MinX : m_CamWMin.x,  MinZ : m_CamWMin.z

        //카메라 화면 우측상단 코너의 월드 좌표
        m_CamWMax = Camera.main.ViewportToWorldPoint(Vector3.one);
        //MaxX : m_CamWMax.x,  MaxZ : m_CamWMax.z
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //this.transform.position = 
        //                new Vector3(m_HeroObj.transform.position.x,
        //                            this.transform.position.y,
        //                            m_HeroObj.transform.position.z);

        newPosition = transform.position;
        newPosition.x = Mathf.SmoothDamp(transform.position.x,
                    m_HeroObj.transform.position.x, ref xVelocity, smoothTime);
        newPosition.z = Mathf.SmoothDamp(transform.position.z,
                    m_HeroObj.transform.position.z, ref zVelocity, smoothTime);


        //------------------- PC에서만 작동되는 줌인 줌아웃 기능
        //Input.GetAxis("Mouse ScrollWheel") 
        //마우스 휠 땅길때 -0.1f(size가 커지게) //마우스 휠 밀때 +0.1f(size가 작아지게)
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && distance < maxDist)
        {
            distance += zoomSpeed;
            RefCam.orthographicSize = distance;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && distance > minDist)
        {
            distance -= zoomSpeed;
            RefCam.orthographicSize = distance;
        }
        //------------------- PC에서만 작동되는 줌인 줌아웃 기능

        //------------------- LimitMoveCam(카메라가 지형 밖으로 나갈 수 없도록 막기)
        //2, 카메라의 위치 화면의 사이즈(화면의 최소좌표, 최대좌표 구하기
        //카메라 화면 좌측하단 코너의 월드 좌표
        m_CamWMin = Camera.main.ViewportToWorldPoint(Vector3.zero);
        //MinX : m_CamWMin.x,  MinZ : m_CamWMin.z

        //카메라 화면 우측상단 코너의 월드 좌표
        m_CamWMax = Camera.main.ViewportToWorldPoint(Vector3.one);
        //MaxX : m_CamWMax.x,  MaxZ : m_CamWMax.z

        m_ScWdHalf.x = (m_CamWMax.x - m_CamWMin.x) / 2.0f;
        m_ScWdHalf.z = (m_CamWMax.z - m_CamWMin.z) / 2.0f;

        a_LmtBdLeft  = m_GroundMin.x + 4.0f + m_ScWdHalf.x;
        a_LmtBdTop   = m_GroundMin.z + 4.0f + m_ScWdHalf.z;
        a_LmtBdRight  = m_GroundMax.x - 4.0f - m_ScWdHalf.x;
        a_LmtBdBottom = m_GroundMax.z - 4.0f - m_ScWdHalf.z;

        if (newPosition.x < a_LmtBdLeft)
            newPosition.x = a_LmtBdLeft;
  
        if (a_LmtBdRight < newPosition.x)
            newPosition.x = a_LmtBdRight;

        if (newPosition.z < a_LmtBdTop)
            newPosition.z = a_LmtBdTop;

        if (a_LmtBdBottom < newPosition.z)
            newPosition.z = a_LmtBdBottom;
        //------------------- LimitMoveCam(카메라가 지형 밖으로 나갈 수 없도록 막기)

        transform.position = newPosition;
    }
}
