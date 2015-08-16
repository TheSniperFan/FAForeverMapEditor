﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Editing : MonoBehaviour {

	public		MapLuaParser		Scenario;
	public		CameraControler		KameraKontroler;
	public		GameObject[]		Categorys;
	public		GameObject[]		CategorysSelected;
	public		EditStates			State = EditStates.MapStat;
	public		bool				MauseOnGameplay;

	public		List<GameObject>		Selected = new List<GameObject>();
	public		List<GameObject>		MirrorSelected = new List<GameObject>();
	public		Vector3[]				SelectedStartPos;
	public		MarkersList			AllMarkersList;

	public		Transform			SelectedMarker;
	public		Transform			SelectedReflectionMarker;
	public		Vector3				SelectedMarkerBeginClickPos;

	public		Texture[]			SelectionSizeTextures;
	public		List<GameObject>	SelectionsRings = new List<GameObject>();
	public		GameObject			RingSelectionPrefab;
	public		InputField			ToolbarTolerance;
	public		float				MirrorTolerance;

	public		Toggle				ToogleMirrorX;
	public		Toggle				ToogleMirrorZ;
	public		Toggle				ToogleMirror90;
	public		Toggle				ToogleMirror270;
	public		Toggle				ToogleMirror180;

	public enum EditStates{
		MapStat, TerrainStat, TexturesStat, LightingStat, MarkersStat, DecalsStat, PropsStat, AIStat
	}

	void OnEnable(){
		ChangeCategory(0);
		State = EditStates.MapStat;
		MirrorTolerance = 0.5f;
		ToolbarTolerance.text = MirrorTolerance + "";
	}

	public void ButtonFunction(string func){
		switch(func){
		case "Save":
			Scenario.StartCoroutine("SaveMap");
			break;
		case "Map":
			State = EditStates.MapStat;
			ChangeCategory(0);
			break;
		case "Terrain":
			State = EditStates.TerrainStat;
			ChangeCategory(1);
			break;
		case "Textures":
			State = EditStates.TexturesStat;
			ChangeCategory(2);
			break;
		case "Lighting":
			State = EditStates.LightingStat;
			ChangeCategory(3);
			break;
		case "Markers":
			State = EditStates.MarkersStat;
			ChangeCategory(4);
			KameraKontroler.AllWorkingObjects = new List<GameObject>();
			foreach(MapLuaParser.Army obj in Scenario.ARMY_){
				KameraKontroler.AllWorkingObjects.Add(obj.Mark.gameObject);
			}
			foreach(MapLuaParser.Mex obj in Scenario.Mexes){
				KameraKontroler.AllWorkingObjects.Add(obj.Mark.gameObject);
			}
			foreach(MapLuaParser.Hydro obj in Scenario.Hydros){
				KameraKontroler.AllWorkingObjects.Add(obj.Mark.gameObject);
			}
			foreach(MapLuaParser.Marker obj in Scenario.SiMarkers){
				KameraKontroler.AllWorkingObjects.Add(obj.Mark.gameObject);
			}
			break;
		case "Decals":
			State = EditStates.DecalsStat;
			ChangeCategory(5);
			break;
		case "Props":
			State = EditStates.PropsStat;
			ChangeCategory(6);
			break;
		case "Ai":
			State = EditStates.AIStat;
			ChangeCategory(7);
			break;
		}
	}


	void ChangeCategory(int id = 0){
		foreach(GameObject obj in Categorys){
			obj.SetActive(false);
		}

		foreach(GameObject obj in CategorysSelected){
			obj.SetActive(false);
		}


		CategorysSelected[id].SetActive(true);
		Categorys[id].SetActive(true);
	}

	public void ChangePointerInGameplay(bool on = true){
		MauseOnGameplay = on;
	}

	public void AddToSelection(List<GameObject> add){
		for(int i = 0; i < add.Count; i++){
			bool AlreadyExist = false;
			foreach(GameObject obj in Selected){
				if(obj == add[i]){
					AlreadyExist = true;
					break;
				}
			}
			if(!AlreadyExist) Selected.Add(add[i]);

		}
		UpdateSelectionRing();
	}

	public void RemoveFromSelection(List<GameObject> remove){
		for(int i = 0; i < remove.Count; i++){
			foreach(GameObject obj in Selected){
				if(obj == remove[i]){
					Selected.Remove(obj);
					break;
				}
			}
		}
		UpdateSelectionRing();
	}

	public void ChangeSelectionState(List<GameObject> change){
		for(int i = 0; i < change.Count; i++){
			bool AlreadyExist = false;
			foreach(GameObject obj in Selected){
				if(obj == change[i]){
					AlreadyExist = true;
					Selected.Remove(obj);
					break;
				}
			}
			if(!AlreadyExist) Selected.Add(change[i]);
		}
		UpdateSelectionRing();
	}

	public void CleanSelection(){
		Selected = new List<GameObject>();
		UpdateSelectionRing();

	}

	public void UpdateReflectionOption(int id){
		UpdateSelectionRing();
	}
	public void UpdateTolerance(){
		//MirrorTolerance = float.Parse( ToleranceField.text, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
		MirrorTolerance = System.Single.Parse(ToolbarTolerance.text);
		UpdateSelectionRing();


	}

	public void UpdateSelectionRing(){
		if(Selected.Count <= 0){
			SelectedMarker.gameObject.SetActive(false);
			SelectedReflectionMarker.gameObject.SetActive(false);
			foreach(GameObject child in SelectionsRings){
				Destroy(child.gameObject);
			}
			SelectionsRings = new List<GameObject>();
			SelectedStartPos = new Vector3[0];
			foreach(ListObject list in AllMarkersList.AllFields){
				list.Unselect();
			}
		}
		else{
			SelectedMarker.gameObject.SetActive(true);
			MirrorSelected = new List<GameObject>();

			foreach(GameObject child in SelectionsRings){
				Destroy(child.gameObject);
			}
			SelectionsRings = new List<GameObject>();

			float MaxX = Selected[0].transform.position.x;
			float MaxY = Selected[0].transform.position.z;

			float MinX = Selected[0].transform.position.x;
			float MinY = Selected[0].transform.position.z;
			float MidHeight = 0;

			foreach(GameObject obj in Selected){
				MaxX = Mathf.Max(MaxX, obj.transform.position.x);
				MaxY = Mathf.Max(MaxY, obj.transform.position.z);
				MinX = Mathf.Min(MinX, obj.transform.position.x);
				MinY = Mathf.Min(MinY, obj.transform.position.z);
				MidHeight += obj.transform.position.y;
			}
			MidHeight /= Selected.Count;

			SelectedMarker.position = new Vector3((MaxX+MinX) /2, MidHeight, (MaxY + MinY) /2);
			SelectedMarker.localScale = Vector3.one * (Mathf.Max(Mathf.Abs(MaxX - MinX), Mathf.Abs(MaxY - MinY)) * 1.1f  +  0.4f);

			if(SelectedMarker.localScale.x < 3){
				SelectedMarker.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", SelectionSizeTextures[0]);
				SelectedReflectionMarker.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", SelectionSizeTextures[0]);
			}
			else{
				SelectedMarker.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", SelectionSizeTextures[1]);
				SelectedReflectionMarker.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", SelectionSizeTextures[1]);
			}

			SelectedStartPos = new Vector3[Selected.Count];

			for(int i = 0; i < Selected.Count; i++){
				SelectedStartPos[i] = Selected[i].transform.position - SelectedMarker.position;
			}

			if(ToogleMirrorX.isOn && ToogleMirrorZ.isOn){
				SelectedReflectionMarker.gameObject.SetActive(true);
				SelectedReflectionMarker.localScale = SelectedMarker.localScale;
				float Xdist = Scenario.MapCenterPoint.x - SelectedMarker.position.x;
				float Zdist = Scenario.MapCenterPoint.z - SelectedMarker.position.z;

				SelectedReflectionMarker.position = new Vector3(SelectedMarker.position.x + Xdist * 2 + 0.2f, SelectedMarker.position.y, SelectedMarker.position.z + Zdist * 2 - 0.2f);

				foreach(GameObject obj in Selected){
					foreach(GameObject all in KameraKontroler.AllWorkingObjects){
						Vector3 SelPos = new Vector3(obj.transform.position.x + Xdist * 2 + 0.2f, 0, obj.transform.position.z + Zdist * 2 - 0.2f);
						Vector3 AllPos = new Vector3(all.transform.position.x, 0, all.transform.position.z);

						if(Vector3.Distance(SelPos, AllPos) < MirrorTolerance){
							bool AlreadyExist = false;
							foreach(GameObject MirObj in MirrorSelected){
								if(MirObj == all){
									AlreadyExist = true;
									break;
								}
							}
							if(!AlreadyExist) MirrorSelected.Add(all);
							continue;
						}
					}
				}

			}
			else if(ToogleMirrorX.isOn){
				SelectedReflectionMarker.gameObject.SetActive(true);
				SelectedReflectionMarker.localScale = SelectedMarker.localScale;
				float Xdist = Scenario.MapCenterPoint.x - SelectedMarker.position.x;

				SelectedReflectionMarker.position = new Vector3(SelectedMarker.position.x + Xdist * 2 + 0.2f, SelectedMarker.position.y, SelectedMarker.position.z);

				foreach(GameObject obj in Selected){
					foreach(GameObject all in KameraKontroler.AllWorkingObjects){
						Xdist = Scenario.MapCenterPoint.x - obj.transform.position.x;
						Vector3 SelPos = new Vector3(obj.transform.position.x + Xdist * 2 + 0.2f, 0, obj.transform.position.z);
						Vector3 AllPos = new Vector3(all.transform.position.x, 0, all.transform.position.z);
						
						if(Vector3.Distance(SelPos, AllPos) < MirrorTolerance){
							bool AlreadyExist = false;
							foreach(GameObject MirObj in MirrorSelected){
								if(MirObj == all){
									AlreadyExist = true;
									break;
								}
							}
							if(!AlreadyExist) MirrorSelected.Add(all);
							continue;
						}
					}
				}
			}
			else if(ToogleMirrorZ.isOn){
				SelectedReflectionMarker.gameObject.SetActive(true);
				SelectedReflectionMarker.localScale = SelectedMarker.localScale;
				float Zdist = Scenario.MapCenterPoint.z - SelectedMarker.position.z;
				SelectedReflectionMarker.position = new Vector3(SelectedMarker.position.x, SelectedMarker.position.y, SelectedMarker.position.z + Zdist * 2 - 0.2f);

				foreach(GameObject obj in Selected){
					foreach(GameObject all in KameraKontroler.AllWorkingObjects){
						Zdist = Scenario.MapCenterPoint.z - obj.transform.position.z;
						Vector3 SelPos = new Vector3(obj.transform.position.x, 0, obj.transform.position.z + Zdist * 2 - 0.2f);
						Vector3 AllPos = new Vector3(all.transform.position.x, 0, all.transform.position.z);
						
						if(Vector3.Distance(SelPos, AllPos) < MirrorTolerance ){
							bool AlreadyExist = false;
							foreach(GameObject MirObj in MirrorSelected){
								if(MirObj == all){
									AlreadyExist = true;
									break;
								}
							}
							if(!AlreadyExist) MirrorSelected.Add(all);
							continue;
						}
					}
				}
			}
			else{
				SelectedReflectionMarker.gameObject.SetActive(false);
			}
			foreach(ListObject list in AllMarkersList.AllFields){
				list.Unselect();
			}
			foreach(GameObject obj in Selected){
				GameObject newRing = Instantiate(RingSelectionPrefab) as GameObject;
				newRing.GetComponent<SelectionRing>().SelectedObject = obj.transform;
				SelectionsRings.Add(newRing);
				foreach(ListObject list in AllMarkersList.AllFields){
					if(list.ConnectedGameObject == obj){
						list.Select();
						break;
					}
				}
			}
			foreach(GameObject obj in MirrorSelected){
				GameObject newRing = Instantiate(RingSelectionPrefab) as GameObject;
				newRing.GetComponent<SelectionRing>().SelectedObject = obj.transform;
				SelectionsRings.Add(newRing);
			}
		}
	}
}