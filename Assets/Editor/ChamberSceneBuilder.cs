#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ChamberSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/PhysicsChamber.unity";

    [MenuItem("Chamber/Build Physics Chamber Scene")]
    public static void Build()
    {
        System.IO.Directory.CreateDirectory("Assets/Scenes");
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        BuildScene();
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        Debug.Log("Physics Chamber scene built at " + ScenePath);
    }

    private static void BuildScene()
    {
        var blue = Mat("Actor Blue", new Color(0.12f, 0.48f, 1f), 0.35f);
        var gold = Mat("Payload Amber", new Color(1f, 0.48f, 0.09f), 0.25f);
        var wall = Mat("Maze Graphite", new Color(0.075f, 0.12f, 0.19f), 0.15f);
        var trim = Mat("Safety Brass", new Color(0.95f, 0.66f, 0.19f), 0.25f);
        var floor = Mat("Chamber Floor", new Color(0.12f, 0.19f, 0.25f), 0.15f);
        var cyan = Mat("Sensor Cyan", new Color(0.08f, 0.9f, 1f), 0.6f);
        var green = Mat("Deposit Green", new Color(0.14f, 0.85f, 0.42f), 0.45f);
        var red = Mat("Gate Red", new Color(0.88f, 0.14f, 0.13f), 0.35f);

        var floorFriction = Physic("Floor Grip", 0.72f, 0.65f);
        var actorFriction = Physic("Actor Glide", 0.08f, 0.08f);
        var payloadFriction = Physic("Payload Grip", 0.48f, 0.38f);

        GameObject manager = new GameObject("Chamber Game Manager");
        manager.AddComponent<ChamberGameManager>();

        var ground = Cube("Arena Floor", new Vector3(0, -0.3f, 0), new Vector3(24, 0.6f, 16), floor);
        ground.GetComponent<BoxCollider>().material = floorFriction;
        Wall("North Boundary", new Vector3(0, 0.8f, 7.8f), new Vector3(24, 2.2f, 0.4f), wall);
        Wall("South Boundary", new Vector3(0, 0.8f, -7.8f), new Vector3(24, 2.2f, 0.4f), wall);
        Wall("West Boundary", new Vector3(-11.8f, 0.8f, 0), new Vector3(0.4f, 2.2f, 16), wall);
        Wall("East Boundary", new Vector3(11.8f, 0.8f, 0), new Vector3(0.4f, 2.2f, 16), wall);

        // Interlocking partitions form a winding lane from the lower-left start to the upper-right exit.
        Wall("Lower Lane Inner", new Vector3(-7.8f, 0.8f, -2.55f), new Vector3(7.6f, 2.2f, 0.35f), wall);
        Wall("Lower Lane Outer", new Vector3(-7.8f, 0.8f, -5.45f), new Vector3(7.6f, 2.2f, 0.35f), wall);
        Wall("First Turn West", new Vector3(-5.9f, 0.8f, -0.45f), new Vector3(0.35f, 2.2f, 4.2f), wall);
        Wall("First Turn East", new Vector3(-3.05f, 0.8f, -0.95f), new Vector3(0.35f, 2.2f, 3.2f), wall);
        Wall("Central Lane South A", new Vector3(-0.75f, 0.8f, 0.55f), new Vector3(4.25f, 2.2f, 0.35f), wall);
        Wall("Central Lane South B", new Vector3(8.25f, 0.8f, 0.55f), new Vector3(4.1f, 2.2f, 0.35f), wall);
        Wall("Central Lane North A", new Vector3(-0.75f, 0.8f, 3.45f), new Vector3(4.25f, 2.2f, 0.35f), wall);
        Wall("Central Lane North B", new Vector3(8.25f, 0.8f, 3.45f), new Vector3(4.1f, 2.2f, 0.35f), wall);
        Wall("Second Turn West", new Vector3(3.05f, 0.8f, 4.9f), new Vector3(0.35f, 2.2f, 2.8f), wall);
        Wall("Second Turn East", new Vector3(5.95f, 0.8f, 4.9f), new Vector3(0.35f, 2.2f, 2.8f), wall);
        Wall("Exit Lane Inner", new Vector3(8.9f, 0.8f, 3.95f), new Vector3(5.4f, 2.2f, 0.35f), wall);
        Wall("Exit Lane Outer", new Vector3(8.9f, 0.8f, 6.85f), new Vector3(5.4f, 2.2f, 0.35f), wall);

        // Gate blocks the center lane. Its BoxCollider moves with the visible door after the sensor fires.
        var gate = Cube("Magnetic Roadblock (slides upward)", new Vector3(1.5f, 0.8f, 2f), new Vector3(0.38f, 2.2f, 2.95f), red);
        gate.AddComponent<SlidingGate>();
        var stripe = Cube("Gate Caution Stripe", new Vector3(1.29f, 1.9f, 2f), new Vector3(0.05f, 0.16f, 2.75f), trim);
        Object.DestroyImmediate(stripe.GetComponent<BoxCollider>());
        stripe.transform.SetParent(gate.transform, true);

        // Sensor is on a side spur near the start; only the ActorController can activate it.
        var sensor = Cube("Actor Sensor Plate", new Vector3(-6.8f, 0.035f, -4f), new Vector3(1.25f, 0.07f, 1.25f), cyan);
        var sensorCollider = sensor.GetComponent<BoxCollider>();
        sensorCollider.isTrigger = true;
        sensor.AddComponent<UnlockSensor>();
        Label("SENSOR", new Vector3(-6.8f, 0.055f, -4f), 0.27f, Color.white);

        // Actor and payload start together in the loading bay. Payload uses higher mass and grip.
        var actor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        actor.name = "Actor (WASD)";
        actor.transform.position = new Vector3(-10f, 0.52f, -4f);
        actor.transform.localScale = Vector3.one * 0.95f;
        actor.GetComponent<Renderer>().sharedMaterial = blue;
        actor.GetComponent<SphereCollider>().material = actorFriction;
        var actorBody = actor.AddComponent<Rigidbody>();
        actorBody.mass = 1.2f;
        actorBody.linearDamping = 0.8f;
        actorBody.angularDamping = 2.2f;
        actor.AddComponent<ActorController>();

        var payload = Cube("Payload (push into deposit)", new Vector3(-8.4f, 0.52f, -4f), Vector3.one * 1.05f, gold);
        payload.GetComponent<BoxCollider>().material = payloadFriction;
        var payloadBody = payload.AddComponent<Rigidbody>();
        payloadBody.mass = 3.8f;
        payloadBody.linearDamping = 0.35f;
        payloadBody.angularDamping = 1.5f;
        payload.AddComponent<PayloadMarker>();
        Label("START / PAYLOAD", new Vector3(-9.2f, 0.035f, -6.55f), 0.3f, Color.white);

        var deposit = new GameObject("Deposit Zone (requires unlocked gate)");
        deposit.transform.position = new Vector3(9.1f, 0.15f, 5.35f);
        var dc = deposit.AddComponent<BoxCollider>();
        dc.isTrigger = true;
        dc.size = new Vector3(2.15f, 1.4f, 2.15f);
        deposit.AddComponent<DepositZone>();
        var pad = Cube("Deposit Pad", new Vector3(9.1f, 0.02f, 5.35f), new Vector3(2.2f, 0.08f, 2.2f), green);
        Object.DestroyImmediate(pad.GetComponent<BoxCollider>());
        Label("DEPOSIT", new Vector3(9.1f, 0.07f, 5.35f), 0.3f, Color.white);

        // Overhead camera keeps the entire maze and route readable while preserving physics depth.
        var camObj = new GameObject("Chamber Camera");
        var cam = camObj.AddComponent<Camera>();
        camObj.tag = "MainCamera";
        cam.orthographic = true;
        cam.orthographicSize = 8.5f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.035f, 0.055f, 0.08f);
        camObj.transform.position = new Vector3(0, 27, 0);
        camObj.transform.rotation = Quaternion.Euler(90, 0, 0);
        camObj.AddComponent<AudioListener>();

        var lightObj = new GameObject("Chamber Lighting");
        var light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.25f;
        lightObj.transform.rotation = Quaternion.Euler(48, -28, 0);
        RenderSettings.ambientLight = new Color(0.52f, 0.58f, 0.68f);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

        Label("PHYSICS CHAMBER  •  FOLLOW THE MAZE", new Vector3(0, 0.04f, -7.05f), 0.35f, new Color(0.62f, 0.83f, 1f));
    }

    private static Material Mat(string name, Color color, float metallic)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var m = new Material(shader) { name = name, color = color };
        if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metallic);
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", 0.42f);
        return m;
    }

    private static PhysicsMaterial Physic(string name, float dynamicFriction, float staticFriction)
    {
        return new PhysicsMaterial(name)
        {
            dynamicFriction = dynamicFriction,
            staticFriction = staticFriction,
            frictionCombine = PhysicsMaterialCombine.Average,
            bounciness = 0f
        };
    }

    private static GameObject Cube(string name, Vector3 position, Vector3 scale, Material material)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.position = position;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = material;
        return go;
    }

    private static GameObject Wall(string name, Vector3 position, Vector3 scale, Material material)
    {
        return Cube(name, position, scale, material);
    }

    private static void Label(string value, Vector3 position, float size, Color color)
    {
        var go = new GameObject(value + " Label");
        go.transform.position = position;
        go.transform.rotation = Quaternion.Euler(90, 0, 0);
        var text = go.AddComponent<TextMesh>();
        text.text = value;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.characterSize = size;
        text.fontSize = 64;
        text.color = color;
        text.fontStyle = FontStyle.Bold;
    }
}
#endif
