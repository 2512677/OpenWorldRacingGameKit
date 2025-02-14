using UnityEngine;

public class chasemodecheaker : MonoBehaviour
{
	public string rt;

	public GameObject chasemodeobject;

	private void Start()
	{
		rt = PlayerPrefs.GetString("RaceType");
		if (rt == "Chase")
		{
			chasemodeobject.SetActive(value: true);
		}
	}

	private void Update()
	{
	}
}
