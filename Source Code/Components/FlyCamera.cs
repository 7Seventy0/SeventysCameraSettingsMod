using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FlyCamera : MonoBehaviour 
{

    string CodeBox = "ABC";
    GameObject uiCanvas;
    bool uiIsShown = true;
    float mainSpeed = 10.0f; //regular speed
    float shiftAdd = 20f; //multiplied by how long shift is held.  Basically running
    float maxShift = 100f; //Maximum speed when holdin gshift
    Vector2 rotation = Vector2.zero;
    AudioListener listener;
    private float totalRun= 1.0f;
    public bool allowedToMove = true;
    public bool allowedToFreecam = true;
    bool freecam;
    bool isFPSview = false;

    TabGroup tabGroup;
    ToggleButton lookAtToggle;
    ToggleButton arenaMode;
    Camera fakeCam;
    Text camposText;
    Text camrotText;
    Text camFOVText;
    Text camFreeCamBool;
    Text camIsFpsBool;

    Slider camSpeedSlider;
    Slider camRotSpeedSlider;
    Slider nearclipslider;
    Slider smoothingSlider;

    Button plug;

    Camera shoulderCamera;
    float fov;
    private int _currentWaypointIndex = 0;
    GorillaNetworking.PhotonNetworkController __instance;

    bool isInWayPointMode;

    float nearclip;
    float smoothing;

    GameObject player;
    public List<GameObject> points = new List<GameObject>();
    void Start()
    {
        shoulderCamera = GetComponent<Camera>();
        fov = shoulderCamera.fieldOfView;
       listener = gameObject.AddComponent<AudioListener>();
        listener.enabled = false;
        InvokeRepeating("SlowUpdate", 0, 0.1f);

        player = GameObject.Find("Player");
        StartCoroutine(LateStart());
        nearclipslider = GameObject.Find("NearClipSlider").GetComponent<Slider>();
        GameObject.Find("DRAG AREA").AddComponent<WindowDragLogic>();

    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(2);
        fakeCam = GameObject.Find("FakeCam").GetComponent<Camera>();
        fakeCam.gameObject.GetComponent<AudioListener>().enabled = false    ;

       lookAtToggle = GameObject.Find("LookAtToggle").AddComponent<ToggleButton>();
        lookAtToggle.SetFalse();
       arenaMode = GameObject.Find("ArenaAtToggle").AddComponent<ToggleButton>();
        arenaMode.SetFalse();
        uiCanvas.GetComponent<TabGroup>().PopulateTabList(GameObject.Find("Camera Settings Tab"));
        uiCanvas.GetComponent<TabGroup>().PopulateTabList(GameObject.Find("Camera Info Tab"));
       




    }
    string roomCode ="OJH82D";

    float sliderMoveSpeedXFactor = 5;
    float sliderRotSpeedXFactor = 150;

    bool playerMover;

    void OnPlugClick()
    {
         Application.OpenURL("https://7seventy0.carrd.co/");
    }


    void OnGUI()
    {
        if (uiIsShown)
        {

            CodeBox = GUI.TextArea(new Rect(10, 10, 200, 20), CodeBox, 200);

            if (GUI.Button(new Rect(10, 35, 100, 30), "Join"))
               GorillaNetworking.PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(CodeBox);
        }

    }
    private LoadBalancingClient loadBalancingClient;

    void SlowUpdate()
    {
        //Had to add the logs because i could not find the line that did that bad :(

       // Debug.Log("gfadadawdadwdawdawdadadadadawdwaddadadwawdadadad");
      
        //Debug.Log("######################################################################");
        if(uiCanvas == null)
        {
            uiCanvas = GameObject.Find("SeventysCameraSettingsUI(Clone)");
            GameObject.Find("CameraSettingsTABButton").AddComponent<TabButton>();
            GameObject.Find("infoTABButton").AddComponent<TabButton>();
            GameObject.Find("RoomTABButton").AddComponent<TabButton>();
            GameObject.Find("HelpTABButton").AddComponent<TabButton>();
            uiCanvas.AddComponent<TabGroup>();


            Debug.Log("CODE: 8888988888888888888888898888888888888888888888");

            foreach (TabButton tabButton in GameObject.FindObjectsOfType<TabButton>())
            {
                tabButton.tabGroup = uiCanvas.GetComponent<TabGroup>();
            }

        }
        else
        {
            if (uiIsShown)
            {
                uiCanvas.SetActive(true);
            }
            else
            {
                uiCanvas.SetActive(false);
            }
        }

      
        if(smoothingSlider == null)
        {
           
            smoothingSlider = GameObject.Find("Smoothing Slider").GetComponent<Slider>();
        }
        else
        {
           
            smoothing = smoothingSlider.value / 10;
        }

        //Debug.Log("ddddddddddddddddddddddddddddddddddddddddddddddddd");
        if(camIsFpsBool == null)
        {
            camIsFpsBool = GameObject.Find("CameraisFirstPerson").GetComponent<Text>();
        }
        else
        {
            camIsFpsBool.text = "isInFirstPerson ? = " + isFPSview;
        }

        //Debug.Log("gggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggg");


        if (camFreeCamBool == null)
        {
            camFreeCamBool = GameObject.Find("CameraisFreeCam").GetComponent<Text>();
        }
        else
        {
            camFreeCamBool.text = "isInFreeCam? = " + freecam;
        }


       // Debug.Log("uuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuu");


        if (camposText != null)
        {
            camposText.text = "POS : " + transform.position.ToString();
        }
        else
        {
            camposText = GameObject.Find("CameraPOS TEXT").GetComponent<Text>();
        }
        //
        //Debug.Log("rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr");

        if (camrotText != null)
       {
            camrotText.text = "ROT : " + transform.eulerAngles.ToString();
       }
       else
       {
            camrotText = GameObject.Find("CameraROT TEXT").GetComponent<Text>();
       }

        //Debug.Log("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");

        if (camFOVText != null)
       {
            camFOVText.text = "FOV : " + shoulderCamera.fieldOfView.ToString();
       }
       else
       {
            camFOVText = GameObject.Find("CameraFOV Text").GetComponent<Text>();
       }

      //  Debug.Log("zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
        if (camSpeedSlider == null)
        {
            camSpeedSlider = GameObject.Find("CAMERASPEEDSLIDER").GetComponent<Slider>();
        }
        if(camRotSpeedSlider == null)
        {

            camRotSpeedSlider = GameObject.Find("ROTATIONSPEEDSLIDER").GetComponent<Slider>();
        }
      //  Debug.Log("0000000000000000000000000000000000000000000000000000000000000000000000000000");
        if (plug == null)
        {
            plug = GameObject.Find("PlugButton").GetComponent<Button>();
            plug.onClick.AddListener(OnPlugClick);
        }
       // Debug.Log("666666666666666666666666666666666666666666666666666666666666666666666666");
    }
    float animationtime = 3f;
    void LatchOnToPlayer(int playerToWatch)
    {
        GameObject camera = GameObject.Find("Shoulder Camera");
        if (playerToWatch > GorillaParent.instance.GetComponentsInChildren<VRRig>().Length)
        {
            playerToWatch = GorillaParent.instance.GetComponentsInChildren<VRRig>().Length;
        }
        camera.transform.SetParent( GorillaParent.instance.GetComponentsInChildren<VRRig>()[playerToWatch].transform);
        LeanTween.moveLocal(camera, new Vector3(-0.263f, 2.1212f, - 2.4989f), animationtime)
            .setEaseOutCubic();
        transform.localEulerAngles = Vector3.zero;
        Debug.Log("Now spectating " + GorillaParent.instance.GetComponentsInChildren<VRRig>()[playerToWatch].gameObject.GetComponent<PhotonView>().Controller);
    }
    void LatchOnToPlayerHead(int playerToWatch)
    {
        GameObject camera = GameObject.Find("Shoulder Camera");
        
        if (playerToWatch > GorillaParent.instance.GetComponentsInChildren<VRRig>().Length)
        {
            playerToWatch = GorillaParent.instance.GetComponentsInChildren<VRRig>().Length;
        }
        camera.transform.SetParent(GorillaParent.instance.GetComponentsInChildren<VRRig>()[playerToWatch].head.rigTarget);
        LeanTween.moveLocal(camera, new Vector3(0, .12f, 0f), animationtime)
            .setEaseOutCubic();
        transform.localEulerAngles = Vector3.zero;

        Debug.Log("Now spectating " + GorillaParent.instance.GetComponentsInChildren<VRRig>()[playerToWatch].gameObject.GetComponent<PhotonView>().Controller + " In first person!");
        
    }


    void Update () 
    {

        nearclip = nearclipslider.value / 2;
        GetComponent<Camera>().nearClipPlane = Mathf.Clamp(nearclip,0.01f,3);

        if (Keyboard.current.f8Key.wasPressedThisFrame)
        {

        }
        
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {

            uiIsShown = !uiIsShown;

        }
        

        if (Keyboard.current.digit1Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayer(1);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayer(2);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayer(3);
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayer(4);
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayer(5);
        }
        if (Keyboard.current.digit6Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayer(6);
        }
        if (Keyboard.current.digit7Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayer(7);
        }
        if (Keyboard.current.digit8Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayer(8);
        }
        if (Keyboard.current.digit9Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayer(9);
        }
        if (Keyboard.current.digit0Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed)
        {
            transform.SetParent(null);
            isFPSview = true;
        }

        if (lookAtToggle.state)
        {
            Vector3 lookDirection = Camera.main.transform.position - transform.position;
            lookDirection.Normalize();

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), smoothing);
        }
        if (arenaMode.state && freecam)
        {
            if(Vector3.Distance(new Vector3(-83.284f, 29.6937f, -83.9702f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-83.284f, 29.6937f, -83.9702f);
            }



            if (Vector3.Distance(new Vector3(-45.3531f, 26.9283f, - 78.6759f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-45.3531f, 26.9283f, -78.6759f);
            }


            if (Vector3.Distance(new Vector3(-36.7909f, 28.5586f, -46.6073f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-36.7909f, 28.5586f, -46.6073f);
            }


            if (Vector3.Distance(new Vector3(-58.9439f, 17.6019f, -63.0056f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-58.9439f, 17.6019f, -63.0056f);
            }

            if (Vector3.Distance(new Vector3(-74.6604f, 28.554f, -35.429f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-74.6604f, 28.554f, -35.429f);
            }


            if (Vector3.Distance(new Vector3(-53.4374f, 7.1665f, -35.4395f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-53.4374f, 7.1665f, -35.4395f);
            }

            if (Vector3.Distance(new Vector3(-39.3977f, 3.4004f, -72.7632f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-39.3977f, 3.4004f, -72.7632f);
            }


            if (Vector3.Distance(new Vector3(-58.7199f, 9.7304f, -84.6619f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-58.7199f, 9.7304f, -84.6619f);
            }


            if (Vector3.Distance(new Vector3(-75.0865f, 7.9192f, -72.0579f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-75.0865f, 7.9192f, -72.0579f);
            }


            if (Vector3.Distance(new Vector3(-25.7918f, 3.0245f, -51.0018f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-25.7918f, 3.0245f, -51.0018f);
            }


            if (Vector3.Distance(new Vector3(-40.5545f, 29.2187f, -37.0431f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-40.5545f, 29.2187f, -37.0431f);
            }

            if (Vector3.Distance(new Vector3(-78.7415f, 26.6295f, -67.8135f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-78.7415f, 26.6295f, -67.8135f);
            }

            if (Vector3.Distance(new Vector3(-79.4927f, 43.8614f, -46.8747f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-79.4927f, 43.8614f, -46.8747f);
            }

            if (Vector3.Distance(new Vector3(-30.5946f, 32.9179f, -73.857f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-30.5946f, 32.9179f, -73.857f);
            }


            if (Vector3.Distance(new Vector3(-73.6739f, 43.1266f, -66.6516f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-73.6739f, 43.1266f, -66.6516f);
            }

            if (Vector3.Distance(new Vector3(-42.8177f, 18.3499f, -53.4773f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-42.8177f, 18.3499f, -53.4773f);
            }

            if (Vector3.Distance(new Vector3(-30.6846f, 27.3545f, -71.3198f), GorillaTagger.Instance.transform.position) < 10)
            {
                transform.position = new Vector3(-30.6846f, 27.3545f, -71.3198f);
            }

        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame && Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayerHead(1);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame && Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayerHead(2);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame && Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayerHead(3);
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame && Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayerHead(4);
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame && Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayerHead(5);
        }
        if (Keyboard.current.digit6Key.wasPressedThisFrame && Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayerHead(6);
        }
        if (Keyboard.current.digit7Key.wasPressedThisFrame && Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayerHead(7);
        }
        if (Keyboard.current.digit8Key.wasPressedThisFrame && Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayerHead(8);
        }
        if (Keyboard.current.digit9Key.wasPressedThisFrame && Keyboard.current.shiftKey.isPressed)
        {
            LatchOnToPlayerHead(9);
        }



        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            isFPSview = !isFPSview;
           
            if (isFPSview)
            {
                transform.SetParent(null);
            }
            

        }

        if (isFPSview)
        {
            transform.position = Camera.main.transform.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Camera.main.transform.rotation, smoothing);
            
            listener.enabled = false;
        }
        if (!isFPSview)
        {
            allowedToMove = true;
        }
        if (Keyboard.current.zKey.isPressed)
        {
            LeanTween.rotateLocal(gameObject, Vector3.zero, 0.3f);
        }

        
        if (playerMover)
        {
            if (freecam)
            {
            GameObject.Find("Player").transform.position = transform.position;
            GameObject.Find("Player").transform.localScale = Vector3.zero;

            }   
        }
        else
        {
            GameObject.Find("Player").transform.localScale = Vector3.one;
        }


        //Keyboard commands
        float f = 0.0f;
        if (allowedToMove)
        {
            Vector3 p = GetBaseInput();
            if (p.sqrMagnitude > 0)
            { // only move while a direction key is pressed
                if (Keyboard.current.leftShiftKey.isPressed)
                {
                    totalRun += Time.deltaTime;
                    p = p * totalRun * shiftAdd;
                    p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                    p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                    p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
                }
                else
                {
                    totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                    p = p * mainSpeed;
                }

                p = p * Time.deltaTime;
                Vector3 newPosition = transform.position;
                if (Keyboard.current.spaceKey.isPressed)
                { 
                    transform.Translate(p);
                    newPosition.x = transform.position.x;
                    newPosition.z = transform.position.z;
                    newPosition.y = transform.position.y;
                    transform.position = newPosition;
                }
                else
                {
                    transform.Translate(p);
                }
            }
        }
        if(GorillaNetworking.PhotonNetworkController.Instance != null) // Change instance to Instance when the new update hits (should be fine: Tested 4/27/2022)
        {
          __instance = GorillaNetworking.PhotonNetworkController.Instance;
            __instance.disableAFKKick = true;
        }
        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            __instance.AttemptToJoinSpecificRoom(roomCode);
            Debug.Log("Joining Room Code :" + roomCode);
        }
        if (Mouse.current.scroll.ReadValue().y < 0f)
        {
            fov += 0.5f * 10;
           fov = Mathf.Clamp(fov, 5, 165);
        }
        if (Mouse.current.scroll.ReadValue().y >  0f )
        {
            fov -= 0.5f * 10;
            fov = Mathf.Clamp(fov, 5, 165);
        }

        shoulderCamera.fieldOfView = Mathf.Lerp(shoulderCamera.fieldOfView, fov, 0.08f);

        if (Keyboard.current.leftArrowKey.isPressed)

        {
            rotation.y -= 1 * camRotSpeedSlider.value * sliderRotSpeedXFactor * Time.deltaTime;
            transform.localEulerAngles = rotation;
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            rotation.y += 1 * camRotSpeedSlider.value * sliderRotSpeedXFactor * Time.deltaTime;
            transform.localEulerAngles = rotation;

        }
        if (Keyboard.current.upArrowKey.isPressed)
        {
            rotation.x -= 1 * camRotSpeedSlider.value * sliderRotSpeedXFactor * Time.deltaTime;
            transform.localEulerAngles = rotation;
        }
        if (Keyboard.current.downArrowKey.isPressed)
        {
            rotation.x += 1 * camRotSpeedSlider.value * sliderRotSpeedXFactor * Time.deltaTime;
            transform.localEulerAngles = rotation;

        }



        if (PhotonNetwork.CurrentRoom != null)
        {
            if (PhotonNetwork.CurrentRoom.IsVisible)
            {
                allowedToFreecam = false;
            }
            else
            {
                allowedToFreecam = true;
            }
        }



        if (allowedToFreecam)
        {
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                freecam = !freecam;
                StartCoroutine(WhatToDo());
            }

            
        }
        else
        {
            if(gameObject.transform.parent == null && !isFPSview)
            {
                gameObject.transform.SetParent(Camera.main.transform, false);
                transform.localPosition = new Vector3(0.2412f, 0.4459f, -1.6936f);
                transform.localEulerAngles = Vector3.zero;
            }
        }

        //Mouse  camera angle done.  

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            NewPoint();

        }

        if (Keyboard.current.iKey.isPressed)
        {
            DeletePoints();

        }

        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
           isInWayPointMode = !isInWayPointMode;    

        }

        if (isInWayPointMode)
        {
        Transform pointTransform = points.ToArray()[_currentWaypointIndex].transform;
        if (Vector3.Distance(transform.position, pointTransform.position) < 1f)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % points.ToArray().Length;
        }
        else
        {

            transform.position = Vector3.Lerp(
                transform.position,
                pointTransform.position,
                1 * Time.deltaTime);


                transform.rotation = Quaternion.Slerp(
    transform.rotation,
    pointTransform.rotation,
    1 * Time.deltaTime);
            }
        }


    }

    IEnumerator WhatToDo()
    {
        if (!freecam)
        {
            gameObject.transform.SetParent(Camera.main.transform);
            transform.localPosition = new Vector3(0.2412f, 0.4459f, -1.6936f);
            listener.enabled = false;
        }
       
        if (freecam)
        {
            gameObject.transform.SetParent(null, true);
            transform.localEulerAngles = new Vector3(gameObject.transform.localEulerAngles.x, gameObject.transform.localEulerAngles.y, 0);
            listener.enabled = true;
        }
        yield return new WaitForSeconds(0.1f);
    }

    void DeletePoints()
    {
        foreach (var point in points)
        {
            Destroy(point.gameObject);
            points.Remove(point);   

            Debug.Log(points.Count);
        }
    }

    void NewPoint()
    {
        if (freecam)
        {
            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.transform.position = transform.position;
            point.transform.rotation = transform.rotation;
            point.GetComponent<SphereCollider>().enabled = false;
            points.Add(point);
            Debug.Log(points.ToArray().Length);
        }
    }
     
    private Vector3 GetBaseInput() { //returns the basic values, if it's 0 than it's not active.


        Vector3 p_Velocity = new Vector3();
        if (Keyboard.current.wKey.isPressed){
            p_Velocity += new Vector3(0, 0 , 1 * camSpeedSlider.value * sliderMoveSpeedXFactor);
        }
        if (Keyboard.current.sKey.isPressed)
        {
            p_Velocity += new Vector3(0, 0, -1 * camSpeedSlider.value * sliderMoveSpeedXFactor);
        }
        if (Keyboard.current.aKey.isPressed)
        {
            p_Velocity += new Vector3(-1 * camSpeedSlider.value * sliderMoveSpeedXFactor, 0, 0);
        }
        if (Keyboard.current.dKey.isPressed)
        {
            p_Velocity += new Vector3(1 * camSpeedSlider.value * sliderMoveSpeedXFactor, 0, 0);
        }
        if (Keyboard.current.leftCtrlKey.isPressed)
        {
            p_Velocity += new Vector3(0, -.5f * camSpeedSlider.value * sliderMoveSpeedXFactor, 0);
        }
        if (Keyboard.current.spaceKey.isPressed)
        {
            p_Velocity += new Vector3(0, .5f * camSpeedSlider.value * sliderMoveSpeedXFactor, 0);
        }

        return p_Velocity;
    }
}