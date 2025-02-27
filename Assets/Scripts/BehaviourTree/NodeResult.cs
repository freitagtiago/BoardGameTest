using UnityEngine;

namespace BoardGame.Config
{/*
  * Classe criada com o intuito de facilitar a validação de retorno.
  * Com a indicação do nó e não apenas o state, é possível 
  * pegar informações como a direção de movimento para que a peça utilize na cena.
  * No futuro outras informações úteis podem ser capturadas dessa mesma forma.
  */
    public class NodeResult 
    {
        public NodeBehaviour _state;
        public Node _node;
    
        public NodeResult(NodeBehaviour state, Node node)
        {
            this._state = state;
            this._node = node;
        }
    }
}