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
                    continue; // �˴�������Ԥ����ʵ�������������������ע�͡�
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

            // ���ҵ��ĸ��������ִ��뼴��ɾ�����˴����HDRP��Ŀ�У������ص���û�õ����塰SceneIDMap��
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
