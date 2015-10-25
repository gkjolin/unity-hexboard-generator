using UnityEngine;
using UnityEditor;
namespace Artwave.Board{
	public class PrepareBoard: ScriptableWizard {
		public BoardManager tableBoardPrefab;
		[MenuItem("Tools/Artwave Board/Prepate Board")]
		public static void Prepare(){
			ScriptableWizard.DisplayWizard ("Artwave Board", typeof(PrepareBoard), "Generate");

		}
		void OnWizardCreate(){
			GameObject objectPrefab = PrefabUtility.InstantiatePrefab (tableBoardPrefab.gameObject) as GameObject;
			Debug.Log (objectPrefab);
			objectPrefab.name = "TableBoard";
		}
		void OnWizardUpdate(){
			BoardManager oldBoardManager = (BoardManager)FindObjectOfType(typeof(BoardManager));
			if (oldBoardManager) {
				errorString = "In Scene there can by only one Board Manager Object";
				isValid = false;
				return;
			}
			if (tableBoardPrefab == null) {
				errorString = "Add Prefab With BoardManager";
				isValid = false;
				return;
			} 
			errorString = "";
			isValid = true;
		}
	}
}
