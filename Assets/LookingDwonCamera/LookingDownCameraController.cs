using UnityEngine;

public class LookingDownCameraController : MonoBehaviour
{
    [SerializeField]
    private ControllerPanel controllerPanel;

    private IInputOperation inputOperation;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        inputOperation = new LookingDownCameraOperation();
        controllerPanel.Init(inputOperation);
    }
}
