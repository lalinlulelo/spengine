using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pointer : MonoBehaviour
{
    public float m_Distance = 10.0f;
    public LineRenderer m_LineRenderer = null;
    public LayerMask m_EverythingMask = 0;
    public LayerMask m_InteractableMask = 0;
    public UnityAction<Vector3, GameObject> OnPointerUpdate = null;

    private Transform m_CurrentOrigin = null;
    private GameObject m_CurrentObject = null;

    public GameObject Player;

    /// <summary>
    /// 
    /// </summary>
    public OVRScreenFade fade;

    private void Awake()
    {
        PlayerEvents.OnControllerSource += UpdateOrigin;
        PlayerEvents.OnTouchpadDown += ProcessTouchpadDown;
    }

    public void Start()
    {
        SetLineColor();
        Player = GameObject.FindWithTag("Player");

        fade = GameObject.Find("CenterEyeAnchor").gameObject.GetComponent<OVRScreenFade>();
    }

    private void OnDestroy()
    {
        PlayerEvents.OnControllerSource -= UpdateOrigin;
        PlayerEvents.OnTouchpadDown -= ProcessTouchpadDown;
    }

    private void Update()
    {
        Vector3 hitPoint = UpdateLine();

        m_CurrentObject = UpdatePointerStatus();

        if(OnPointerUpdate != null)
        {
            OnPointerUpdate(hitPoint, m_CurrentObject);
        }
    }

    private Vector3 UpdateLine()
    {
        //Create ray
        RaycastHit hit = CreateRaycast(m_EverythingMask);

        //Default end
        Vector3 endPosition = m_CurrentOrigin.position + (m_CurrentOrigin.forward * m_Distance);

        //Check hit
        if(hit.collider != null)
        {
            endPosition = hit.point;
        }

        //Set position
        m_LineRenderer.SetPosition(0, m_CurrentOrigin.position);
        m_LineRenderer.SetPosition(1, endPosition);

        return endPosition;
    }

    private void UpdateOrigin(OVRInput.Controller controller, GameObject controllerObject)
    {
        //Set origin of pointer
        m_CurrentOrigin = controllerObject.transform;

        //Is the laser visible?
        if(controller == OVRInput.Controller.Touchpad)
        {
            m_LineRenderer.enabled = false;
        }
        else
        {
            m_LineRenderer.enabled = true;
        }
    }

    private GameObject UpdatePointerStatus()
    {
        //Create ray
        RaycastHit hit = CreateRaycast(m_InteractableMask);

        //Check hit
        if (hit.collider)
        {
            return hit.collider.gameObject;
        }

        //return
        return null;
    }

    private RaycastHit CreateRaycast(int layer)
    {
        RaycastHit hit;
        Ray ray = new Ray(m_CurrentOrigin.position, m_CurrentOrigin.forward);
        //
        Physics.Raycast(ray, out hit, m_Distance*1000, layer);
        //

        return hit;
    }

    private void SetLineColor()
    {
        if (!m_LineRenderer)
        {
            return;
        }

        Color endColor = Color.white;
        endColor.a = 0.0f;

        m_LineRenderer.endColor = endColor;
    }

    private void ProcessTouchpadDown()
    {
        //If I don't point any object just teleport 10 Units forward
        if (!m_CurrentObject)
        {
            //Fade in every time you click
            fade.fadeOnStart = true;

            Vector3 endPosition = m_CurrentOrigin.position + (m_CurrentOrigin.forward * m_Distance);
            Player.transform.position = Vector3.MoveTowards(m_CurrentOrigin.position, endPosition, m_Distance);

            return;
        }

        //Interactable interactable = m_CurrentObject.GetComponent<Interactable>();
        //interactable.Pressed();

        //Fade in every time you click
        fade.fadeOnStart = true;

        //If I click on one object, then teleport to this object
        Vector3 endPosition2 = m_CurrentObject.transform.position - (m_CurrentOrigin.forward * 3);
        Player.transform.position = Vector3.MoveTowards(m_CurrentOrigin.position, endPosition2, m_Distance*1000);   
    }
}
