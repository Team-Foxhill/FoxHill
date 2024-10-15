using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FoxHill.Scene.Production
{
    public class GameSceneProduction : MonoBehaviour
    {
        [SerializeField] private List<Vector3> _wayPoints = new List<Vector3>(); // 2D 환경이므로 z좌표는 zoom size로 사용
        [SerializeField] private CinemachineVirtualCamera _productionCamera;
        [SerializeField] private CinemachineVirtualCamera _playerCamera;
        [SerializeField] private GameObject _player;

        public IEnumerator C_StartSceneProduction()
        {
            _productionCamera.Priority = 20;
            _playerCamera.Priority = 10;

            yield return StartCoroutine(C_FollowTrack(0, 1, 4f));
            yield return StartCoroutine(C_FollowTrack(2, 3, 4f));
            yield return StartCoroutine(C_FollowTrack(4, 5, 4f));
            yield return StartCoroutine(C_FollowTrack(6, 7, 4f));
            yield return StartCoroutine(C_FollowTrack(7, 8, 5f));

            // yield return StartCoroutine(C_FollowTrack(2, 3, 4f));

            _productionCamera.Priority = 10;
            _playerCamera.Priority = 20;
        }

        private void TransitionCamera(Vector3 targetPosition)
        {
            _productionCamera.transform.position = TrimVector(targetPosition, _productionCamera.transform.position.z);
        }

        private IEnumerator C_FollowTrack(int startIndex, int endIndex, float time)
        {
            float elapsedTime;

            TransitionCamera(_wayPoints[startIndex]);
            StartCoroutine(C_Zoom(_wayPoints[startIndex].z, 0f, true));

            for (int i = startIndex + 1; i <= endIndex; i++)
            {
                elapsedTime = 0f;

                var startPosition = _productionCamera.transform.position;
                var targetPosition = TrimVector(_wayPoints[i], _productionCamera.transform.position.z);

                StartCoroutine(C_Zoom(_wayPoints[i].z, time));

                while (elapsedTime < time)
                {
                    Debug.Log(elapsedTime);
                    elapsedTime += Time.deltaTime;
                    _productionCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / time);

                    yield return null;
                }

                TransitionCamera(targetPosition);
            }
        }

        private Vector3 TrimVector(Vector3 position, float z)
        {
            return new Vector3(position.x, position.y, z);
        }

        private IEnumerator C_Zoom(float target, float time, bool isInstant = false)
        {
            if (isInstant == true)
            {
                _productionCamera.m_Lens.OrthographicSize = target;
                yield break;
            }

            float startSize = _productionCamera.m_Lens.OrthographicSize;
            float elapsedTime = 0f;

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                _productionCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, target, elapsedTime / time);

                yield return null;
            }

            _productionCamera.m_Lens.OrthographicSize = target;
        }
    }
}