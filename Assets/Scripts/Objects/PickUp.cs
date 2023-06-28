using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickUpTypes {SPEED, HEALTH, WORMHOLE }

namespace ItemPickUps
{
    public abstract class PickUpBase : MonoBehaviour
    {
        public GameObject objPrefab;
        [SerializeField] protected float speed;
        public PickUpTypes pickUpType;

        protected PoolingObjectType poolType;

        public abstract void MovePickUp();
        public abstract void PowerUp();

        public abstract void DeletePickUp();

        public abstract void SetPoolType(PoolingObjectType type);
    }
}
