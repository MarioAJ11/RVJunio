using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controles PC para grabar: WASD, raton, mantener clic para agarrar.
/// Debe estar en la escena (GameplaySystems).
/// </summary>
[DefaultExecutionOrder(-200)]
public class DesktopVRRecorder : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float lookSensitivity = 0.12f;
    [SerializeField] private float grabDistance = 6f;
    [SerializeField] private float holdDistance = 1.1f;
    [SerializeField] private float throwForce = 3f;

    private Camera _camera;
    private float _pitch = 10f;
    private float _yaw;
    private Rigidbody _heldBody;
    private Transform _leftHand;
    private Transform _rightHand;
    private bool _controlsEnabled;
    private float _startTime;

    private void Awake()
    {
        if (!ShouldUseDesktopMode())
        {
            enabled = false;
            return;
        }

        DisableMetaInteractionForDesktop();
        DisableOvrInputCapture();
        HideMetaRigVisuals();
    }

    private static bool ShouldUseDesktopMode()
    {
#if UNITY_EDITOR
        return true;
#else
        return !UnityEngine.XR.XRSettings.isDeviceActive;
#endif
    }

    private void Start()
    {
        if (!enabled)
        {
            return;
        }

        _startTime = Time.time;
        SetupCamera();
        CreateSimpleHands();
        _controlsEnabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (!enabled || _camera == null)
        {
            return;
        }

        RefreshControlsEnabled();

        if (!_controlsEnabled)
        {
            return;
        }

        HandleLook();
        HandleMove();
        HandleGrabInput();
        UpdateHeldObject();
        UpdateHandPoses();
    }

    private void RefreshControlsEnabled()
    {
        if (_controlsEnabled)
        {
            return;
        }

        if (Time.time - _startTime > 0.4f
            || IsKeyHeld(Key.W) || IsKeyHeld(Key.A) || IsKeyHeld(Key.S) || IsKeyHeld(Key.D))
        {
            _controlsEnabled = true;
        }
    }

    private static bool IsKeyHeld(Key key)
    {
        var keyboard = Keyboard.current;
        return keyboard != null && keyboard[key].isPressed;
    }

    private static void HideMetaRigVisuals()
    {
        var rigGo = GameObject.Find("[BuildingBlock] Camera Rig");
        if (rigGo != null)
        {
            rigGo.SetActive(false);
        }
    }

    private void DisableOvrInputCapture()
    {
        foreach (var ovr in FindObjectsByType<OVRManager>(FindObjectsSortMode.None))
        {
            ovr.enabled = false;
        }
    }

    private static void DisableMetaInteractionForDesktop()
    {
        foreach (var behaviour in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (behaviour == null)
            {
                continue;
            }

            var ns = behaviour.GetType().Namespace ?? string.Empty;
            if (ns.StartsWith("Oculus.Interaction") || behaviour.GetType().Name == "ActiveStateTracker")
            {
                behaviour.enabled = false;
            }
        }
    }

    private void SetupCamera()
    {
        transform.position = new Vector3(0f, 1.6f, -0.35f);
        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);

        var camGo = new GameObject("DesktopCamera");
        camGo.transform.SetParent(transform, false);
        _camera = camGo.AddComponent<Camera>();
        _camera.tag = "MainCamera";
        _camera.nearClipPlane = 0.05f;
        _camera.farClipPlane = 200f;
        camGo.AddComponent<AudioListener>();
    }

    private void CreateSimpleHands()
    {
        _leftHand = CreateHandVisual("ManoIzq", new Color(0.85f, 0.85f, 0.9f));
        _rightHand = CreateHandVisual("ManoDer", new Color(0.85f, 0.85f, 0.9f));
    }

    private Transform CreateHandVisual(string name, Color color)
    {
        var hand = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        hand.name = name;
        Destroy(hand.GetComponent<Collider>());
        hand.transform.localScale = new Vector3(0.07f, 0.09f, 0.07f);

        var renderer = hand.GetComponent<Renderer>();
        if (renderer != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (mat.shader == null || mat.shader.name == "Hidden/InternalErrorShader")
            {
                mat = new Material(Shader.Find("Standard"));
            }

            mat.color = color;
            renderer.material = mat;
        }

        return hand.transform;
    }

    private void HandleLook()
    {
        var mouse = Mouse.current;
        if (mouse == null)
        {
            return;
        }

        var delta = mouse.delta.ReadValue();
        if (delta.sqrMagnitude < 0.01f)
        {
            return;
        }

        _yaw += delta.x * lookSensitivity;
        _pitch = Mathf.Clamp(_pitch - delta.y * lookSensitivity, -85f, 85f);
        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }

    private void HandleMove()
    {
        var input = Vector3.zero;
        if (IsKeyHeld(Key.W)) input += Vector3.forward;
        if (IsKeyHeld(Key.S)) input += Vector3.back;
        if (IsKeyHeld(Key.A)) input += Vector3.left;
        if (IsKeyHeld(Key.D)) input += Vector3.right;

        if (input.sqrMagnitude < 0.001f)
        {
            return;
        }

        input.Normalize();
        var world = transform.TransformDirection(input);
        world.y = 0f;
        transform.position += world.normalized * (moveSpeed * Time.deltaTime);
    }

    private void HandleGrabInput()
    {
        var mouse = Mouse.current;
        if (mouse == null)
        {
            return;
        }

        var grabHeld = mouse.leftButton.isPressed || mouse.rightButton.isPressed;

        if (_heldBody != null)
        {
            if (!grabHeld)
            {
                ReleaseHeld();
            }

            return;
        }

        if (mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame)
        {
            TryGrab();
        }
    }

    private void TryGrab()
    {
        var ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        var hits = Physics.RaycastAll(ray, grabDistance, ~0, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.name.StartsWith("Grab_Mesa")
                || hit.collider.gameObject.name.StartsWith("Mano"))
            {
                continue;
            }

            var grab = hit.collider.GetComponent<DesktopGrabbable>()
                ?? hit.collider.GetComponentInParent<DesktopGrabbable>();
            if (grab == null)
            {
                continue;
            }

            var body = grab.GetComponent<Rigidbody>();
            if (body == null)
            {
                continue;
            }

            _heldBody = body;
            _heldBody.isKinematic = true;
            _heldBody.linearVelocity = Vector3.zero;
            _heldBody.angularVelocity = Vector3.zero;
            return;
        }
    }

    private void UpdateHeldObject()
    {
        if (_heldBody == null)
        {
            return;
        }

        var holdPoint = _camera.transform.position
            + _camera.transform.forward * holdDistance
            + _camera.transform.up * -0.12f;
        _heldBody.transform.position = holdPoint;
    }

    private void ReleaseHeld()
    {
        if (_heldBody == null)
        {
            return;
        }

        _heldBody.isKinematic = false;
        _heldBody.useGravity = true;
        _heldBody.linearVelocity = _camera.transform.forward * throwForce;
        _heldBody = null;
    }

    private void UpdateHandPoses()
    {
        var mouse = Mouse.current;
        var cam = _camera.transform;
        var leftDown = mouse != null && mouse.leftButton.isPressed;
        var rightDown = mouse != null && mouse.rightButton.isPressed;
        var baseForward = cam.forward * 0.32f + cam.up * -0.08f;

        if (_leftHand != null)
        {
            var offset = leftDown || (_heldBody != null && leftDown)
                ? cam.forward * holdDistance * 0.85f
                : cam.right * -0.22f + baseForward;
            _leftHand.position = cam.position + offset;
            _leftHand.rotation = cam.rotation;
        }

        if (_rightHand != null)
        {
            var offset = rightDown || (_heldBody != null && rightDown)
                ? cam.forward * holdDistance * 0.85f
                : cam.right * 0.22f + baseForward;
            _rightHand.position = cam.position + offset;
            _rightHand.rotation = cam.rotation;
        }
    }
}
