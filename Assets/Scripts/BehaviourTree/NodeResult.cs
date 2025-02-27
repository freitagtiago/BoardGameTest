using UnityEngine;

namespace BoardGame.Config
{/*
  * Classe criada com o intuito de facilitar a valida��o de retorno.
  * Com a indica��o do n� e n�o apenas o state, � poss�vel 
  * pegar informa��es como a dire��o de movimento para que a pe�a utilize na cena.
  * No futuro outras informa��es �teis podem ser capturadas dessa mesma forma.
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