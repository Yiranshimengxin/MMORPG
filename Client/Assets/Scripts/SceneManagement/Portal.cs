using System;
using System.Collections;
using GameCore.Entitys;
using GameCore.Entitys.Atts;
using RPG.Control;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 0.5f;

        AttCharactor mainAttr;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set.");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            //SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;
            mainAttr = GameCore.Entitys.EntityManager.MainPlayer.AttCharactor;
            EntityManager.Instance.RemoveEntity(EntityManager.MainPlayer);
            yield return fader.FadeOut(fadeOutTime);


            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            EntityManager.Instance.CreateEntityByAtt(eEntityType.PLAYER_MAIN, mainAttr.id, mainAttr);
            PlayerController newPlayerController = GameCore.Entitys.EntityManager.MainPlayer.GetRootGameObject().GetComponent<PlayerController>();
            newPlayerController.enabled = false;


            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);


            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            newPlayerController.enabled = true;
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;

                return portal;
            }

            return null;
        }
    }
}