using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Artwave.Board{
	/**
	 * jest to obiekt listenera przechowujący grupę listenerów, używamy jak zwykłego ale pod spodem wywołuje wszystkie na liście.
	 * W controlerze może być wiele listenerów, by dla eventa za każdym razem nie iterować po wszystkich to użyjemy tej klasy
	 * ma ona ten sam interfejs a iteracja idzie w jej wnętrzu
	 */ 
	public class BuilderListenerList : BuilderListenerInterface {
		private List<BuilderListenerInterface> listenerList;
		public BuilderListenerList(){
			listenerList = new List<BuilderListenerInterface> ();
		}
		public void Add(BuilderListenerInterface listener){
			foreach(BuilderListenerInterface currListener in listenerList){
				if(currListener == listener){
					return;
				}
			}
			listenerList.Add (listener);
		}
		public void OnBuildStart(HexField patternHex, int colNumber, int rowNumber, Vector2 hexSize, bool isEven, bool symmetricHorizontal){
			foreach (BuilderListenerInterface listener in listenerList) {
				listener.OnBuildStart(patternHex, colNumber, rowNumber, hexSize, isEven, symmetricHorizontal);
			}
		}
		public void OnBuildEnd(){
			foreach (BuilderListenerInterface listener in listenerList) {
				listener.OnBuildEnd();
			}
		}
		public void OnResetStart(){
			foreach (BuilderListenerInterface listener in listenerList) {
				listener.OnResetStart();
			}
		}
		public void OnResetEnd(){
			foreach (BuilderListenerInterface listener in listenerList) {
				listener.OnResetEnd();
			}
		}
		/**
		 * pierwszy false blokuje dalsze listenery i przerywa budowę danego hexa 
		 * isInverse oznacza czy jest to wiersz przesunięty
		 */
		public bool OnCreateHexStart(Vector3 coordinatesCube, Vector2 coordinatesOffset, bool isInverse){
			foreach (BuilderListenerInterface listener in listenerList) {
				bool result = listener.OnCreateHexStart(coordinatesCube, coordinatesOffset, isInverse);
				if(result == false){
					return false;
				}
			}
			return true;
		}
		/**
		 * jeśli jest jakikolwiek false to przerwie budowę hexa ale odpyta wszystkie listenery
		 */
//		public bool OnCreateHexStart(Vector3 coordinates){
//			bool allResult = true;
//			foreach (BuilderListenerInterface listener in listenerList) {
//				bool result = listener.OnCreateHexStart(coordinates);
//				if(result == false){
//					Debug.Log ("MAMY FALSE");
//					allResult = false;
//				}
//			}
//			return allResult;
//		}
		public void OnCreateHexEnd(HexField hex){
			foreach (BuilderListenerInterface listener in listenerList) {
				listener.OnCreateHexEnd(hex);
			}
		}
	}
}
