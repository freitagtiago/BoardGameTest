using TMPro;
using UnityEngine;

namespace BoardGame.Game
{
    public class Tile : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TextMeshPro _coordinateText;
        [SerializeField] private MeshRenderer _cube;
        [SerializeField] private GameObject _pathIndicator;
        [SerializeField] private GameObject _endpathIndicator;

        [SerializeField]private Material _endpathIndicatorMat;
        [SerializeField]private Material _pathIndicatorMat;

        public Piece _occupiedBy { get; private set; }
        private int _treeBehaviourIndex = -1;
        private bool _isEndPath = false;
        private Vector2 _coordinate;

        public void Setup(Vector2 coordinate, Material material)
        {
            _coordinate = coordinate;
            _coordinateText.text = $"{_coordinate.x},{_coordinate.y}";

            transform.position = new Vector3(_coordinate.x, 0, _coordinate.y);

            _cube.material = material;
        }

        public void SetPiece(Piece piece)
        {
            if(_occupiedBy is EnemyPiece)
            {
                _occupiedBy.gameObject.SetActive(false);
            }
            _occupiedBy = piece;
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

        public bool IsEndPath()
        {
            return _isEndPath;
        }

        public int GetTreeBehaviourIndex()
        {
            return _treeBehaviourIndex;
        }

        public Vector2 GetCoordinates()
        {
            return _coordinate;
        }
    }
}
