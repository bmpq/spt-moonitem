using Comfort.Common;
using EFT;
using UnityEngine;

namespace tarkin.moonitem
{
    internal class TheF : MonoBehaviour
    {
        float timeHiding;
        bool canAppear;

        void Start()
        {
            if (Singleton<GameWorld>.Instance == null)
            {
                Destroy(gameObject);
                return;
            }

            timeHiding = 10f * Plugin.DelayMultiplier;
            Physics.simulationMode = SimulationMode.FixedUpdate;
            transform.eulerAngles = new Vector3(0, CameraClass.Instance.Camera.transform.eulerAngles.y - 180f, 0);
        }

        void Update()
        {
            timeHiding -= Time.deltaTime / Plugin.DelayMultiplier;

            transform.position = CameraClass.Instance.Camera.transform.position;

#if DEBUG
            transform.position += new Vector3(0, 3, 0);
#endif

            if (!canAppear || timeHiding > 0f)
            {
                transform.eulerAngles = new Vector3(0, CameraClass.Instance.Camera.transform.eulerAngles.y - 180f, 0);
            }
        }

        void FixedUpdate()
        {
            if (timeHiding > 0)
                return;

            if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, CameraClass.Instance.Camera.transform.eulerAngles.y)) < 90f)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, CameraClass.Instance.Camera.transform.eulerAngles.y)) < 30f)
                {
                    Singleton<GameWorld>.Instance.MainPlayer.KillMe(EBodyPartColliderType.Eyes, 7347);
                }
                canAppear = true;
                return;
            }


            Transform camTransform = CameraClass.Instance.Camera.transform;

            canAppear = !(Physics.Raycast(camTransform.position - camTransform.forward, -camTransform.forward, 5f, LayerMaskClass.LowPolyColliderLayerMask));
        }
    }
}
