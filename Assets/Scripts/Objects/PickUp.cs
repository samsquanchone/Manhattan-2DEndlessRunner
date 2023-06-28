using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemPickUps
{
    public abstract class PickUpBase : MonoBehaviour
    {
        public GameObject objPrefab;
        [SerializeField] protected float speed;

        public abstract void MovePickUp();
        public abstract void PowerUp();

        public abstract void DeletePickUp();
    }
}
