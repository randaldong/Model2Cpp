using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class GenerateCode : MonoBehaviour
{
	public Camera camera;
	public List<GameObject> pointLights = new List<GameObject>();

	List<Vector3> sphereCenters = new List<Vector3>();
	List<float> radiusValues = new List<float>();
	List<Vector3> materialColors = new List<Vector3>();
	string folder = "Assets/Resources/";
	private void Start()
	{
		Directory.CreateDirectory(folder);
		string path = folder + "SceneCodeCpp.txt";
		WriteChildrenInfoByName(gameObject, "Sphere");
		// Camera
		if (!File.Exists(path))
		{
			string cameraString = String.Format("Scene myScene{{\n\tCamera{{ \n\t\tVector3{{{0}, {1}, {2}}}, // lookfrom\n\t\tVector3{{{3}, {4}, {5}}}, // lookat\n\t\tVector3{{{6}, {7}, {8}}}, // up\n\t\t{9} // vfov\n\t}},\n",
				camera.transform.position.x.ToString("F"), camera.transform.position.y.ToString("F"), camera.transform.position.z.ToString("F"),
				camera.transform.position.x + camera.transform.forward.x, camera.transform.position.y + camera.transform.forward.y, camera.transform.position.z + camera.transform.forward.z,
				camera.transform.up.x, camera.transform.up.y, camera.transform.up.z,
				camera.fieldOfView);
			File.WriteAllText(path, cameraString);
		}
		// Sphere
		File.AppendAllText(path, "\t std::vector<Sphere>{" + "\n");
		for (int i = 0; i < sphereCenters.Count; i++)
		{
			String sphereString = String.Format("\t\t{{Vector3{{{0}, {1}, {2}}}, {3}, {4}}},\n",
				sphereCenters[i].x, sphereCenters[i].y, sphereCenters[i].z, radiusValues[i], i);
			File.AppendAllText(path, sphereString);
			if (i == sphereCenters.Count - 1)
			{
				File.AppendAllText(path, "\t},\n");
			}
		}
		// Material
		File.AppendAllText(path, "\t std::vector<Material>{" + "\n");
		for (int i = 0; i < materialColors.Count; i++)
		{
			String materialString = String.Format("\t\t{{MaterialType:: {0}, Vector3{{{1}, {2}, {3}}}}},\n",
				"Diffuse", materialColors[i].x, materialColors[i].y, materialColors[i].z);
			File.AppendAllText(path, materialString);
			if (i == materialColors.Count - 1)
			{
				File.AppendAllText(path, "\t},\n");
			}
		}
		//PointLight
		File.AppendAllText(path, "\t std::vector<PointLight>{" + "\n");
		for (int i = 0; i < pointLights.Count; i++)
		{
			String pointLightString = String.Format("\t\t{{Vector3{{{0}, {0}, {0}}}, Vector3{{{1}, {2}, {3}}}}},\n",
				pointLights[i].GetComponent<Light>().range* pointLights[i].GetComponent<Light>().intensity,
				pointLights[i].transform.position.x, pointLights[i].transform.position.y, pointLights[i].transform.position.z);
			File.AppendAllText(path, pointLightString);
			if (i == pointLights.Count - 1)
			{
				File.AppendAllText(path, "\t},\n");
			}
		}
		File.AppendAllText(path, "};\n");
	}

	private void WriteChildrenInfoByName(GameObject obj, string name)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			if (obj.transform.GetChild(i).name.Contains(name))
			{
				WriteChildInfo(obj.transform.GetChild(i).gameObject);
			}
			WriteChildrenInfoByName(transform.GetChild(i).gameObject, name);
		}
	}

	private void WriteChildInfo(GameObject obj)
	{
		int index = sphereCenters.Count;
		sphereCenters.Add(obj.transform.position);
		radiusValues.Add(obj.transform.localScale.x);
		Color materialColor = obj.GetComponent<Renderer>().material.color;
		materialColors.Add(new Vector3(materialColor.r, materialColor.g, materialColor.b));
	}
}
