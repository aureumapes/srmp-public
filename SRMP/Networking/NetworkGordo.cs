using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMultiplayer.Networking
{
    public class NetworkGordo : MonoBehaviour
    {
        public string ID { get { return Gordo.id == null ? Gordo.GetComponentInParent<GadgetSite>(true).id : Gordo.id;  } }
        public GordoEat Gordo;
        public NetworkRegion Region;

        private void OnDestroy()
        {
            Globals.Gordos.Remove(ID);
        }

        public void Eat(Vector3 position, Quaternion rotation)
        {
            if (Gordo.eatFX != null)
            {
                SRBehaviour.SpawnAndPlayFX(Gordo.eatFX, position, rotation);
            }
            if (Gordo.eatCue != null)
            {
                SECTR_AudioSystem.Play(Gordo.eatCue, position, false);
            }
        }

        public void Burst()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ReachedTarget());
            }
            else
            {
                Gordo.gameObject.SetActive(false);
                Gordo.SetEatenCount(-1);
            }
        }

        private IEnumerator ReachedTarget()
        {
            Gordo.WillStartBurst();
            Gordo.GetComponent<GordoFaceAnimator>().SetTrigger("Strain");
            SECTR_AudioSystem.Play(Gordo.strainCue, Gordo.transform.position, false);
            yield return new WaitForSeconds(2f);
            SECTR_AudioSystem.Play(Gordo.burstCue, Gordo.transform.position, false);
            if (Gordo.destroyFX != null)
            {
                GameObject gameObject = SRBehaviour.SpawnAndPlayFX(Gordo.destroyFX, Gordo.transform.position + Vector3.up * 2f, Gordo.transform.rotation);
                Identifiable component = Gordo.gameObject.GetComponent<Identifiable>();
                Color[] colors = SlimeUtil.GetColors(Gordo.gameObject, (component != null) ? component.id : Identifiable.Id.NONE, true);
                RecolorSlimeMaterial[] componentsInChildren = gameObject.GetComponentsInChildren<RecolorSlimeMaterial>();
                for (int i = 0; i < componentsInChildren.Length; i++)
                {
                    componentsInChildren[i].SetColors(colors[0], colors[1], colors[2]);
                }
            }
            Gordo.DidCompleteBurst();
            Gordo.gameObject.SetActive(false);
            Gordo.SetEatenCount(-1);
            yield break;
        }
    }
}
