using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VCamReference : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcam;
    public CinemachineVirtualCamera VCam
    {
        get { return vcam; }
    }
}
