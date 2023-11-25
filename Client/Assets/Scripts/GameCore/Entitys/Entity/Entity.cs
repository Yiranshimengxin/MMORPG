using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using GameCore.Entitys.Atts;
using System.Xml;
using RPG.Attributes;
using RPG.Stats;
using System.Security.Cryptography;

namespace GameCore.Entitys
{
    public class Entity
    {
        protected GameObject mRootGameObject;
        protected Transform mRootTransform;

        protected eEntityType mEntityType;

        public AttCharactor AttCharactor { get; set; }

        public int GetID()
        {
            return AttCharactor.id;
        }

        public virtual eEntityType GetEntityType()
        {
            return mEntityType;
        }

        public virtual void OnCreate()
        {
            
        }

        public virtual void OnDestory()
        {

        }

        public Vector3 GetPosition()
        {
            return mRootTransform.position;
        }

        public void SetPosition(Vector3 position)
        {
            mRootTransform.position = position;
        }

        public void SetPosition(float x, float y, float z)
        {
            mRootTransform.position = new Vector3(x, y, z);
        }

        public void SetForward(Vector3 forward)
        {
            mRootTransform.forward = forward;
        }
        public void SetForward(float x, float y, float z)
        {
            mRootTransform.forward = new Vector3(x, y, z);
        }

        public void SetLocalScale(Vector3 localScale)
        {
            mRootTransform.localScale = localScale;
        }


        public Vector3 GetForward()
        {
            return mRootTransform.forward;
        }

        public GameObject GetRootGameObject() { return mRootGameObject; }
        public Transform GetRootTransform() { return mRootTransform; }
    }
}