using System;
using UnityEngine;

namespace MeshPencil.Demo.CharacterCreator
{
    [Serializable]
    public class CharacterPartData
    {
        public PartType Type;
        public float CameraSize;
        public Transform CreatedPartPivotObject;
        public Transform[] TargetBoneRoot;
    }

    public enum PartType
    {
        None = 0,
        Head = 1,
        Body = 2,
        UpperHand = 3,
        LowerHand = 4,
        UpperLeg = 5,
        LowerLeg = 6,
        Weapon = 7,
        Tail = 8
    }
}
