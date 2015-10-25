using UnityEngine;
using System.Collections;


namespace Artwave.Board{
	/**
 * Interfejs dla Hex buildera, pozwalający informować listenery o zmianach w trakcie budowy planszy
 */ 
	public interface BuilderListenerInterface{
		/**
		 * wywoływane w momencie rozpoczęcia budowy planszy
		 */ 
		void OnBuildStart(HexField patternHex, int colNumber, int rowNumber, Vector2 hexSize, bool isEven, bool symmetricHorizontal);
		/**
		 * wywoływany w momencie zbudowania planszy
		 */ 
		void OnBuildEnd();
		/**
		 * wywoływany w momencie resetu planszy początek
		 */ 
		void OnResetStart();
		/**
		 * wywoływany w momencie resetu planszy koniec
		 */ 
		void OnResetEnd();
		/**
		 * wywoływany w momencie rozpoczęcia budowy hexa w dany miejscu, jeśli zwrócony false to budowa danego hexa zostanie przerwana
		 * isInverse okdreśla czy dany wiersz jest przesunięty, dodatkowo taki wiersz w trybie symmetricHorizon nie ma ostatniego hexa
		 */ 
		bool OnCreateHexStart(Vector3 coordinatesCube, Vector2 coordinatesOffset, bool isInverse);
		/**
		 * wywoływane gdy hex zostanie zbudowany
		 */ 
		void OnCreateHexEnd(HexField hex);

	}
}