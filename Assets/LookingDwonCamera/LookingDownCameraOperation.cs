using UnityEngine;

public class LookingDownCameraOperation : BaseInputOperation
{
    private Plane interactPlane;
    private Plane cameraMovablePlane;
    private bool canOperate;

    public LookingDownCameraOperation()
    {
        var cameraForward = CameraManager.Instance.Camera.transform.forward;
        interactPlane = new Plane(-cameraForward, 0f);
        cameraMovablePlane = new Plane(cameraForward, 0f);
    }

    public override void OnPointerDown(InputData inputData)
    {
        base.OnPointerDown(inputData);

        //Objectレイヤーだと操作しない
        int objectLayer = 1 << LayerMask.NameToLayer("Object");
        RaycastHit hit;
        canOperate = true;
        if (Utils.TryGetHitInfo(inputData.screenPosition, objectLayer, out hit)) {
            canOperate = false;
        }
    }

    public override void OnBeginDrag(InputData inputData)
    {
        if (!canOperate) {
            return;
        }

        base.OnBeginDrag(inputData);
        cameraMovablePlane.distance = CameraManager.Instance.Camera.transform.position.y;
        Vector3 camPos = Vector3.zero;
        if (TryGetCameraPosition(inputData.screenPosition, out camPos)) {
            inputData.previousCameraPosition = camPos;
        }
    }

    public override void OnDrag(InputData inputData)
    {
        if (!canOperate) {
            return;
        }

        base.OnDrag(inputData);
        Vector3 camPos = Vector3.zero;
        if (TryGetCameraPosition(inputData.screenPosition, out camPos)) {
            Vector3 diff = camPos - inputData.previousCameraPosition;
            Vector3 pos = CameraManager.Instance.Camera.transform.position - diff;
            CameraManager.Instance.ReservePosition(pos);
        }
    }

    private bool TryGetCameraPosition(Vector3 screenPos, out Vector3 camPos)
    {
        camPos = Vector3.zero;
        var ray = CameraManager.Instance.Camera.ScreenPointToRay(screenPos);
        float enter = 0f;
        var camTrans = CameraManager.Instance.Camera.transform;
        if (interactPlane.Raycast(ray, out enter)) {
            Vector3 pos = ray.origin + enter * ray.direction;
            var invRay = new Ray(pos, -camTrans.forward);
            // 画角による距離の差を考慮.
            if (cameraMovablePlane.Raycast(invRay, out enter)) {
                camPos = invRay.origin + enter * invRay.direction;
                return true;
            }
        }
        return false;
    }
}