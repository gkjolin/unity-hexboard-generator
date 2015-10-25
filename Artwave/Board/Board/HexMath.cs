using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/**
 * klasa przechowuje metody służące do różnych obliczeń
 * wzory zaczerpnięte z strony http://www.redblobgames.com/grids/hexagons
 * ogólnie założenie jest że hexy są w wersji "pointy topped" a nie "flat tapped"
 */ 
public static class HexMath {
	/**
	 * gdy budujemy mapę tak że hexy w pozycji horyzontalnej mają równoległe do siebie boki i nadajemy współrzędne x,y hexom to taki układ współrzędnych nazywa się 
	 * odd-r offset lub even-r offset (even- parzysty, odd-nieparzysty)
	 * r w nazwie oznacza Row czyli wiersz, a więc że hexy ułożone są obok siebie w wierszu, jak by były ich ściany równoległe w pozycji vertykalnej to byśmy mieli ułożenie 
	 * columnowe w skrócie przyjmowane jako q a więc odd-q offset lub even-q offset
	 * w r typie odd oznacza że odstają hexy z y parzystym
	 * czyli :
	 * (0,0)(1,0)(2,0)        (0,0)(1,0)(2,0)   
	 *   (0,1)(1,1)(2,1)    (0,1)(1,1)(2,1)
	 * (0,2)(1,2)(2,2)        (0,2)(1,2)(2,2)
	 *   (0,3)(1,3)(2,3)    (0,3)(1,3)(2,3)
	 * jest odd              jest even
	 * dużo lepszym układem współrzędnych jest układ Cube cordinate, większość przeliczeń będzie zakładać ten układ
	 * w przeliczeniu col jest to która kolumna a wiec x
	 * row który wiersz a więc y
	 * 
	 * 
	 */ 
	public static Vector3 ConvertEvenROffsetToCubeCoordinate(int col, int row){
		float x = col - (row + (row & 1)) / 2;
		float z = row;
		float y = -x - z;
		return new Vector3(x, y, z);
	}
	/**
	 * to samo co ConvertEvenROffsetToCubeCoordinate ale oblicza dla Odd board
	 */ 
	public static Vector3 ConvertOddROffsetToCubeCoordinate(int col, int row){
		float x = col - (row - (row & 1)) / 2;
		float z = row;
		float y = -x - z;
		return new Vector3(x, y, z);
	}
	public static Vector2 ConvertCubeToEvenROffsetCoordinate(int x, int y, int z){
		float col = x + (z + (z & 1)) / 2;
		float row = z;
		return new Vector3(col, row);
	}
	public static Vector2 ConvertCubeToOddROffsetCoordinate(int x, int y, int z){
		float col = x + (z - (z & 1)) / 2;
		float row = z;
		return new Vector3(col, row);
	}
	/**
	 * w Cube coordinate system, 
	 * trzecia oś jest w zasadzie zbędna, ponieważ x + y + z = 0 więc z = -x - y; Dlatego mając dwie dowolne osie szybko możemy obliczyć trzecią
	 * bez problemu możemy przechowywać tylko dwie osie a trzecią obliczać w locie. System w którym używamy dwóch osi zamiast trzech osi z Cube coordinate, nazywa się Axial coordinates jest to więc alternatywne zaprezentowanie Cube
	 * zwykle rezygnujemy z y w axial
	 */ 
	public static int CalculateThirdCoordinate(int firstCoordinate, int secondCoordinate){
		return - firstCoordinate - secondCoordinate;
	}
	/**
	 *  wzraca listę współrzędnych sąsiadujących hexów (nie zwraca wskazanego punktu), gdy offset = 1 to pomija pierwszy krąg i range liczy od następnego
	 */ 
	public static List<Vector3> GetRange(Vector3 center, int range, int offset = 0){
		List<Vector3> list = new List<Vector3> ();
		int xMin = (int)center.x - range - offset;
		int xMax = (int)center.x + range + offset;
		int yMin = (int)center.y - range - offset;
		int yMax = (int)center.y + range + offset;
		for (int x = xMin; x <= xMax; x++) {
			for (int y = yMin; y <= yMax; y++) {
				int z = CalculateThirdCoordinate(x, y);
				if(z < center.z - range - offset || z > center.z + range + offset){
					continue;
				}
				if(x > center.x - offset -1 && x < center.x + offset + 1 && y > center.y - offset -1 && y < center.y + offset + 1 && z > center.z - offset -1 && z < center.z + offset + 1){
					continue;
				} 
				list.Add(new Vector3(x, y, z));
			}
		}
		return list;
	}
	/**
	 * wysokość i szerokość hexa są od siebie zależne. znając szerokość obliczymy wysokość
	 */ 
	public static float CalculateHexHeight(float width){
		return 2 * width / Mathf.Sqrt (3);
	}
	/**
	 * wysokość i szerokość hexa są od siebie zależne. znając wysokość obliczymy szerokość
	 */ 
	public static float CalculateHexWidth(float height){
		return height * Mathf.Sqrt (3) / 2;
	}
	/**
	 * oblicza wyskokość kolumny, na podstawie wysokości hexa i ilości hexów w kolumnie
	 */ 
	public static float CalculateColumnHeight(float hexHeight, int columnHexNumber){
		return (3* columnHexNumber +1) * hexHeight / 4;
	}
	/**
	 * oblicza szerokość kolumny hexów, na podstawie ilości hexów w tej kolumnie i szerokości hexa
	 * multi ponieważ tam gdzie są conajmniej 2 wiersze jest dodatkowe przesunięcie co drugiego wiersza o pół szerokości.
	 */ 
	public static float CalculateMultiRowWidth(float hexWidth, int rowHexNumber){
		return  CalculateRowWidth(hexWidth, rowHexNumber) + hexWidth / 2;
	}
	/**
	 * oblicza szerokość kolumny hexów, na podstawie ilości hexów w tej kolumnie i szerokości hexa, ma zastosowanie gdy Chcemy obliczyć
	 * szerokość tylko jednego wiersza, gdy są conajmniej 2 ich wspólną szerokość trzeba obliczyć z CalculateMultiRowWidth
	 */ 
	public static float CalculateRowWidth(float hexWidth, int rowHexNumber){
		return  rowHexNumber * hexWidth;
	}
	/**
	 * na podstawie wysokości kolumny i ilości hexów w pionie oblicza wysokość hexa 
	 */ 
	public static float CalculateHexHeightInColumn(float columnHeight, int columnHexNumber){
		return (4 * columnHeight) / (3 * columnHexNumber + 1);
	}
	/**
	 * na podstawie szerokości wiersza i ilości hexów w ppoziomie oblicza wysokość hexa 
	 */ 
	public static  float CalculateHexWidthInRow(float rowWidth, int rowHexNumber){
		return rowWidth / rowHexNumber;
	}
}
