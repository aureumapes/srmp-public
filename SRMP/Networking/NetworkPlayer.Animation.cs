using Lidgren.Network;
using SRMultiplayer.Packets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMultiplayer.Networking
{
    public partial class NetworkPlayer : MonoBehaviour
    {
        float syncInterval = 0.1f;

        float m_AnimatorSpeed;
        float previousSpeed;

        // Note: not an object[] array because otherwise initialization is real annoying
        int[] lastIntParameters;
        float[] lastFloatParameters;
        bool[] lastBoolParameters;
        AnimatorControllerParameter[] parameters;

        // multiple layers
        int[] animationHash;
        int[] transitionHash;
        float[] layerWeight;
        float nextSendTime;

        IEnumerator SetupAnimator()
        {
            yield return new WaitForEndOfFrame();
            // store the m_Animator parameters in a variable - the "Animator.parameters" getter allocates
            // a new parameter array every time it is accessed so we should avoid doing it in a loop
            parameters = m_Animator.parameters
                .Where(par => !m_Animator.IsParameterControlledByCurve(par.nameHash))
                .ToArray();
            lastIntParameters = new int[parameters.Length];
            lastFloatParameters = new float[parameters.Length];
            lastBoolParameters = new bool[parameters.Length];

            animationHash = new int[m_Animator.layerCount];
            transitionHash = new int[m_Animator.layerCount];
            layerWeight = new float[m_Animator.layerCount];
        }

        void FixedUpdate()
        {
            if (!IsLocal || !HasLoaded || !m_Animator.enabled)
                return;

            CheckSendRate();

            for (int i = 0; i < m_Animator.layerCount; i++)
            {
                int stateHash;
                float normalizedTime;
                if (!CheckAnimStateChanged(out stateHash, out normalizedTime, i))
                {
                    continue;
                }

                NetOutgoingMessage writer = CreateMessage();
                writer.Write((ushort)PacketType.PlayerAnimationLayer);
                writer.Write(Globals.LocalID);
                WriteAnimatorLayer(writer, stateHash, normalizedTime, i, layerWeight[i]);
                Send(writer);
            }

            CheckSpeed();
        }

        void WriteAnimatorLayer(NetOutgoingMessage writer, int stateHash, float normalizedTime, int layerNumber, float layerWeight)
        {
            writer.Write(stateHash);
            writer.Write(normalizedTime);
            writer.Write(layerNumber);
            writer.Write(layerWeight);
            WriteParameters(writer);
        }

        public void ReadAnimatorLayer(NetIncomingMessage im)
        {
            if (m_Animator == null) return;

            int stateHash = im.ReadInt32();
            float normalizedTime = im.ReadFloat();
            int layerNumber = im.ReadInt32();
            float layerWeight = im.ReadFloat();

            if (stateHash != 0 && m_Animator.enabled)
            {
                m_Animator.Play(stateHash, layerNumber, normalizedTime);
            }

            m_Animator.SetLayerWeight(layerNumber, layerWeight);

            ReadParameters(im);
        }

        void CheckSpeed()
        {
            float newSpeed = m_Animator.speed;
            if (Mathf.Abs(previousSpeed - newSpeed) > 0.001f)
            {
                previousSpeed = newSpeed;
                NetOutgoingMessage writer = CreateMessage();
                writer.Write((ushort)PacketType.PlayerAnimationSpeed);
                writer.Write(Globals.LocalID);
                WriteAnimatorSpeed(writer, newSpeed);
                Send(writer);
            }
        }

        void WriteAnimatorSpeed(NetOutgoingMessage writer, float newSpeed)
        {
            writer.Write(newSpeed);
        }

        public void ReadAnimatorSpeed(NetIncomingMessage im)
        {
            if (m_Animator == null) return;

            var newSpeed = im.ReadFloat();
            // set m_Animator
            m_Animator.speed = newSpeed;
            m_AnimatorSpeed = newSpeed;
        }

        bool CheckAnimStateChanged(out int stateHash, out float normalizedTime, int layerId)
        {
            bool change = false;
            stateHash = 0;
            normalizedTime = 0;

            float lw = m_Animator.GetLayerWeight(layerId);
            if (Mathf.Abs(lw - layerWeight[layerId]) > 0.001f)
            {
                layerWeight[layerId] = lw;
                change = true;
            }

            if (m_Animator.IsInTransition(layerId))
            {
                AnimatorTransitionInfo tt = m_Animator.GetAnimatorTransitionInfo(layerId);
                if (tt.fullPathHash != transitionHash[layerId])
                {
                    // first time in this transition
                    transitionHash[layerId] = tt.fullPathHash;
                    animationHash[layerId] = 0;
                    return true;
                }
                return change;
            }

            AnimatorStateInfo st = m_Animator.GetCurrentAnimatorStateInfo(layerId);
            if (st.fullPathHash != animationHash[layerId])
            {
                // first time in this animation state
                if (animationHash[layerId] != 0)
                {
                    // came from another animation directly - from Play()
                    stateHash = st.fullPathHash;
                    normalizedTime = st.normalizedTime;
                }
                transitionHash[layerId] = 0;
                animationHash[layerId] = st.fullPathHash;
                return true;
            }
            return change;
        }

        void CheckSendRate()
        {
            float now = Time.time;
            if (IsLocal && syncInterval >= 0 && now > nextSendTime)
            {
                nextSendTime = now + syncInterval;

                NetOutgoingMessage writer = CreateMessage();
                writer.Write((ushort)PacketType.PlayerAnimationParameters);
                writer.Write(Globals.LocalID);
                if (WriteParameters(writer))
                {
                    Send(writer);
                }
            }
        }

        ulong NextDirtyBits()
        {
            ulong dirtyBits = 0;
            for (int i = 0; i < parameters.Length; i++)
            {
                AnimatorControllerParameter par = parameters[i];
                bool changed = false;
                if (par.type == AnimatorControllerParameterType.Int)
                {
                    int newIntValue = m_Animator.GetInteger(par.nameHash);
                    changed = newIntValue != lastIntParameters[i];
                    if (changed)
                        lastIntParameters[i] = newIntValue;
                }
                else if (par.type == AnimatorControllerParameterType.Float)
                {
                    float newFloatValue = m_Animator.GetFloat(par.nameHash);
                    changed = Mathf.Abs(newFloatValue - lastFloatParameters[i]) > 0.001f;
                    // only set lastValue if it was changed, otherwise value could slowly drift within the 0.001f limit each frame
                    if (changed)
                        lastFloatParameters[i] = newFloatValue;
                }
                else if (par.type == AnimatorControllerParameterType.Bool)
                {
                    bool newBoolValue = m_Animator.GetBool(par.nameHash);
                    changed = newBoolValue != lastBoolParameters[i];
                    if (changed)
                        lastBoolParameters[i] = newBoolValue;
                }
                if (changed)
                {
                    dirtyBits |= 1ul << i;
                }
            }
            return dirtyBits;
        }

        bool WriteParameters(NetOutgoingMessage writer, bool forceAll = false)
        {
            ulong dirtyBits = forceAll ? (~0ul) : NextDirtyBits();
            writer.Write(dirtyBits);
            for (int i = 0; i < parameters.Length; i++)
            {
                if ((dirtyBits & (1ul << i)) == 0)
                    continue;

                AnimatorControllerParameter par = parameters[i];
                if (par.type == AnimatorControllerParameterType.Int)
                {
                    int newIntValue = m_Animator.GetInteger(par.nameHash);
                    writer.Write(newIntValue);
                }
                else if (par.type == AnimatorControllerParameterType.Float)
                {
                    float newFloatValue = m_Animator.GetFloat(par.nameHash);
                    writer.Write(newFloatValue);
                }
                else if (par.type == AnimatorControllerParameterType.Bool)
                {
                    bool newBoolValue = m_Animator.GetBool(par.nameHash);
                    writer.Write(newBoolValue);
                }
            }
            return dirtyBits != 0;
        }

        public void ReadParameters(NetIncomingMessage reader)
        {
            if (m_Animator == null) return;

            bool m_AnimatorEnabled = m_Animator.enabled;
            // need to read values from NetworkReader even if m_Animator is disabled

            ulong dirtyBits = reader.ReadUInt64();
            for (int i = 0; i < parameters.Length; i++)
            {
                if ((dirtyBits & (1ul << i)) == 0)
                    continue;

                AnimatorControllerParameter par = parameters[i];
                if (par.type == AnimatorControllerParameterType.Int)
                {
                    int newIntValue = reader.ReadInt32();
                    if (m_AnimatorEnabled)
                        m_Animator.SetInteger(par.nameHash, newIntValue);
                }
                else if (par.type == AnimatorControllerParameterType.Float)
                {
                    float newFloatValue = reader.ReadSingle();
                    if (m_AnimatorEnabled)
                        m_Animator.SetFloat(par.nameHash, newFloatValue);
                }
                else if (par.type == AnimatorControllerParameterType.Bool)
                {
                    bool newBoolValue = reader.ReadBoolean();
                    if (m_AnimatorEnabled)
                        m_Animator.SetBool(par.nameHash, newBoolValue);
                }
            }
        }

        NetOutgoingMessage CreateMessage()
        {
            if (Globals.IsClient) return NetworkClient.Instance.CreateMessage();
            else return NetworkServer.Instance.CreateMessage();
        }

        void Send(NetOutgoingMessage om)
        {
            if (Globals.IsClient) NetworkClient.Instance.Send(om, NetDeliveryMethod.ReliableOrdered, 0);
            else NetworkServer.Instance.SendToAll(om);
        }
    }
}
