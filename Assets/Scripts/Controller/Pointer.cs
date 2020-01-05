using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Pointer : MonoBehaviour
{
    public float m_Distance = 100.0f;
    public LineRenderer m_LineRenderer = null;
    public LayerMask m_EverythingMask = 0;
    public LayerMask m_InteractableMask = 0;
    public UnityAction<Vector3, GameObject> OnPointerUpdate = null;
    public CelestialManager celestialManager;
    public Text debugText;

    private Transform m_CurrentOrigin = null;
    private GameObject m_CurrentObject = null;
    private bool resetLine = false;
    private float tempDistance;
    private bool isTwoHeld;

    public GameObject Player;
    public GameObject[] line;
    Vector3 lineEndPosition;
    public bool isPressed;
    public GameObject reticle;

    /// <summary>
    /// 
    /// </summary>
    public OVRScreenFade fade;

    //For the button Two/Back
    private enum ControlMode { Teleport, Fly, Accelerate, TeleportFly };
    private ControlMode controlMode = ControlMode.Teleport;
    private float holdCounter = 0;

    //GamepadMovement
    private Vector3 inputVector;
    private Vector3 movementVector;
    private Vector3 accelDirVector;
    private float AccelSpeed { get; set; } = 0f;
    private const float AccelRate = 5f;
    private float MovementSpeed { get; } = 10.0f;
    public Camera c;

    private void Awake()
    {
        PlayerEvents.OnControllerSource += UpdateOrigin;
        PlayerEvents.OnTouchpadDown += ProcessTouchpadDown;
        PlayerEvents.OnTouchpadUp += ProcessTouchpadUp;
        PlayerEvents.OnTouchpadGet += ProcessTouchpadGet;
        PlayerEvents.OnIndexTriggerDown += ProcessIndexTriggerDown;
        PlayerEvents.OnTwoGet += ProcessTwoGet;
        PlayerEvents.OnTwoUp += ProcessTwoUp;
    }

    public void Start()
    {
        SetLineColor();
        Player = GameObject.FindWithTag("Player");

        fade = GameObject.Find("CenterEyeAnchor").gameObject.GetComponent<OVRScreenFade>();
        line = GameObject.FindGameObjectsWithTag("Line");
        controlMode = ControlMode.Teleport;
        isPressed = false;
        resetLine = true;
        tempDistance = m_Distance;
        //debugText.text = "Text";
    }

    private void OnDestroy()
    {
        PlayerEvents.OnControllerSource -= UpdateOrigin;
        PlayerEvents.OnTouchpadDown -= ProcessTouchpadDown;

        PlayerEvents.OnTouchpadUp -= ProcessTouchpadUp;
        //
        PlayerEvents.OnIndexTriggerDown -= ProcessIndexTriggerDown;
        PlayerEvents.OnTwoGet -= ProcessTwoGet;
    }

    private void Update()
    {
        switch (controlMode)
        {
            case ControlMode.Teleport:
                TeleportMovement();
                break;
            case ControlMode.Fly:         
                GamepadMovement();
                break;
            case ControlMode.Accelerate:
                GamepadMovementAccel();
                break;
            case ControlMode.TeleportFly:
                TeleportMovement();
                GamepadMovement();
                break;
        }
        if (celestialManager.isSetScene)
        {
            CheckTasks();
        }
    }

    private void CheckTasks()
    {
        Transform targetBody = celestialManager.gravityManager.TargetBody;
        float dist = Vector3.Distance(Player.transform.position, targetBody.position);
        if (dist <= targetBody.localScale.x * 2 + 1)
        {
            celestialManager.gravityManager.AdvanceTask();
        }
    }

    private void TeleportMovement()
    {
        Vector2 extendVector = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        if (extendVector.y == 0)
        {
            resetLine = true;
        } else if (extendVector.y > 0)
        {
            resetLine = false;
            tempDistance += m_Distance * extendVector.y * 0.2f;
            lineEndPosition = m_CurrentOrigin.position + (m_CurrentOrigin.forward * tempDistance);
        }
        Vector3 hitPoint = UpdateLine();
        m_CurrentObject = UpdatePointerStatus();
        OnPointerUpdate?.Invoke(hitPoint, m_CurrentObject);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private Vector3 UpdateLine()
    {
        //Create ray
        RaycastHit hit = CreateRaycast(m_EverythingMask);

        //Default end
        //Vector3 endPosition = m_CurrentOrigin.position + (m_CurrentOrigin.forward * m_Distance);
        if (resetLine == true)
        {
            lineEndPosition = m_CurrentOrigin.position + (m_CurrentOrigin.forward * m_Distance);
            tempDistance = m_Distance;
        }

        //Check hit
        if (hit.collider != null)
        {
            //endPosition = hit.point;
            lineEndPosition = hit.point;
        }

        //Set position
        m_LineRenderer.SetPosition(0, m_CurrentOrigin.position);
        //m_LineRenderer.SetPosition(1, endPosition);
        m_LineRenderer.SetPosition(1, lineEndPosition);

        //return endPosition;
        return lineEndPosition;
    }

    private void UpdateOrigin(OVRInput.Controller controller, GameObject controllerObject)
    {
        //Set origin of pointer
        m_CurrentOrigin = controllerObject.transform;

        //Is the laser visible?
        if (controller == OVRInput.Controller.Touchpad || controlMode != ControlMode.Teleport)
        {
            m_LineRenderer.enabled = false;
            reticle.SetActive(false);
        }
        else
        {
            m_LineRenderer.enabled = true;
            reticle.SetActive(true);
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
        Ray ray = new Ray(m_CurrentOrigin.position, m_CurrentOrigin.forward);
        //
        Physics.Raycast(ray, out RaycastHit hit, m_Distance * 10, layer);
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
        //Changing the mode 
        if (controlMode == ControlMode.Teleport)
        {
            controlMode = ControlMode.Fly;
            movementVector = new Vector3(0, 0, 0);
            AccelSpeed = 0;
            accelDirVector = new Vector3(0, 0, 0);
            reticle.SetActive(false);
        }
        else if (controlMode == ControlMode.Fly)
        {
            controlMode = ControlMode.Accelerate;
            movementVector = new Vector3(0, 0, 0);
            AccelSpeed = 0;
            accelDirVector = new Vector3(0, 0, 0);
        }
        else if (controlMode == ControlMode.Accelerate)
        {
            controlMode = ControlMode.TeleportFly;
            movementVector = new Vector3(0, 0, 0);
            AccelSpeed = 0;
            accelDirVector = new Vector3(0, 0, 0);

        }
        else if (controlMode == ControlMode.TeleportFly)
        {
            controlMode = ControlMode.Teleport;
        }
    }

    private void ProcessTouchpadGet()
    {
        
    }

    private void ProcessTouchpadUp()
    {
        
    }

    private void DoTeleport()
    {
        isPressed = false;

        //Fade in every time you click
        fade.FadeOutIn();

        //If I don't point any object just teleport 10 Units forward
        if (!m_CurrentObject)
        {
            //Vector3 endPosition = m_CurrentOrigin.position + (m_CurrentOrigin.forward * m_Distance);
            Player.transform.position = Vector3.MoveTowards(m_CurrentOrigin.position, lineEndPosition, Mathf.Infinity);

            return;
        }

        //Interactable interactable = m_CurrentObject.GetComponent<Interactable>();
        //interactable.Pressed();

        //If I click on one object, then teleport to this object
        Vector3 endPosition2 = m_CurrentObject.transform.position - (m_CurrentOrigin.forward * 3);
        Player.transform.position = Vector3.MoveTowards(m_CurrentOrigin.position, endPosition2, Mathf.Infinity);
    }

    private void ProcessIndexTriggerDown()
    {
        if (controlMode == ControlMode.Teleport)
        {
            DoTeleport();

        } else if (controlMode == ControlMode.Accelerate)
        {
            movementVector = new Vector3(0, 0, 0);
            AccelSpeed = 0;
            accelDirVector = new Vector3(0, 0, 0);
        } else if (controlMode == ControlMode.Fly)
        {
            movementVector = new Vector3(0, 0, 0);
            AccelSpeed = 0;
            accelDirVector = new Vector3(0, 0, 0);
        }   
    }

    private void ProcessTwoGet()
    {
        celestialManager.SwapScene();
    }

    private void ProcessTwoUp()
    {
        
    }

    private void GamepadMovement()
    {
        //OVRInput.Update();
        Vector2 moveVector = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        if (moveVector.Equals(new Vector2(0, 0)))
        {
            return;
        }
        Vector3 dir = Vector3.Scale(Camera.main.transform.forward, new Vector3(moveVector.normalized.x, moveVector.normalized.y, 1));
        Player.transform.position += dir * Time.deltaTime * MovementSpeed;
    }

    private void GamepadMovementAccel()
    {
        //OVRInput.Update();
        Vector2 moveVector = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        if (!moveVector.Equals(new Vector2(0, 0)))
        {
            accelDirVector = Vector3.Scale(Camera.main.transform.forward, new Vector3(moveVector.normalized.x, moveVector.normalized.y, 1));
            AccelSpeed += AccelRate * Time.deltaTime;
        }
        Player.transform.position += accelDirVector * Time.deltaTime * AccelSpeed;    
    }
}
