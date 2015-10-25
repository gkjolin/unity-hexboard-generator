using UnityEngine;
using System.Collections;


namespace Artwave.Board{
	/**
 * Interfejs dla Controller, pozwalający komunikować się z z zewnętrznymi skryptami
 */ 
	public interface BoardListenerInterface{
		/**
		 * user wdusza przycisk na hexie, w tym miejscu nie powiniśmy przeprowadzać żadnych akcji, co najwyżej jakiś fade on czy coś podobnego, ponieważ przycisk może przerodzić się w drag
		 */ 
		void OnHexBoardDown(HexField hex);
		/**
		 * gdy przesuwamy po mapie. Nie musi być poprzedzone OnHexBoardDown. Ponieważ user mógł wdusić nie na hexie ale w pobliżu lub na połączeniu. Odpala się co klatkę gdy jest drag, oznacza to że user przesuwa po mapie.
		 * Nie ma informacji o hexie na jakim się zatrzymuje, bo wcale nie musi się na hexie zatrzymać
		 */ 
		void OnBoardDrag();
		/**
		 * wywołane gdy był OnHexBoardDown, a potem się nie pojawił drag
		 */ 
		void OnHexBoardUp(HexField hex);
		/**
		 * jeśli mamy animowane przejście planszy w jakiś punkt to ten event się odpali po dojściu do niego
		 */ 
		void OnGoToHexAnimationFinish();
	}
}