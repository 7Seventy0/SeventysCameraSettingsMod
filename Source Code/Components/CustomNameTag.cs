using UnityEngine;
using System.Collections;
using Photon.Pun;
using TMPLoader;
public class CustomNameTag : MonoBehaviour
{
    TMPro.TextMeshProUGUI nametag_Text;
   public  GameObject nametag;
    GameObject Inametag;
    FlyCamera flyCamera;

    void Start()
    {
        StartCoroutine(CoolStart());
    }
    IEnumerator CoolStart()
    {
        yield return new WaitForSeconds(1);
        
            Inametag = Instantiate(nametag);

            Inametag.name = "GorillaTagTag";
            Inametag.transform.SetParent(transform, false);
            Inametag.transform.localPosition = new Vector3(0, 2.4f, 0);
            Inametag.transform.localScale = new Vector3(-0.2f, 0.2f, -0.2f);
            nametag_Text = Inametag.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            nametag_Text.text = gameObject.GetComponent<PhotonView>().Owner.NickName;

           

        flyCamera = FindObjectOfType<FlyCamera>();



        InvokeRepeating("SlowUpdate", 0, 0.2f);
    }

    void SlowUpdate()
    {

        nametag_Text.outlineColor = GetComponentInChildren<SkinnedMeshRenderer>().material.color;

    }
    void Update()
    {
        if(flyCamera == null)
        {
            flyCamera = FindObjectOfType<FlyCamera>();
        }
        if(Inametag != null)
        {
            //Debug.Log("Facing");
        Inametag.transform.LookAt(flyCamera.transform.position);
        }
    }
}