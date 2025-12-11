using UnityEngine;

[CreateAssetMenu(fileName = "CutSceneCast", menuName = "ScriptableObject/CutSceneCast")]
public class CutSceneCast : ScriptableObject
{
    [System.Serializable]
    public class ActorInfo
    {
        public string key;
        public string actorName;
        public Vector3 position;
        public GameObject actorPrefab;
    }
    
    [System.Serializable]
    public class ObjectInfo
    {
        public string key;
        public string objectName;
        public Vector3 position;
        public GameObject objectPrefab;
    }
    
    public ActorInfo[] actorInfos;
    public ObjectInfo[] objectInfos;
}
