#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VoidspireStudio.FNATS.Cameras;

namespace VoidspireStudio.Editor
{
    public class SecurityCameraCreator
    {
        [MenuItem("GameObject/Security Camera", false, 10)]
        static void CreateCustomCamera(MenuCommand menuCommand)
        {
            GameObject camObj = new("SecurityCamera1");
            camObj.AddComponent<Camera>();
            camObj.AddComponent<SecurityCamera>();
            GameObjectUtility.SetParentAndAlign(camObj, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(camObj, "Create MyCamera");
            Selection.activeObject = camObj;
        }
    }
}
#endif
