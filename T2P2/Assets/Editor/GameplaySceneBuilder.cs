#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Coloca mesa, objetos agarrables y sistemas en la escena activa (persistente para el tutor).
/// Menu: T2P2 / Configurar escena de gameplay
/// </summary>
public static class GameplaySceneBuilder
{
    private const string GameplayRootName = "GameplayObjects";
    private const string SystemsRootName = "GameplaySystems";

    [MenuItem("T2P2/Configurar escena de gameplay")]
    public static void SetupFromMenu()
    {
        SetupActiveScene();
        EditorUtility.DisplayDialog("T2P2", "Escena configurada: mesa, objetos Grab_* y GameplaySystems.", "OK");
    }

    public static void SetupSampleSceneBatch()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        SetupActiveScene();
        EditorApplication.Exit(0);
    }

    public static void SetupActiveScene()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid())
        {
            Debug.LogError("No hay escena activa.");
            return;
        }

        EnsureSystems();
        EnsureGameplayObjects();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("T2P2: escena de gameplay guardada.");
    }

    private static void EnsureSystems()
    {
        var systems = GameObject.Find(SystemsRootName);
        if (systems == null)
        {
            systems = new GameObject(SystemsRootName);
        }

        if (systems.GetComponent<DesktopVRRecorder>() == null)
        {
            systems.AddComponent<DesktopVRRecorder>();
        }

        if (systems.GetComponent<VRUiBootstrap>() == null)
        {
            systems.AddComponent<VRUiBootstrap>();
        }
    }

    private static void EnsureGameplayObjects()
    {
        var root = GameObject.Find(GameplayRootName)?.transform;
        if (root == null)
        {
            var rootGo = new GameObject(GameplayRootName);
            root = rootGo.transform;
        }

        EnsureTable(root);
        EnsureGrabObject(root, PrimitiveType.Cube, "Grab_CuboRojo",
            new Vector3(-0.35f, 1.08f, 0.75f), new Vector3(0.14f, 0.14f, 0.14f),
            new Color(0.92f, 0.2f, 0.18f));
        EnsureGrabObject(root, PrimitiveType.Sphere, "Grab_EsferaVerde",
            new Vector3(0f, 1.08f, 0.75f), new Vector3(0.16f, 0.16f, 0.16f),
            new Color(0.2f, 0.78f, 0.28f));
        EnsureGrabObject(root, PrimitiveType.Cylinder, "Grab_CilindroAzul",
            new Vector3(0.35f, 1.08f, 0.75f), new Vector3(0.13f, 0.18f, 0.13f),
            new Color(0.22f, 0.48f, 0.95f));
        EnsureGrabObject(root, PrimitiveType.Cube, "Grab_CuboAmarillo",
            new Vector3(0f, 1.08f, 1.05f), new Vector3(0.12f, 0.12f, 0.12f),
            new Color(0.95f, 0.82f, 0.15f));
    }

    private static void EnsureTable(Transform root)
    {
        var existing = root.Find("Grab_Mesa");
        if (existing != null)
        {
            return;
        }

        var table = GameObject.CreatePrimitive(PrimitiveType.Cube);
        table.name = "Grab_Mesa";
        table.transform.SetParent(root, false);
        table.transform.position = new Vector3(0f, 0.98f, 0.85f);
        table.transform.localScale = new Vector3(1.1f, 0.06f, 0.65f);

        var rb = table.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        ApplyColor(table.GetComponent<Renderer>(), new Color(0.45f, 0.28f, 0.14f));
    }

    private static void EnsureGrabObject(
        Transform root,
        PrimitiveType type,
        string objectName,
        Vector3 position,
        Vector3 scale,
        Color color)
    {
        var existing = root.Find(objectName);
        if (existing != null)
        {
            return;
        }

        var go = GameObject.CreatePrimitive(type);
        go.name = objectName;
        go.transform.SetParent(root, false);
        go.transform.position = position;
        go.transform.localScale = scale;

        var grab = go.AddComponent<DesktopGrabbable>();
        grab.SetColor(color);
    }

    private static void ApplyColor(Renderer renderer, Color color)
    {
        if (renderer == null)
        {
            return;
        }

        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (mat.shader == null || mat.shader.name == "Hidden/InternalErrorShader")
        {
            mat = new Material(Shader.Find("Standard"));
        }

        mat.color = color;
        renderer.sharedMaterial = mat;
    }
}
#endif
