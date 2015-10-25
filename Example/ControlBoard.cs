using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Artwave.Board;

public class ControlBoard : MonoBehaviour, BoardListenerInterface, BuilderListenerInterface  {
	public BoardManager boardManager;
	void Start () {
		boardManager.Init ();
//		boardManager.GetPatternHex ().ShowColor (Color.red);
		boardManager.AddBoardListener(this);
		boardManager.AddBuildListener(this);
		boardManager.SetTableSizeFullScreen ();
		boardManager.Build ();
//		boardManager.Reset ();
//		boardManager.Build (4,4);
	}
	public void OnHexBoardDown(HexField hex){
		
	}
	public void OnHexBoardUp(HexField hex){
		Debug.Log ("Hex Up coord:" + hex.GetCoordinates() + " position:" + hex.transform.position);
		hex.ShowColor (Color.green);
		List<HexField> list = boardManager.GetNeigbors (hex.GetCoordinates(),2);
		foreach (HexField hex2 in list) {
			hex2.ShowColor (Color.yellow);
		}
		boardManager.AnimationGoToHex (hex, 5, false);
	}
	public void OnBoardDrag(){

	}
	
	public void OnBuildStart(HexField patternHex, int colNumber, int rowNumber, Vector2 hexSize, bool isEven, bool symmetricHorizontal){
		
	}
	public void OnBuildEnd(){
		
	}
	public void OnResetStart(){
		
	}
	public void OnResetEnd(){
		
	}
	public bool OnCreateHexStart(Vector3 coordinatesCube, Vector2 coordinatesOffset, bool isInverse){

		return true;
	}
	public void OnCreateHexEnd(HexField hex){
		
	}
	public void OnGoToHexAnimationFinish(){
		
	}
}
