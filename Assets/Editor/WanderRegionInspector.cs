// WanderRegionInspector.cs
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WanderRegion))]
public class WanderRegionInspector : Editor
{
    [Tooltip("Size of the box.")]
    public Vector3 size;

    // Quick reference to target with a typecast:
    private WanderRegion Target
    {
        get
        {
            return (WanderRegion)target;
        }
    }

    // OnSceneGUI method runs when scene GUI is drawing
    // The height of the box display.
    private const float BoxHeight = 10f;

    void OnSceneGUI()
    {
        // Make the handles white:
        Handles.color = Color.white;
        // Draw a wireframe cube resembling the wander region:
        Handles.DrawWireCube(Target.transform.position + (Vector3.up * BoxHeight * 0.5f),
                              new Vector3(Target.size.x, BoxHeight, Target.size.z));
    }
}
