using UnityEngine;
using System.Collections;
namespace Artwave.Board{
	/**
	 * Interfejs dla Board, pozwalający komunikować się z Hexami - tablica musi mieć ten interfejs by mogła się komunikować;
	 */ 
	public interface HexListenerInterface{
		void OnHexDown(HexField hex);
		void OnHexOver(HexField hex);
		void OnHexUp(HexField hex);
//		void OnHexExit(AbstractHex hex);
	}
}
