using System;
using System.Collections.Generic;
using UnityEngine;
using GameCore;

namespace GameCore.Inputs
{
    public class InputManager : BaseMgr<InputManager>
    {

        public enum InputType
        {
            /// <summary>
            /// 跳跃
            /// </summary>
            Jump,
            /// <summary>
            /// 跳下
            /// </summary>
            JumpDown,
        }
        public enum InputState
        {
            Down,
            Up,
            Hold,
            Click
        }
        public delegate void OnInputCallback(InputType inputType, InputState state);

        private Dictionary<InputType, OnInputCallback> mCommandCallback = new Dictionary<InputType, OnInputCallback>();


        public override void Init(GameObject owner)
        {
            base.Init(owner);

        }



        public void RegCallback(InputType command, OnInputCallback callback)
        {
            if (mCommandCallback.ContainsKey(command))
            {
                mCommandCallback[command] += callback;
            }
            else
            {
                mCommandCallback.Add(command, callback);
            }
        }
        public void UnRegCallback(InputType command, OnInputCallback callback)
        {
            if (mCommandCallback.ContainsKey(command))
            {
                mCommandCallback[command] -= callback;
            }
        }
        private void OnTrigerCommand(InputType command, InputState state)
        {
            if (mCommandCallback.ContainsKey(command))
            {
                mCommandCallback[command](command, state);
            }
            else
            {
                Debug.LogError("InputManager.OnTrigerCommand error ,mCommandDic.ContainsKey(command) == false");
            }
        }


        override public void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (Input.GetKey(KeyCode.S))
                {
                    OnTrigerCommand(InputType.JumpDown, InputState.Click);
                    return;
                }
                OnTrigerCommand(InputType.Jump, InputState.Click);
            }
        }
    }
}
