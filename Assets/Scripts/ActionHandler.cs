using BoardGame.Game;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionHandler : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputActionAsset;
    [SerializeField] private LayerMask _tileLayer;
    [SerializeField] private LayerMask _playerPiecesLayer;
    public bool _selectionEnabled = true;
    private InputAction _interactionAction;
    private PlayerPiece _selectedPiece;

    private void Awake()
    {
        InputActionMap playerActionMap = _inputActionAsset.FindActionMap("InGame");

        _interactionAction = playerActionMap.FindAction("Interact");
        _interactionAction.performed += HandleInput;
        _interactionAction.Enable();
    }

    private void Start()
    {
        BoardHandler.Instance.OnEndDrawing += SetSelection;
        BoardHandler.Instance.OnEndBehaviour += EndBehaviour;
    }

    private void OnDisable()
    {
        _interactionAction.performed += HandleInput;
        BoardHandler.Instance.OnEndDrawing -= SetSelection;
        BoardHandler.Instance.OnEndBehaviour -= EndBehaviour;
    }

    private void HandleInput(InputAction.CallbackContext context)
    {
        if (_selectionEnabled)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _playerPiecesLayer))
            {
                PlayerPiece playerPiece = hit.collider.gameObject.GetComponentInParent<PlayerPiece>();
                playerPiece.DrawMovementChoices();
                _selectedPiece = playerPiece;
            }
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _tileLayer))
            {
                Tile tile = hit.collider.gameObject.GetComponentInParent<Tile>();
                if (tile.IsEndPath())
                {
                    _selectedPiece.ApplyMovement(tile.GetTreeBehaviourIndex());
                }
            }
        }
    }

    private void SetSelection(bool selectionEnabled)
    {
        _selectionEnabled = selectionEnabled;
        if (_selectionEnabled)
        {
            _selectedPiece = null;
            Debug.Log("Peças foram distribuídas, selecione sua peça");
        }
    }

    private void EndBehaviour(Tile tile)
    {
        _selectionEnabled = true;
        _selectedPiece._currentTile = tile;
        _selectedPiece = null;
    }
}
