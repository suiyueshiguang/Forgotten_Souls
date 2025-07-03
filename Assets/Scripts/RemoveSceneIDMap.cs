using UnityEditor;
using UnityEngine;

namespace GameMain
{
    public class RemoveSceneIDMap : MonoBehaviour
    {
        [MenuItem("Tools/SceneIDMap Fixer")]
        public static void KillSceneIdMap()
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            GameObject obj;

            foreach (GameObject go in allObjects)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(go) && PrefabUtility.IsAnyPrefabInstanceRoot(go))
                {
                    continue; // 此处跳过了预制体实例，如果不想跳过自行注释。
                }

                Component[] components = go.GetComponents<Component>();

                foreach (var component in components)
                {
                    if (component == null)
                    {
                        Debug.Log("GameObject with missing script found: " + go.name, go);
                        break;
                    }
                }
            }

            // 将找到的该物体名字传入即可删除。此处针对HDRP项目中，被隐藏掉的没用的物体“SceneIDMap”
            while (GameObject.Find("SceneIDMap") != null)
            {
                obj = GameObject.Find("SceneIDMap");
                if (obj != null)
                {
                    DestroyImmediate(obj);
                    Debug.Log("Cleared a SceneIDMap instance");
                }
                else
                {
                    Debug.Log("Clear Completed!");
                    break;
                }
            }
        }
    }
}
