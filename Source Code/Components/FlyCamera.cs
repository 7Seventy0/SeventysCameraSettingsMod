using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UniverseLib.Input;
using UniverseLib.UI;

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

    public GameObject model;

    TabGroup tabGroup;
    ToggleButton lookAtToggle;
    ToggleButton modelToggle;
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

    Transform playerToTrackArenaMode;

    Camera shoulderCamera;
    float fov;
    private int _currentWaypointIndex = 0;
    GorillaNetworking.PhotonNetworkController __instance;

    bool isInWayPointMode;

    float nearclip;
    float smoothing;

    bool primaryR;
    public static UIBase UiBase { get; private set; }
    GameObject player;
    public List<Transform> points = new List<Transform>();
    void Start()
    {
        shoulderCamera = GetComponent<Camera>();
        fov = shoulderCamera.fieldOfView;
        listener = gameObject.AddComponent<AudioListener>();
        listener.enabled = false;
        InvokeRepeating("SlowUpdate", 0, 0.1f);

        player = GameObject.Find("Player");
        StartCoroutine(LateStart());
        
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(1);
        fakeCam = GameObject.Find("FakeCam").GetComponent<Camera>();
        fakeCam.gameObject.GetComponent<AudioListener>().enabled = false    ;
        GameObject.Find("DRAG AREA").AddComponent<WindowDragLogic>();
        nearclipslider = GameObject.Find("NearClipSlider").GetComponent<Slider>();
        arenaMode = GameObject.Find("ArenaAtToggle").AddComponent<ToggleButton>();
        lookAtToggle = GameObject.Find("LookAtToggle").AddComponent<ToggleButton>();
        

        

        yield return new WaitForSeconds(1);
        uiCanvas = GameObject.FindObjectOfType<UItag>().gameObject;
        
        
        uiCanvas.GetComponent<TabGroup>().PopulateTabList(GameObject.Find("Camera Settings Tab"));
      
        uiCanvas.GetComponent<TabGroup>().PopulateTabList(GameObject.Find("Camera Info Tab"));

        lookAtToggle.SetFalse();
        
        arenaMode.SetFalse();
       
        modelToggle = GameObject.Find("ModelToggle").AddComponent<ToggleButton>();

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

    float arenaCameraSwitchCoolDown = 5;
    float arenaCameraSwitchNextSwitch;
    // Pov: free code!
    void ArenaUpdate()
    {

        if(points.Count > 0)
        {
            if (playerToTrackArenaMode == null)
            {
                playerToTrackArenaMode = Camera.main.transform;
            }

            float distanceToClosestEnemy = Mathf.Infinity;
            Point closestEnemy = null;
            Point[] allEnemies = GameObject.FindObjectsOfType<Point>();
            foreach (Point currentEnemy in allEnemies)
            {
                float distanceToEnemy = (currentEnemy.transform.position - playerToTrackArenaMode.position).sqrMagnitude;
                if (distanceToEnemy < distanceToClosestEnemy)
                {
                    distanceToClosestEnemy = distanceToEnemy;
                    closestEnemy = currentEnemy;
                }
            }

            if (transform.position != closestEnemy.transform.position && Time.time > arenaCameraSwitchNextSwitch)
            {
                transform.position = closestEnemy.transform.position;
                arenaCameraSwitchNextSwitch = Time.time + arenaCameraSwitchCoolDown;
            }

        }






    }
    // :)
    void SlowUpdate()
    {
        //Had to add the logs because i could not find the line that did that bad :(

      // Debug.Log("gfadadawdadwdawdawdadadadadawdwaddadadwawdadadad");
      
        //Debug.Log("######################################################################");
        if(uiCanvas == null)
        {
          //  Debug.Log("###########nnnnnnnnnnnnnnnnnnnnnnnnnn#############");
            uiCanvas = GameObject.FindObjectOfType<UItag>().gameObject;
           // Debug.Log("###########nnnnnnooooooooooooooooooooooooooooooonnnnnnn#############");
            GameObject.Find("CameraSettingsTABButton").AddComponent<TabButton>();
            GameObject.Find("infoTABButton").AddComponent<TabButton>();
            GameObject.Find("RoomTABButton").AddComponent<TabButton>();
            GameObject.Find("HelpTABButton").AddComponent<TabButton>();
            uiCanvas.AddComponent<TabGroup>();


          //  Debug.Log("CODE: 8888988888888888888888898888888888888888888888");

            foreach (TabButton tabButton in GameObject.FindObjectsOfType<TabButton>())
            {
                tabButton.tabGroup = uiCanvas.GetComponent<TabGroup>();
            }

        }

        if(uiCanvas != null)
        {
          //  Debug.Log("###########nnnnnnniiiiiiiiiiiiiiiinnnnnnnnnn#############");
            if (uiIsShown)
            {
                uiCanvas.SetActive(true);
            }
            else
            {
                uiCanvas.SetActive(false);
            }
        }




        model.SetActive(modelToggle.state);

        UniversalUI.EventSys.enabled = true;
        
             //   Debug.Log("###########nnnnnnniiiiiiiii55555555555555555555555555iiiiiiinnnnnnnnnn#############");


        if (arenaMode.state && allowedToFreecam)
        {
            ArenaUpdate();

        }

       
    //    Debug.Log("ddddddddddddddddddddddddddddddddddddddddddddddddd");



        //    Debug.Log("###########nnnnnnniiii333333337777777777777777777777777777777777777777#######");
        if (smoothingSlider == null)
        {
        //    Debug.Log("###########nnnnnnniiii33333222222222222222222222222222223333333iiiinnnnnnnnnn#############");
            smoothingSlider = GameObject.Find("SmoothingSlidergaming").GetComponent<UnityEngine.UI.Slider>();
               
        }
        else
        {
           
            smoothing = smoothingSlider.value / 10;
        }
       // Debug.Log("####0000000000000000000000000000000000000000000000000######");
        if (camIsFpsBool == null)
        {
            camIsFpsBool = GameObject.Find("CameraisFirstPerson").GetComponent<Text>();
        }
        else
        {
            camIsFpsBool.text = "isInFirstPerson ? = " + isFPSview;
        }

       // Debug.Log("gggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggg");


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
        //Debug.Log("666666666666666666666666666666666666666666666666666666666666666666666666");
    }
    float animationtime = 3f;
    void LatchOnToPlayer(int playerToWatch)
    {
        if (!arenaMode.state)
        {
            GameObject camera = GameObject.Find("Shoulder Camera");
            if (playerToWatch > GorillaParent.instance.GetComponentsInChildren<VRRig>().Length)
            {
                playerToWatch = GorillaParent.instance.GetComponentsInChildren<VRRig>().Length;
            }
            camera.transform.SetParent(GorillaParent.instance.GetComponentsInChildren<VRRig>()[playerToWatch].transform);
            LeanTween.moveLocal(camera, new Vector3(-0.263f, 2.1212f, -2.4989f), animationtime)
                .setEaseOutCubic();
            transform.localEulerAngles = Vector3.zero;
            Debug.Log("Now spectating " + GorillaParent.instance.GetComponentsInChildren<VRRig>()[playerToWatch].gameObject.GetComponent<PhotonView>().Controller);

        }

    }
    void LatchOnToPlayerHead(int playerToWatch)
    {
        if (!arenaMode.state)
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

        
    }

    void PlayerToTrackArenaMode(int playerToTrack)
    {
        GameObject camera = GameObject.Find("Shoulder Camera");

        if (playerToTrack > GorillaParent.instance.GetComponentsInChildren<VRRig>().Length)
        {
            playerToTrack = GorillaParent.instance.GetComponentsInChildren<VRRig>().Length ;
        }

        playerToTrackArenaMode = GorillaParent.instance.GetComponentsInChildren<VRRig>()[playerToTrack].head.rigTarget;

    }

    

    void Update () 
    {


      
        // Debug.Log("DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD");
        nearclip = nearclipslider.value / 2;
     //  Debug.Log(")))))))))))eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee)))))))))))))))))))))))))))))))))");
        GetComponent<Camera>().nearClipPlane = Mathf.Clamp(nearclip,0.01f,3);
       // Debug.Log("))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))");
        
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {

            uiIsShown = !uiIsShown;
          

        }

     //   Debug.Log("VVVVVVVVVVVVVVVVVVVVVVV///////////////////////////////////////////////////////////////////");
        if (Keyboard.current.digit1Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && !Keyboard.current.leftCtrlKey.isPressed)
        {
            LatchOnToPlayer(1);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && !Keyboard.current.leftCtrlKey.isPressed)
        {
            LatchOnToPlayer(2);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && !Keyboard.current.leftCtrlKey.isPressed)
        {
            LatchOnToPlayer(3);
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && !Keyboard.current.leftCtrlKey.isPressed)
        {
            LatchOnToPlayer(4);
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && !Keyboard.current.leftCtrlKey.isPressed)
        {
            LatchOnToPlayer(5);
        }
        if (Keyboard.current.digit6Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && !Keyboard.current.leftCtrlKey.isPressed)
        {
            LatchOnToPlayer(6);
        }
        if (Keyboard.current.digit7Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && !Keyboard.current.leftCtrlKey.isPressed)
        {
            LatchOnToPlayer(7);
        }
        if (Keyboard.current.digit8Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && !Keyboard.current.leftCtrlKey.isPressed)
        {
            LatchOnToPlayer(8);
        }
        if (Keyboard.current.digit9Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && !Keyboard.current.leftCtrlKey.isPressed)
        {
            LatchOnToPlayer(9);
        }
        if (Keyboard.current.digit0Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && !Keyboard.current.leftCtrlKey.isPressed)
        {
            transform.SetParent(null);
            isFPSview = true;
        }




        if (Keyboard.current.digit1Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && Keyboard.current.leftCtrlKey.isPressed)
        {
            PlayerToTrackArenaMode(1);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && Keyboard.current.leftCtrlKey.isPressed)
        {
            PlayerToTrackArenaMode(2);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && Keyboard.current.leftCtrlKey.isPressed)
        {
            PlayerToTrackArenaMode(3);
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && Keyboard.current.leftCtrlKey.isPressed)
        {
            PlayerToTrackArenaMode(4);
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && Keyboard.current.leftCtrlKey.isPressed)
        {
            PlayerToTrackArenaMode(5);
        }
        if (Keyboard.current.digit6Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && Keyboard.current.leftCtrlKey.isPressed)
        {
            PlayerToTrackArenaMode(6);
        }
        if (Keyboard.current.digit7Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && Keyboard.current.leftCtrlKey.isPressed)
        {
            PlayerToTrackArenaMode(7);
        }
        if (Keyboard.current.digit8Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && Keyboard.current.leftCtrlKey.isPressed)
        {
            PlayerToTrackArenaMode(8);
        }
        if (Keyboard.current.digit9Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && Keyboard.current.leftCtrlKey.isPressed)
        {
            PlayerToTrackArenaMode(9);
        }
        if (Keyboard.current.digit9Key.wasPressedThisFrame && !Keyboard.current.shiftKey.isPressed && Keyboard.current.leftCtrlKey.isPressed)
        {
            playerToTrackArenaMode = Camera.main.transform;
        }

        if(playerToTrackArenaMode == null)
        {
            playerToTrackArenaMode = Camera.main.transform;
        }



          // Debug.Log("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
        if (lookAtToggle.state && !isFPSview)
        {
            Vector3 lookDirection = playerToTrackArenaMode.transform.position - transform.position;
            lookDirection.Normalize();

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), smoothing);
        }
        //Debug.Log("DGBWHJVBDAHJDJDBJHDHBAJVDHWVDHJADVHJHWJ");
 
   //     Debug.Log("ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ");
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


      // Debug.Log("OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
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

     //   Debug.Log("VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV");
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
            point.AddComponent<Point>();
            points.Add(point.transform);
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