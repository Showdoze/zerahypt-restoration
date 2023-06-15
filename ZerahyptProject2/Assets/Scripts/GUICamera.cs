using UnityEngine;

[System.Serializable]
public partial class GUICamera : MonoBehaviour
{
    public static Camera instance;
    public virtual void Awake()
    {
        instance = GetComponent<Camera>();
    }

}