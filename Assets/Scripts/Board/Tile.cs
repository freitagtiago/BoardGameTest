using TMPro;
using UnityEngine;

namespace BoardGame.Game
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _coordinateText;
        [SerializeField] private MeshRenderer _cube;
        [SerializeField] private GameObject _pathIndicator;
        [SerializeField] private GameObject _endpathIndicator;

        private Vector2 _coordinate;
        public Piece _occupiedBy;
        [SerializeField]private Material _endpathIndicatorMat;
        [SerializeField]private Material _pathIndicatorMat;

        private int _treeBehaviourIndex = -1;
        private bool _isEndPath = false;

        public void Setup(Vector2 coordinate, Material material)
        {
            _coordinate = coordinate;
            _coordinateText.text = $"{_coordinate.x},{_coordinate.y}";

            transform.position = new Vector3(_coordinate.x, 0, _coordinate.y);

            _cube.material = material;
        }

        public void SetPiece(Piece piece)
        {
            if(GameManager.Instance._isSelectionEnabled)
            {
                _occupiedBy = piece;
            }
            else
            {
                if(_occupiedBy is EnemyPiece)
                {
                    _occupiedBy.gameObject.SetActive(false);
                }
                _occupiedBy = piece;
            }
        }

        public bool IsOccupied()
        {
            return _occupiedBy != null;
        }

        void OnMouseDown()
        {
            if (GameManager.Instance._isSelectionEnabled)
            {
                if (_occupiedBy is PlayerPiece)
                {
                    (_occupiedBy as PlayerPiece).DrawMovementChoices(this);
                    GameManager.Instance._selectedPiece = (_occupiedBy as PlayerPiece);
                }
            }
            else
            {
                if (_isEndPath)
                {
                    GameManager.Instance._selectedPiece.ApplyMovement(this, _treeBehaviourIndex);
                }
            }
        }

        /*
         * Por simplicidade utilizei troca de materiais vinculados, mas nessa etapa 
         * do jogo poderia ser implementada a instanciação dos marcadores e adicionálos em uma pool.
         * Assim no momento do uso apenas trocaria a posição, e reduziria o número de objetos em cena que hoje,
         * tem um marcador para cada Tile.
         */
        public void HandlePathIndicator(bool enablePathIndicator, bool isEndPath, int treeBehaviourIndex)
        {
            _pathIndicator.SetActive(enablePathIndicator);
            _isEndPath = isEndPath;
            _treeBehaviourIndex = enablePathIndicator ? treeBehaviourIndex : -1;

            if (isEndPath)
            {
                _pathIndicator.GetComponent<MeshRenderer>().material = _endpathIndicatorMat;
            }
            else
            {
                _pathIndicator.GetComponent<MeshRenderer>().material = _pathIndicatorMat;
            }
        }

        public Vector2 GetCoordinates()
        {
            return _coordinate;
        }
    }
}
