using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BoardGame.Game
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _coordinateText;
        [SerializeField] private MeshRenderer _cube;

        private Vector2 _coordinate;
        private GameObject _occupiedBy;

        public void Setup(Vector2 coordinate, Material material)
        {
            _coordinate = coordinate;
            _coordinateText.text = $"{_coordinate.x},{_coordinate.y}";

            transform.position = new Vector3(_coordinate.x, 0, _coordinate.y);

            _cube.material = material;
        }

        public void SetPiece(GameObject piece)
        {
            _occupiedBy = piece;
        }

        public bool IsOccupied()
        {
            return _occupiedBy != null;
        }

        void OnMouseDown()
        {
            Debug.Log($"OVER {_coordinateText.text}/ OCCUPIED? {_occupiedBy != null}");
        }
    }
}
