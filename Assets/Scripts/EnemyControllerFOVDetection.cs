using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControllerFOVDetection : MonoBehaviour {
    [SerializeField] Vector3 min, max;
    Vector3 destination;
    [SerializeField] float playerDetectionDistance, playerAttackDistance;
    Transform player;
    float visionAngle;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        RandomDestination();
        StartCoroutine(Patrol());
        StartCoroutine(Alert());
    }

    void Update() {
        
    }

    void RandomDestination() {
        destination = new Vector3(Random.Range(min.x, max.x), 0, Random.Range(min.z, max.z));
        GetComponent<NavMeshAgent>().SetDestination(destination);
        GetComponent<Animator>().SetFloat("velocity", 2);
    }

    IEnumerator Patrol() {
        GetComponent<NavMeshAgent>().SetDestination(destination);
        while (true) {
            if (Vector3.Distance(transform.position, destination) < 1.5f) {
                GetComponent<Animator>().SetFloat("velocity", 0);
                yield return new WaitForSeconds(Random.Range(1f, 3f));
                RandomDestination();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Alert() {
        while (true) {
            if (Vector3.Distance(transform.position, player.position) < playerDetectionDistance) {
                Vector3 vectorPlayer = player.position - transform.position;
                if (Vector3.Angle(vectorPlayer.normalized, transform.forward) < visionAngle) {
                    StopCoroutine(Patrol());
                    StartCoroutine(Attack());
                    break;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Attack() {
        while (true) {
            if (Vector3.Distance(transform.position, player.position) > playerDetectionDistance) {
                StartCoroutine(Patrol());
                StartCoroutine(Alert());
                break;
            }
            if (Vector3.Distance(transform.position, player.position) < playerAttackDistance) {
                GetComponent<NavMeshAgent>().SetDestination(transform.position);
                GetComponent<NavMeshAgent>().velocity = Vector3.zero;
                GetComponent<Animator>().SetBool("attack", true);
                yield return new WaitForSeconds(3);
            } else {
                GetComponent<NavMeshAgent>().SetDestination(player.position);
                GetComponent<Animator>().SetBool("attack", false);
            }
            yield return new WaitForEndOfFrame();
        }
    }
}