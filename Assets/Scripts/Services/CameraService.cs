using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserChess
{
    public class CameraService : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}