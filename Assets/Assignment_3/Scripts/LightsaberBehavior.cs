using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Avatar;

public class LightsaberBehavior : MonoBehaviour
{
    //Accessing the script that take care of lightsaber's grabbing state
    OVRGrabbable m_GrabState;

    //The Quillon that already installed on the handle. Should be inactive at the begginning of the game
    [SerializeField]
    GameObject m_LightsaberQuillonInstalled;
    //The Quillon module that has not been installed yet.
    [SerializeField]
    GameObject m_LightsaberQuillonModule;
    //The active area to snap the quillon module to the handle
    [SerializeField]
    GameObject m_QuillonConnectZone;
    bool m_QuillonIsInstalled;

    //The Power that already installed on the handle. Should be inactive at the beginning of the game
    [SerializeField]
    GameObject m_LightsaberPowerInstalled;
    //The Power module that has not been installed yet
    [SerializeField]
    GameObject m_LightsaberPowerModule;
    //The active area to snap the power module to the handle
    [SerializeField]
    GameObject m_PowerConnectZone;
    bool m_PowerIsInstalled;

    //bool to signal if the lightsaber is done assembling
    bool m_LightsaberIsAssembled;

    //The blade that already installed on the handle
    [SerializeField]
    GameObject m_LightsaberBlade;
    [SerializeField]
    float m_LightsaberLength = 1f;
    [SerializeField]
    float m_BladeSmooth = 1f;
    bool m_BladeIsActivated;


    AudioSource audioData;


    private void Awake()
    {
        //[TODO]Getting the info of OVRGrabbable
        m_GrabState = GetComponent<OVRGrabbable>();
        m_BladeIsActivated = false;
        m_QuillonIsInstalled = false;
        m_PowerIsInstalled = false;
        m_LightsaberIsAssembled = false;

        audioData = GetComponent<AudioSource>();
        
    }

    private void FixedUpdate()
    {
        //[TODO]Step one: check if the power is connected.
        ConnectingPower();

        //[TODO]Step two: check in the Quillon is connected.
        ConnectingQuillon();

        //[TODO]Once the lightsaber is done assembling, set the blade GameObject active.

        if ((m_PowerIsInstalled) && (m_QuillonIsInstalled)) {
            m_LightsaberIsAssembled = true;
        }

        //[TODO]If the lightsaber is done assembled, change bladeIsActivated after pressing the A button on the R-Controller while the player is grabbing it

        if (m_LightsaberIsAssembled) {
        	if (m_GrabState.isGrabbed) {
            	if (OVRInput.Get(OVRInput.Button.One)) {
                	m_BladeIsActivated = !m_BladeIsActivated;
                	if (m_BladeIsActivated) {
                		audioData.Play();
                	}
                	else {
                		audioData.Pause();
                	}
            	}
        	}
        }

        SetBladeStatus(m_BladeIsActivated);

    }

    void ConnectingPower()
    {
        
        //get the connector state of power
        
        //if it is connected:
       
            //activate the pre-installed power part on the handle
            
            //simply make the power module "invisible" by switching off its mesh renderer
            
            //we dont need the connect area anymore so switch it off
            

        if (gameObject.GetComponentInChildren<LightsaberModuleConnector>().isConnected){
                        m_LightsaberPowerInstalled.SetActive(true);
                        m_LightsaberPowerModule.GetComponent<MeshRenderer>().enabled = false;
                        gameObject.GetComponentInChildren<LightsaberModuleConnector>().enabled = false;
                        m_PowerIsInstalled = true;



        }

        // }
        
    }

    void ConnectingQuillon()
    { 

        if ( !m_QuillonIsInstalled && m_LightsaberPowerInstalled.GetComponentInChildren<LightsaberModuleConnector>().isConnected) {

                        m_LightsaberQuillonInstalled.SetActive(true);
                        m_LightsaberQuillonModule.GetComponent<MeshRenderer>().enabled = false;
                        m_LightsaberPowerInstalled.GetComponentInChildren<LightsaberModuleConnector>().enabled = false;
                        m_QuillonIsInstalled = true;

        }
        
            //same process as in power connection        
    }

    void SetBladeStatus(bool bladeStatus)
    {
        if(!bladeStatus)
        {
            	m_LightsaberBlade.gameObject.transform.localScale = Vector3.Lerp(m_LightsaberBlade.gameObject.transform.localScale, new Vector3(0.0001f, 1.0f, 1.0f), Time.deltaTime * m_BladeSmooth);

            //Lightsaber goes back
        }

        if(bladeStatus)
        {
           //Lightsaber pulls out
                m_LightsaberBlade.gameObject.SetActive(true);
                m_LightsaberBlade.gameObject.transform.localScale = Vector3.Lerp(m_LightsaberBlade.gameObject.transform.localScale, new Vector3(0.5f, 1.0f, 1.0f), Time.deltaTime * m_BladeSmooth);


        }
    }
}
