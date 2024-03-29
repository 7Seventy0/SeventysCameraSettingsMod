﻿using BepInEx;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilla;
using System.Collections;
using HarmonyLib;

namespace SeventysCameraSettingsMod
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]

    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;
    
        void OnEnable()
        {
            /* Set up your mod here */
            /* Code here runs at the start and whenever your mod is enabled*/

            HarmonyPatches.ApplyHarmonyPatches();
            Utilla.Events.GameInitialized += OnGameInitialized;
        }
        IEnumerator VerySlowUpdate()
        {
            yield return new WaitForSeconds(5);
            if (shoulderCam.GetComponent<FlyCamera>() == null)
            {
                shoulderCam.AddComponent<FlyCamera>();
            }
        }
        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

            HarmonyPatches.RemoveHarmonyPatches();
            Utilla.Events.GameInitialized -= OnGameInitialized;
        }

        GameObject model;
        GameObject modelInstance;

        GameObject customNameTag;
        GameObject shoulderCam;

        Camera shoulderCamera;
        float fov;
        float cameraMovementSpeed;
        float cameraRotationSpeed;
        Vector3 pos;
        Vector3 rot;
        bool isFPSview;

      IEnumerator SeventysStart()
        {

            
            var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SeventysCameraSettingsMod.Assets.scsm");
            var bundleLoadRequest = AssetBundle.LoadFromStreamAsync(fileStream);
            yield return bundleLoadRequest;

            var myLoadedAssetBundle = bundleLoadRequest.assetBundle;
            if (myLoadedAssetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                yield break;
            }

            var assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync<GameObject>("SeventysCameraSettingsUI");
            yield return assetLoadRequest;

            GameObject uiCanvas = assetLoadRequest.asset as GameObject;
            Instantiate(uiCanvas).AddComponent<UItag>();

            assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync<GameObject>("GorilaSpectator");
            yield return assetLoadRequest;

            model = assetLoadRequest.asset as GameObject;
            modelInstance = Instantiate(model);

            assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync<GameObject>("CustomNameTag");
            yield return assetLoadRequest;

            customNameTag = assetLoadRequest.asset as GameObject;
            Instantiate(customNameTag);



            /////////////////////////////////////////////////////////////////////////////////////////////////////////

            yield return new WaitForSeconds(1);

            modelInstance.transform.SetParent(shoulderCam.transform,false);

            modelInstance.transform.localPosition = Vector3.zero;
            modelInstance.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            shoulderCam.GetComponent<FlyCamera>().model = modelInstance;

            InvokeRepeating("SuperSlowUpdate", 0, 1);
        }

        void SuperSlowUpdate()
        {
            if(FindObjectsOfType<CustomNameTag>() != null)
            {
                foreach (CustomNameTag nameTag in FindObjectsOfType<CustomNameTag>())
                {
                    if (nameTag.nametag == null)
                    {
                        nameTag.nametag = customNameTag;

                    }

                }
            }

        }


        void OnGameInitialized(object sender, EventArgs e)
        {
            StartCoroutine(SeventysStart());    
            cameraMovementSpeed = 0.7f;
            cameraRotationSpeed = 20;
            /* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
            shoulderCam = GameObject.Find("Shoulder Camera");
            shoulderCam.transform.SetParent(Camera.main.transform);
            pos = shoulderCam.transform.localPosition;
            GameObject.Find("CM vcam1").SetActive(false);
            shoulderCamera = shoulderCam.GetComponent<Camera>();
            shoulderCam.AddComponent<FlyCamera>();
            
            
            
        }

        void Update()
        {
            
            //if (Keyboard.current.upArrowKey.isPressed)
            //{
            //    fov += 0.5f;
            //    shoulderCamera.fieldOfView = fov;
            //}
            //if (Keyboard.current.downArrowKey.isPressed)
            //{
            //    fov -= 0.5f;
            //    shoulderCamera.fieldOfView = fov;
            //}

            //if (Keyboard.current.enterKey.isPressed)
            //{
            //    isFPSview = true;

            //}
            //if (Keyboard.current.backspaceKey.isPressed)
            //{
            //    isFPSview = false;
            //}

            //if (isFPSview)
            //{
            //    shoulderCam.transform.position = Camera.main.transform.position;
            //}

            //if (!isFPSview)
            //{

            //    if (Keyboard.current.wKey.isPressed)
            //    {
            //        pos.z += cameraMovementSpeed * Time.deltaTime;
            //        shoulderCam.transform.localPosition = pos;
            //    }
            //    if (Keyboard.current.sKey.isPressed)
            //    {
            //        pos.z -= cameraMovementSpeed * Time.deltaTime;
            //        shoulderCam.transform.localPosition = pos;
            //    }
            //    if (Keyboard.current.aKey.isPressed)
            //    {
            //        pos.x -= cameraMovementSpeed * Time.deltaTime;
            //        shoulderCam.transform.localPosition = pos;
            //    }
            //    if (Keyboard.current.dKey.isPressed)
            //    {
            //        pos.x += cameraMovementSpeed * Time.deltaTime;
            //        shoulderCam.transform.localPosition = pos;
            //    }
            //    if (Keyboard.current.spaceKey.isPressed)
            //    {
            //        pos.y += cameraMovementSpeed * Time.deltaTime;
            //        shoulderCam.transform.localPosition = pos;
            //    }
            //    if (Keyboard.current.leftCtrlKey.isPressed)
            //    {
            //        pos.y -= cameraMovementSpeed * Time.deltaTime;
            //        shoulderCam.transform.localPosition = pos;
            //    }


            //}
            //if (Keyboard.current.eKey.isPressed)
            //{
            //    rot.y -= cameraRotationSpeed * Time.deltaTime;
            //    shoulderCam.transform.localEulerAngles = rot;
            //}
            //if (Keyboard.current.qKey.isPressed)
            //{
            //    rot.y += cameraRotationSpeed * Time.deltaTime;
            //    shoulderCam.transform.localEulerAngles = rot;

            //}
            //if (Keyboard.current.rKey.isPressed)
            //{
            //    rot.x -= cameraRotationSpeed * Time.deltaTime;
            //    shoulderCam.transform.localEulerAngles = rot;

            //}
            //if (Keyboard.current.fKey.isPressed)
            //{
            //    rot.x += cameraRotationSpeed * Time.deltaTime;
            //    shoulderCam.transform.localEulerAngles = rot;

            //}

        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = true;
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = false;
        }
    }
    namespace SolidMonkeys.Patches
    {
        [HarmonyPatch(typeof(VRRig))]
        [HarmonyPatch("Start", MethodType.Normal)]
        internal class VRRigColliderPatch
        {
            public static bool ModEnabled { get; set; }

            private static void Postfix(VRRig __instance)
            {
                if (__instance.isOfflineVRRig)
                    return;

                Photon.Pun.PhotonView photView = __instance.photonView;
                if (photView != null && photView.IsMine)
                    return;

               // __instance.gameObject.AddComponent<CustomNameTag>();

                

                // Debug.Log("Thank you Haunted! if you see this check this out : https://www.youtube.com/watch?v=dQw4w9WgXcQ :) youll like it");
            }
        }
    }
}
