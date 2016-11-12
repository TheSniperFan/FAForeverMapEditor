﻿using UnityEngine;
using System.Collections;
using UndoHistory;
using EditMap;

public class HistoryTerrainHeight : HistoryObject {

	public		float[,]		Pixels;

	public override void Register(){
		Undo.RegisterMarkersDelete = true;

		int x = Undo.UndoData_newheights.GetLength (0);
		int y = Undo.UndoData_newheights.GetLength (1);

		Pixels = new float[x, y];
		for (int i = 0; i < x; i++) {
			for (int j = 0; j < y; j++) {
				Pixels [i, j] = Undo.UndoData_newheights [i, j];
			}
		}
	}


	public override void DoUndo(){
		Undo.UndoData_newheights = Undo.Current.Scmap.Teren.terrainData.GetHeights(0, 0, Undo.Current.Scmap.Teren.terrainData.heightmapWidth, Undo.Current.Scmap.Teren.terrainData.heightmapHeight);
		HistoryTerrainHeight.GenerateRedo (Undo.Current.Prefabs.TerrainHeightChange).Register();
		DoRedo ();
	}

	public override void DoRedo(){
		if(Undo.Current.EditMenu.State != Editing.EditStates.TerrainStat){
			Undo.Current.EditMenu.State = Editing.EditStates.TerrainStat;
			Undo.Current.EditMenu.ChangeCategory(1);
		}
		Debug.Log("Undo " + Pixels.Length);
		Undo.Current.Scmap.Teren.terrainData.SetHeights(0, 0, Pixels);
		Undo.Current.EditMenu.EditTerrain.Markers.UpdateMarkersHeights();
	}
}