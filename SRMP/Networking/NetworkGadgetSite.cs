using SRMultiplayer.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMultiplayer.Networking
{
    public class NetworkGadgetSite : MonoBehaviour
    {
        public GadgetSite Site;
        public NetworkRegion Region;

        public bool IsLocal { get { return Region.IsLocal; } }

        private HydroTurret[] m_Turrets;
        private Quaternion[] m_ActualRotations;
        private Quaternion[] m_PreviousRotations;
        private Quaternion[] m_LatestRotations;
        private float m_LatestPositionTime;
        private float m_InterpolationPeriod = 0.1f;

        private void Update()
        {
            if (Site.HasAttached() && (Site.GetAttachedId() == Gadget.Id.HYDRO_TURRET || Site.GetAttachedId() == Gadget.Id.SUPER_HYDRO_TURRET))
            {
                if(m_Turrets == null)
                {
                    m_Turrets = Site.gameObject.GetComponentsInChildren<HydroTurret>(true);
                    m_ActualRotations = new Quaternion[m_Turrets.Length];
                    m_PreviousRotations = new Quaternion[m_Turrets.Length];
                    m_LatestRotations = new Quaternion[m_Turrets.Length];
                }
                if(IsLocal)
                {
                    if(AnyChange())
                    {
                        m_LatestPositionTime = Time.time + m_InterpolationPeriod;
                        var data = new List<PacketGadgetTurrets.TurretData>();
                        for (int i = 0; i < m_Turrets.Length; i++)
                        {
                            m_ActualRotations[i] = m_Turrets[i].toRotate.rotation;
                            m_PreviousRotations[i] = m_Turrets[i].toRotate.rotation;
                            m_LatestRotations[i] = m_Turrets[i].toRotate.rotation;
                            data.Add(new PacketGadgetTurrets.TurretData()
                            {
                                Index = i,
                                Rotation = m_Turrets[i].toRotate.rotation
                            });
                        }
                        new PacketGadgetTurrets()
                        {
                            ID = Site.id,
                            Turrets = data
                        }.Send(Lidgren.Network.NetDeliveryMethod.Unreliable);
                    }
                }
                else
                {
                    float t = 1.0f - ((m_LatestPositionTime - Time.time) / m_InterpolationPeriod);
                    for (int i = 0; i < m_Turrets.Length; i++)
                    {
                        m_ActualRotations[i] = Quaternion.Slerp(m_PreviousRotations[i], m_LatestRotations[i], t);
                        m_Turrets[i].toRotate.rotation = m_ActualRotations[i];
                    }
                }
            }
            else
            {
                m_Turrets = null;
            }
        }

        private bool AnyChange()
        {
            for(int i = 0; i < m_Turrets.Length; i++)
            {
                if(Vector3.Distance(m_ActualRotations[i].eulerAngles, m_Turrets[i].toRotate.eulerAngles) > 0.1f)
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateTurretRotation(int index, Quaternion rotation)
        {
            m_LatestPositionTime = Time.time + m_InterpolationPeriod;
            m_LatestRotations[index] = rotation;
            m_PreviousRotations[index] = m_ActualRotations[index];
        }
    }
}
